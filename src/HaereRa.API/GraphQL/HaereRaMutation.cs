using System;
using GraphQL.Types;
using HaereRa.API.DAL;
using HaereRa.API.Services;
using GraphQL;

using HaereRa.GraphQL.Services;

namespace HaereRa.API.GraphQL
{
    public class HaereRaMutation : ObjectGraphType
    {
        public HaereRaMutation(ISuggestionService suggestionService)
        {
			Field<ProfileSuggestionType>(
			"acceptSuggestion",
			arguments: new QueryArguments(new QueryArgument<IntGraphType> { Name = "Id" }),
            resolve: context =>
            {
                // userContext = context.UserContext.As<GraphQLUserContext>();
                var id = context.GetArgument<int>("Id");
                suggestionService.AcceptSuggestionAsync(id).Wait(); // TODO: Error handling & async
                return suggestionService.GetSuggestionAsync(id).Result; // TODO: Async
            });

			Field<ProfileSuggestionType>(
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
