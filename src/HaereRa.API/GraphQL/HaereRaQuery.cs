using System;
using GraphQL.Types;
using HaereRa.API.Models;

namespace HaereRa.API.GraphQL
{
    public class HaereRaQuery : ObjectGraphType
    {
        public HaereRaQuery()
        {
            Field<PersonType>(
                "person",
                resolve: context => new Person { Id = 1, FullName = "John Smith", KnownAs = "John", IsAdmin = false }
			);
			Field<PersonType>(
				"person",
				resolve: context => new Person { Id = 1, FullName = "John Smith", KnownAs = "John", IsAdmin = false }
			);
        }
    }
}
