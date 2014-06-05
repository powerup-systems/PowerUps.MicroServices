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

using System.Collections.Generic;
using Blocks.Core;
using Blocks.Core.Blocks;
using Blocks.Core.Services;
using Blocks.Core.Specifications;
using Blocks.Messaging;
using Blocks.Messaging.Extensions;
using Blocks.Nancy.Selfhost;
using Blocks.WindowsService;
using Blocks.WindowsService.Extensions;

namespace PowerUps.MicroServices.Core.Setup
{
    public class MicroServicesCoreBlock : BlockBase, IMicroServicesCoreBlock
    {
        public override IEnumerable<BlockSpecification> Requirements
        {
            get
            {
                return new[]
                    {
                        new BlockSpecification{PlugIn = typeof(ICoreBlock)},
                        new BlockSpecification{PlugIn = typeof(IMessagingBlock)}, 
                        new BlockSpecification{PlugIn = typeof(INancySelfHostBlock)},
                        new BlockSpecification{PlugIn = typeof(IWinServiceBlock)}, 
                    };
            }
        }

        public override void Initialize(IServiceRegister register)
        {
            base.Initialize(register);
            register.ScanWithDefaultConventions(this);
            register.RegisterJobs(this);
            register.RegisterEventSubscribers(this);
        }
    }
}
