using System;
using System.Buffers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using React.AspNet;
using UrlMetadata.Services;
using UrlMetadata.Services.Interfaces;
using JavaScriptEngineSwitcher.Extensions.MsDependencyInjection;
using JavaScriptEngineSwitcher.ChakraCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using UrlMetadata.Middleware;

namespace UrlMetadata
{
    public class Startup
    {
        private const string AllowAllOrigins = "_AllowAllOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJsEngineSwitcher(options =>
                options.DefaultEngineName = ChakraCoreJsEngine.EngineName
            )
                .AddChakraCore();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();

            services.AddMvc(options =>
            {
                options.OutputFormatters.RemoveType<JsonOutputFormatter>();
                options.OutputFormatters.Add(new CustomJsonOutputFormatter());
            })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSingleton<IUrlService, UrlService>();

            services.AddCors(options =>
            {
                options.AddPolicy(AllowAllOrigins,
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseReact(config =>
            {
                config.AddScript("~/js/main.jsx");
            });

            app.UseStaticFiles();

            app.UseMvc();
        }
    }
}
