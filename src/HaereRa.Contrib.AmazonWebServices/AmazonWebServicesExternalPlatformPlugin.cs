using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HaereRa.Plugin;
using Microsoft.Extensions.Options;
using Amazon.IdentityManagement;
using Amazon.Runtime;
using Amazon.IdentityManagement.Model;
using System.Threading;
using HaereRa.Plugin.Models;
using HaereRa.Contrib.AmazonWebServices.Options;

namespace HaereRa.Contrib.AmazonWebServices
{
    public class AmazonWebServicesExternalPlatformPlugin : ExternalPlatformPlugin, IFetchExternalUserAccounts, IProvisionExternalUserAccounts
    {
        private readonly AmazonWebServicesExternalPlatformPluginOptions _options;
        private readonly AmazonIdentityManagementServiceClient _iamClient;

        public AmazonWebServicesExternalPlatformPlugin(IOptions<AmazonWebServicesExternalPlatformPluginOptions> options)
        {
            _options = options.Value;
            _iamClient = new AmazonIdentityManagementServiceClient(_options.AccessKey, _options.SecretKey);
        }

        public Task<ExternalUserAccount> GetProfileAsync(string accountIdentifier, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<List<ExternalUserAccount>> ListProfilesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
