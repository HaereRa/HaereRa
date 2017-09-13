using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;

namespace HaereRa.Plugin
{
    public abstract class ExternalPlatformPlugin
    {
        public bool CanFetchExternalUserAccounts
        {
            get
            {
                return this.GetType().GetTypeInfo().GetInterfaces()     // Get list of interfaces 
                           .Select(i => i.Name)                         // as a list of strings
                           .Contains(nameof(IFetchExternalUserAccounts));   // than check to see if any of them match what we're looking for
            }
        }

        public bool CanProvisionExternalUserAccounts
        {
            get
            {
                return this.GetType().GetTypeInfo().GetInterfaces()         // Get list of interfaces 
                           .Select(i => i.Name)                             // as a list of strings
                           .Contains(nameof(IProvisionExternalUserAccounts));   // than check to see if any of them match what we're looking for
            }
        }

        public bool CanEnableDisableExternalUserAccounts
        {
            get
            {
                return this.GetType().GetTypeInfo().GetInterfaces()             // Get list of interfaces 
                           .Select(i => i.Name)                                 // as a list of strings
                           .Contains(nameof(IEnableDisableExternalUserAccounts));   // than check to see if any of them match what we're looking for
            }
        }
    }
}
