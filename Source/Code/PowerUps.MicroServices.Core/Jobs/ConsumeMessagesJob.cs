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

using System.Linq;
using System.Threading.Tasks;
using Blocks.Core;
using Blocks.Core.Configuration;
using Blocks.Messaging;
using Blocks.Messaging.AntiCorruption;
using Blocks.Messaging.Messages;
using Blocks.WindowsService.Jobs;
using Newtonsoft.Json.Linq;

namespace PowerUps.MicroServices.Core.Jobs
{
    public class ConsumeMessagesJob : DisposableObject, IJob
    {
        private readonly ILogger _logger;
        private IExternalMessageBusReceiver _receiver;
        private readonly IApplicationConfiguration<IMicroServicesCoreConfiguration> _configuration;
        private readonly IMessageParserFactory _messageParserFactory;

        public ConsumeMessagesJob(
            ILogger logger,
            IExternalMessageBusReceiver receiver,
            IApplicationConfiguration<IMicroServicesCoreConfiguration> configuration,
            IMessageParserFactory messageParserFactory)
        {
            _logger = logger;
            _receiver = receiver;
            _configuration = configuration;
            _messageParserFactory = messageParserFactory;
        }

        public void Start()
        {
            var connectionStringName = _configuration.Get(c => c.RabbitMqConnectionStringName);
            var connectionString = _configuration.ConnectionString(connectionStringName);
            var exchangeName = new ExchangeName(_configuration.Get(c => c.RabbitMqExchangeName));
            var queueName = new QueueName(_configuration.Get(c => c.RabbitMqQueueName));
            var routingKeys = _messageParserFactory.GetRoutingKeys();

            _receiver.Connect(exchangeName, connectionString);
            _receiver.Subscribe(queueName, b => Task.Factory.StartNew(() => Handle(b)), routingKeys.ToArray());
        }

        public void Stop()
        {
            //
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _receiver.Dispose();
                _receiver = null;
            }

            base.Dispose(disposing);
        }

        private void Handle(JObject body)
        {
            _logger.DebugFormat("Received message\n{0}", body.ToString());
            _messageParserFactory.ParseAndPublish(body);
        }
    }
}
