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
using System.Configuration;
using System.Reflection;
using Blocks.Core;
using Blocks.Core.ApplicationInformation;
using Blocks.Core.LogEngines.SeriLog;
using Blocks.Core.Setup;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.ElasticSearch;

namespace PowerUps.MicroServices.Core
{
    public static class MicroServiecsDefaults
    {
        public static BlocksOptions CreateBlocksOptions(
            Assembly assembly,
            params IBlock[] blocks)
        {
            var appInformation = AppInformation.Create(
                new AppName(ConfigurationManager.AppSettings["IMicroServicesCoreConfiguration_AppName"]),
                assembly.GetName().Version,
                new EnvName(ConfigurationManager.AppSettings["IMicroServicesCoreConfiguration_EnvName"]));
            var logEventLevel = SeriLogEngine.ParseLogEventLevel(ConfigurationManager.AppSettings["IMicroServicesCoreConfiguration_LogLevel"]);
            var elasticSearchUrl = ConfigurationManager.AppSettings["IMicroServicesCoreConfiguration_ElasticSearchUrl"];

            var logEngine = new SeriLogEngine(
                Environment.UserInteractive,
                logEventLevel,
                appInformation,
                (lc, lel) => ConfigureLog(lc, lel, elasticSearchUrl));

            return BlocksOptions.Create(
                appInformation,
                () => logEngine,
                blocks);
        }

        private static LoggerConfiguration ConfigureLog(LoggerConfiguration loggerConfiguration, LogEventLevel logEventLevel, string elasticSearchUrl)
        {
            return loggerConfiguration
                .WriteTo.Sink(new ElasticSearchSink(new Uri(elasticSearchUrl), "logstash-{0:yyyy.MM}", 1, 1000, TimeSpan.FromSeconds(3), null), logEventLevel);
        }
    }
}
