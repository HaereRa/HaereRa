using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using HaereRa.API.ViewModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
using HaereRa.API.GraphQL;
using GraphQL;

namespace HaereRa.API.Controllers
{
    [Route("[controller]")]
    public class GraphQLController : Controller
    {
        private HaereRaQuery _haereRaQuery { get; set; }

        public GraphQLController(HaereRaQuery haereRaQuery)
        {
            _haereRaQuery = haereRaQuery;
        }

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Post([FromBody] GraphQLQuery query)
		{
            var schema = new Schema { 
                Query = _haereRaQuery, 
            };

			var result = await new DocumentExecuter().ExecuteAsync(_ =>
			{
				_.Schema = schema;
				_.Query = query.Query;
			}).ConfigureAwait(false);

			if (result.Errors?.Count > 0)
			{
				return BadRequest();
			}

			return Ok(result);
		}
    }
}
