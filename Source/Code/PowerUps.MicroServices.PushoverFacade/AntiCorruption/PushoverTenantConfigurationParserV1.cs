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

using Blocks.Core.Messaging.Events;
using Blocks.Messaging.AntiCorruption;
using PowerUps.MicroServices.PushoverFacade.Events;

namespace PowerUps.MicroServices.PushoverFacade.AntiCorruption
{
    public class PushoverTenantConfigurationParserV1 : MessageParser
    {
        public override string RoutingKey { get { return "powerups.notification.facade.pushover.tenantconfiguration"; } }

        public override bool CanParse(dynamic message)
        {
            return message.eventType == RoutingKey && message.version == 1;
        }

        protected override Event Parse(dynamic message)
        {
            return new PushoverTenantConfigurationEvent
                {
                    Id = message.id,
                    Tenant = message.tenant,
                    ApiKey = message.apiKey,
                };
        }
    }
}
