﻿/* -----------------------------------------------------------------------------
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

using Blocks.Core.Utilities;
using Blocks.Nancy.Selfhost;

namespace PowerUps.MicroServices.Core
{
    public interface IMicroServicesCoreConfiguration : ISelfhostConfiguration
    {
        string EnvName { get; set; }

        string AppName { get; set; }

        [DefaultConfiguration("RabbitMqConnectionString")]
        string RabbitMqConnectionStringName { get; set; }

        [DefaultConfiguration("PowerUps.MicroServices")]
        string RabbitMqExchangeName { get; set; }

        [DefaultConfiguration("MicroService")]
        string RabbitMqQueueName { get; set; }

        [DefaultConfiguration("DEBUG")]
        string LogLevel { get; set; }

        [DefaultConfiguration("http://localhost:9200")]
        string ElasticSearchUrl { get; set; }
    }
}
