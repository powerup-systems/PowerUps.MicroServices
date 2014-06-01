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
using Blocks.Core;
using Blocks.Persistence;
using Microsoft.Practices.ServiceLocation;
using PowerUps.MicroServices.PushoverFacade.Database.DTOs;

namespace PowerUps.MicroServices.PushoverFacade.Database.Repositories
{
    public class TenantApiKeyRepository : ITenantApiKeyRepository
    {
        private readonly ILogger _logger;

        public TenantApiKeyRepository(
            ILogger logger)
        {
            _logger = logger;
        }

        public void Store(TenantApiKey tenantApiKey)
        {
            _logger.InfoFormat(
                "Storing new Pushover API key for tenant '{0}' with value '{1}'",
                tenantApiKey.Tenant,
                tenantApiKey.ApiKey);

            Delete(tenantApiKey.Tenant);
            using (var repository = ServiceLocator.Current.GetInstance<IRepository>())
            {
                repository.Add(tenantApiKey);
                repository.SaveChanges();
            }
        }

        public TenantApiKey Fetch(string tenant)
        {
            using (var repository = ServiceLocator.Current.GetInstance<IRepository>())
            {
                return repository.Query<TenantApiKey>().SingleOrDefault(t => t.Tenant == tenant);
            }
        }

        public void Delete(string tenant)
        {
            _logger.InfoFormat(
                "Deleting Pushover API key for tenant '{0}'",
                tenant);

            using (var repository = ServiceLocator.Current.GetInstance<IRepository>())
            {
                var tenantApiKey = repository.Query<TenantApiKey>().SingleOrDefault(t => t.Tenant == tenant);
                if (tenantApiKey == null)
                {
                    return;
                }
                repository.Remove(tenantApiKey);
                repository.SaveChanges();
            }
        }
    }
}
