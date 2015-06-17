﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Management.Automation;
using Microsoft.WindowsAzure.Commands.ExpressRoute.Properties;
using Microsoft.WindowsAzure.Management.ExpressRoute.Models;

namespace Microsoft.WindowsAzure.Commands.ExpressRoute
{

    [Cmdlet(VerbsCommon.Set, "AzureBGPPeering"), OutputType(typeof(AzureBgpPeering))]
    public class SetAzureBGPPeeringCommand : ExpressRouteBaseCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, HelpMessage = "Service Key representing Azure Circuit for which BGP peering needs to be created/modified")]
        public Guid ServiceKey { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Advertised Public Prefixes")]
        [ValidateNotNullOrEmpty]
        public string AdvertisedPublicPrefixes;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Customer AS number")]
        public UInt32 CustomerAsn { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true,
            HelpMessage = "Peer Asn")]
        public UInt32? PeerAsn { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Primary Peer Subnet")]
        [ValidateNotNullOrEmpty]
        public string PrimaryPeerSubnet { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Routing Registry Name for Prefix Validation")]
        public string RoutingRegistryName { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Secondary Peer Subnet")]
        [ValidateNotNullOrEmpty]
        public string SecondaryPeerSubnet { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Shared Key")]
        [ValidateNotNullOrEmpty]
        public string SharedKey { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Vlan Id")]
        public UInt32? VlanId { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, HelpMessage = "Bgp Peering Access Type: Microsoft, Public or Private")]
        [DefaultValue("Private")]
        public BgpPeeringAccessType AccessType { get; set; }

        public override void ExecuteCmdlet()
        {
            try
            {
                var route = ExpressRouteClient.GetAzureBGPPeering(ServiceKey, AccessType);
                var updatedRoute = ExpressRouteClient.UpdateAzureBGPPeering(ServiceKey, AccessType, CustomerAsn,
                    PeerAsn.HasValue ? PeerAsn.Value : route.PeerAsn, PrimaryPeerSubnet ?? route.PrimaryPeerSubnet, RoutingRegistryName,
                    SecondaryPeerSubnet ?? route.SecondaryPeerSubnet, VlanId.HasValue ? VlanId.Value : route.VlanId,
                    SharedKey);
                WriteObject(updatedRoute, false);
            }
            catch
            {
                if (!PeerAsn.HasValue)
                {
                    throw new ArgumentException(Resources.PeerAsnRequired);
                }

                if (!VlanId.HasValue)
                {
                    throw new ArgumentException(Resources.VlanIdRequired);
                }

                if (PrimaryPeerSubnet == null)
                {
                    throw new ArgumentException(Resources.PrimaryPeerSubnetRequired);
                }

                if (SecondaryPeerSubnet == null)
                {
                    throw new ArgumentException(Resources.SecondaryPeerSubnetRequired);
                }

                var newRoute = ExpressRouteClient.NewAzureBGPPeering(ServiceKey, AdvertisedPublicPrefixes, CustomerAsn,
                    PeerAsn.Value, PrimaryPeerSubnet, RoutingRegistryName, SecondaryPeerSubnet, VlanId.Value, AccessType,
                    SharedKey);
                WriteObject(newRoute);
            }
        }
    }
}
