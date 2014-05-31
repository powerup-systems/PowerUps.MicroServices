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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Blocks.Core;
using Blocks.Core.Configuration;
using Blocks.Core.Setup;
using Blocks.Messaging;
using Blocks.Messaging.Messages;
using Blocks.Messaging.Setup;
using Blocks.Nancy.Selfhost.Setup;
using Blocks.TestHelpers;
using Blocks.WindowsService.Jobs;
using Blocks.WindowsService.Setup;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using PowerUps.MicroServices.PushoverFacade;
using PowerUps.MicroServices.PushoverFacade.Jobs;
using PowerUps.MicroServices.PushoverFacade.Setup;

namespace PowerUps.Test.MicroServices.PushoverFacade.Integration.Jobs
{
    [TestFixture]
    public class ComsumeMessagesTests : IntegrationTest
    {
        protected override IEnumerable<IBlock> Blocks
        {
            get
            {
                return new IBlock[]
                    {
                        new CoreBlock(),
                        new MessagingBlock(),
                        new PushoverFacadeBlock(),
                        new WinServiceBlock(),
                        new NancySelfHostBlock(),
                        new PushoverFacadeIntegrationBlock(),
                    };
            }
        }

        [Test]
        [Explicit]
        public void ReveiceTests()
        {
            var configuration = ServiceLocator.Current.GetInstance<IApplicationConfiguration<IPushoverFacadeConfiguration>>();
            configuration.Set(c => c.RabbitMqExchangeName, "PowerUps.Test.MicroServices.PushoverFacade");
            var exchangeName = new ExchangeName(configuration.Get(c => c.RabbitMqExchangeName));

            var sender = ServiceLocator.Current.GetInstance<IExternalMessageBusSender>();
            var consumer = ServiceLocator.Current.GetAllInstances<IJob>()
                .OfType<ConsumeMessagesJob>()
                .First();

            consumer.Start();
            sender.Connect(exchangeName, configuration.ConnectionString(configuration.Get(c => c.RabbitMqConnectionStringName)));

            const string eventType = "powerups.notification.pushover.send";
            sender.Send(
                new
                    {
                        id = Guid.NewGuid(),
                        version = 1,
                        eventType = eventType,
                        apiKey = "",
                        userKey = "",
                        message = "Test",
                    },
                exchangeName,
                new RoutingKey(eventType));

            Thread.Sleep(100000);
        }
    }
}
