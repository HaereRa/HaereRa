using GraphQL.Types;
using HaereRa.API.GraphQL;

namespace HaereRa.API
{
    public class HaereRaSchema : Schema
    {
        public HaereRaSchema(HaereRaQuery query, HaereRaMutation mutation, HaereRaSubscription subscription)
        {
            Query = query;
            Mutation = mutation;
            Subscription = subscription;
        }
    }
}