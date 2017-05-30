using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HaereRa.Plugin;
using Microsoft.Extensions.Options;
using Amazon.IdentityManagement;
using Amazon.Runtime;
using Amazon.IdentityManagement.Model;

namespace HaereRa.Contrib.AmazonWebServices
{
    public class AmazonWebServicesProfileType : IHaereRaProfileType
    {
        private readonly AmazonWebServicesProfileTypeOptions _options;
        private readonly AmazonIdentityManagementServiceClient _iamClient;

        public AmazonWebServicesProfileType(IOptions<AmazonWebServicesProfileTypeOptions> options)
        {
            _options = options.Value;
            _iamClient = new AmazonIdentityManagementServiceClient(_options.AccessKey, _options.SecretKey);
        }

        public Task ActivateProfileAsync(string accountIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task DeactivateProfileAsync(string accountIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileTypeListing> GetProfileAsync(string accountIdentifier)
        {
            throw new NotImplementedException();
        }

        public Task<List<string>> ListProfilesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
