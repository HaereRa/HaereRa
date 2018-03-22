using Microsoft.AspNetCore.Http;
using GraphQL.Types;
using HaereRa.API.GraphQL.Types;
using HaereRa.API.Services;

namespace HaereRa.API.GraphQL
{
    public class HaereRaMutation : ObjectGraphType
    {
        public HaereRaMutation(IHttpContextAccessor httpContextAccessor, IPersonService personService, IExternalAccountService externalAccountService)
        {
            var user = httpContextAccessor.HttpContext.User;

			Field<ExternalAccountSuggestionStatusEnum>(
			"acceptSuggestion",
			arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "Id" }),
            resolve: context =>
            {
                // userContext = context.UserContext.As<GraphQLUserContext>();
                var id = context.GetArgument<int>("Id");
                externalAccountService.AcceptSuggestionAsync(id).Wait(); // TODO: Error handling & async
                return externalAccountService.GetExternalAccountAsync(id).Result; // TODO: Async
            });

            Field<ExternalAccountSuggestionStatusEnum>(
			"rejectSuggestion",
			arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "Id" }),
			resolve: context =>
			{
				var id = context.GetArgument<int>("Id");
                externalAccountService.RejectSuggestionAsync(id).Wait(); // TODO: Error handling & async
                return externalAccountService.GetExternalAccountAsync(id).Result; // TODO: Async
            });

            Field<PersonType>(
            "scanUserForSuggestions",
            arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "Id" }),
            resolve: context =>
            {
                var id = context.GetArgument<int>("Id");
                externalAccountService.UpdateSuggestionsAsync(id).Wait(); // TODO: Error handling & async
                return personService.GetPersonAsync(id).Result; // TODO: Async
            });
        }
    }
}
