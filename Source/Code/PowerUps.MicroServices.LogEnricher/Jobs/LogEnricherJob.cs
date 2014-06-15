/* -----------------------------------------------------------------------------
 * This source file is part of PowerUps Micro Services
 * For the latest info, contact r@smus.nu
 *
 * Copyright (c) 2014 PowerUp Systems (http://powerup.systems/)
 * 
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU Lesser General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with
 * this program; if not, write to the Free Software Foundation, Inc., 59 Temple
 * Place - Suite 330, Boston, MA 02111-1307, USA, or go to
 * http://www.gnu.org/copyleft/lesser.txt.
 * -----------------------------------------------------------------------------
 */

using System;
using System.Linq;
using System.Net;
using Blocks.Core;
using Blocks.Core.Configuration;
using Blocks.WindowsService.Jobs;
using Nest;
using PowerUps.MicroServices.Core;
using PowerUps.MicroServices.LogEnricher.GeoIp;

namespace PowerUps.MicroServices.LogEnricher.Jobs
{
    public class LogEnricherJob : ThreadJob
    {
        private readonly IGeoIpFetcher _geoIpFetcher;
        private readonly IApplicationConfiguration<IMicroServicesCoreConfiguration> _configuration;
        private readonly ElasticClient _elasticClient;

        protected override TimeSpan SleepPeriod { get { return TimeSpan.FromMinutes(5); } }

        public LogEnricherJob(
            ILogger logger,
            IGeoIpFetcher geoIpFetcher,
            IApplicationConfiguration<IMicroServicesCoreConfiguration> configuration)
            : base(logger)
        {
            _geoIpFetcher = geoIpFetcher;
            _configuration = configuration;

            var elasticSearchUrl = _configuration.Get(c => c.ElasticSearchUrl);
            _elasticClient = new ElasticClient(new ConnectionSettings(new Uri(elasticSearchUrl)));
        }

        protected override void Execute()
        {
            var response = _elasticClient.Search(sd => sd
                .Index("logstash*")
                .MatchAll()
                .Filter(f => f.Exists("src_ip") && f.Missing("geo_json"))
                .Size(10)
                .Scroll("1m"));

            var cnt = 0;
            while (response.Documents.Any())
            {
                foreach (var hit in response.DocumentsWithMetaData)
                {
                    var ip = (string)hit.Source.src_ip;
                    if (string.IsNullOrEmpty(ip) ||
                        string.Equals(ip, "127.0.0.1") ||
                        string.Equals(ip, "::1"))
                    {
                        continue;
                    }

                    var ipAddress = IPAddress.Parse(ip);
                    var geoIp = _geoIpFetcher.Fetch(ipAddress);
                    var latitude = double.Parse(geoIp.Latitude);
                    var longitude = double.Parse(geoIp.Longitude);

                    dynamic updateDoc = new System.Dynamic.ExpandoObject();
                    updateDoc.geo_country_name = geoIp.CountryName;
                    updateDoc.geo_country_code = geoIp.CountryCode;
                    updateDoc.geo_city = geoIp.City;
                    updateDoc.geo_longitude = geoIp.Longitude;
                    updateDoc.geo_latitude = geoIp.Latitude;
                    updateDoc.geo_json = new[] { longitude, latitude };
                    updateDoc.geo_description = string.Format("{0} - {1}", geoIp.CountryName, geoIp.City);

                    _elasticClient.Update<object>(u => u
                        .Index(hit.Index)
                        .Id(hit.Id)
                        .Type(hit.Type)
                        .RetriesOnConflict(4)
                        .Document(updateDoc));
                    cnt++;
                }

                response = _elasticClient.Scroll("1m", response.ScrollId);
            }

            if (cnt > 0)
            {
                Logger.InfoFormat("Processed {0} new log messages", cnt);
            }
        }
    }
}
