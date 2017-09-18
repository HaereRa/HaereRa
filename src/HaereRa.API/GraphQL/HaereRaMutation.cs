using Microsoft.AspNetCore.Http;
using GraphQL.Types;
using HaereRa.API.GraphQL.Types;
using HaereRa.API.Services;

namespace HaereRa.API.GraphQL
{
    public class HaereRaMutation : ObjectGraphType
    {
        public HaereRaMutation(IHttpContextAccessor httpContextAccessor, ISuggestionService suggestionService)
        {
            var user = httpContextAccessor.HttpContext.User;

			Field<ExternalAccountSuggestionType>(
			"acceptSuggestion",
			arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "Id" }),
            resolve: context =>
            {
                // userContext = context.UserContext.As<GraphQLUserContext>();
                var id = context.GetArgument<int>("Id");
                suggestionService.AcceptSuggestionAsync(id).Wait(); // TODO: Error handling & async
                return suggestionService.GetSuggestionAsync(id).Result; // TODO: Async
            });

            Field<ExternalAccountSuggestionType>(
			"rejectSuggestion",
			arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "Id" }),
			resolve: context =>
			{
				var id = context.GetArgument<int>("Id");
				suggestionService.RejectSuggestionAsync(id).Wait(); // TODO: Error handling & async
				return suggestionService.GetSuggestionAsync(id).Result; // TODO: Async
			});
        }
    }
}
