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

using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace PowerUps.MicroServices.LogEnricher.GeoIp
{
    public class GeoIpFetcher : IGeoIpFetcher
    {
        private readonly IRestClient _restClient;

        public GeoIpFetcher()
        {
            _restClient = new RestClient("http://10.0.1.24:9080");
        }

        public GeoIpDto Fetch(IPAddress ipAddress)
        {
            var restRequest = new RestRequest(string.Format("/json/{0}", ipAddress));

            var restResponse = _restClient.Execute(restRequest);
            if (string.IsNullOrEmpty(restResponse.Content) || restResponse.ResponseStatus != ResponseStatus.Completed)
            {
                return null;
            }

            var geoIp = JsonConvert.DeserializeObject<GeoIpDto>(restResponse.Content);
            return geoIp;
        }
    }
}
