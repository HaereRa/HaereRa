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
using System.Linq;

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

        public async Task<ExternalUserAccount> GetProfileAsync(string accountIdentifier, CancellationToken cancellationToken = default(CancellationToken))
        {
            var getUserResponse = await _iamClient.GetUserAsync(new GetUserRequest { UserName = accountIdentifier }, cancellationToken);
            if (getUserResponse.User == null) return null;

            return new ExternalUserAccount
            {
                AccountIdentifier = getUserResponse.User.UserName,
                IsActive = true,
            };
        }

        public async Task<IEnumerable<ExternalUserAccount>> ListProfilesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var listUsersResponse = await _iamClient.ListUsersAsync(cancellationToken);
            var allUsers = new List<User>();
            var stillSearching = true;
            while (stillSearching)
            {
                allUsers.AddRange(listUsersResponse.Users);

                if (listUsersResponse.IsTruncated)
                {
                    listUsersResponse = await _iamClient.ListUsersAsync(new ListUsersRequest { Marker = listUsersResponse.Marker }, cancellationToken);
                    stillSearching = true;
                }
                else
                {
                    stillSearching = false;
                }
            }

            return allUsers.Select(u => new ExternalUserAccount
            {
                AccountIdentifier = u.UserName,
                IsActive = true,
            });
        }
    }
}
