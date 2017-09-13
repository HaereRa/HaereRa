using GraphQL.Types;
using Microsoft.AspNetCore.Http;

namespace HaereRa.API.GraphQL
{
    public class HaereRaSubscription : ObjectGraphType
    {
        public HaereRaSubscription(IHttpContextAccessor httpContextAccessor)
        {
            var user = httpContextAccessor.HttpContext.User;
        }
    }
}