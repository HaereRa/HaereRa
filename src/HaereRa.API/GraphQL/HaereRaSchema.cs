using GraphQL.Types;
using HaereRa.API.GraphQL;
using System.Linq;

namespace HaereRa.API
{
    public class HaereRaSchema : Schema
    {
        public HaereRaSchema(HaereRaQuery query, HaereRaMutation mutation, HaereRaSubscription subscription)
        {
            Query = query.Fields.Any() ? query : null;
            Mutation = mutation.Fields.Any() ? mutation : null;
            Subscription = subscription.Fields.Any() ? subscription : null;
        }
    }
}