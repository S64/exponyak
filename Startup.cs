using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using S64.Bot.Builder.Adapters.Slack;
using S64.Bot.Builder.Adapters.Slack.AspNetCore;

namespace ExPonyak
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSlackBot<ExPonyakBot>(options =>
            {
                options.Middleware = new List<IMiddleware> { new SlackMessageTypeMiddleware() };
                options.SlackOptions = Program.Options;
                options.Paths = new SlackBotPaths
                {
                    BasePath = "/api",
                    RequestPath = "events",
                };
                options.VerificationToken = Program.VerificationToken;
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseSlack();
        }

    }
}
