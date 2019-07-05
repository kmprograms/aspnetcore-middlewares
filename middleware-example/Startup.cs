using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace middleware_example
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseAuthentication();
            // app.UseStaticFiles();
            // app.UseCors();

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("<h1>MID 1</h1>");
                await next();
            });

            // app.Run(async context => { await context.Response.WriteAsync("<h1>RUN</h1>"); });

            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("<h1>MID 2</h1>");
                await next();
            });

            app.Map("/req", action =>
            {
                action.Use(async (context, next) =>
                {
                    await context.Response.WriteAsync("<h1>MAP 1.1</h1>");
                    await next();
                });

                action.Use(async (context, next) =>
                {
                    await context.Response.WriteAsync("<h1>MAP 1.2</h1>");
                    await next();
                });

                action.Run(async context => { await context.Response.WriteAsync("<h1>MAP 1.3</h1>"); });
            });

            app.MapWhen(
                context => context.Request.Query.ContainsKey("name"),
                action =>
                {
                    action.Run(async context =>
                    {
                        var nameParam = context.Request.Query["name"];
                        await context.Response.WriteAsync($"<h1>Name: {nameParam}</h1>");
                    });
                });


            app.UseMvc();

        }
    }
}
