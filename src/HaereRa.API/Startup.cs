using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HaereRa.API.DAL;
using HaereRa.API.GraphQL;
using HaereRa.API.Services;

namespace HaereRa.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add Entity Framework
            services.AddDbContext<HaereRaDbContext>(options => options.UseSqlite("Data Source=data.db"));

            // Add GraphQL things
            services.AddTransient<HaereRaQuery>();
            services.AddTransient<HaereRaMutation>();

			// Add other services
			services.AddTransient<IPersonService, PersonService>();
			services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<ISuggestionService, SuggestionsService>();
            services.AddTransient<IGraphQLService, GraphQLService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseGraphiQl("/graphiql");

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
