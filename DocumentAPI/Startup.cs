using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using DocumentAPI.Infrastructure.Interfaces;
using DocumentAPI.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Net;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Reflection;
using Microsoft.OpenApi.Models;
using System.Linq;
using Amazon.S3;

namespace DocumentAPI
{
    public class Startup
    {
        public Startup(ILogger<Startup> logger, IHostingEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        private ILogger _logger;
        public IConfiguration _configuration { get; set; }

        private IHostingEnvironment _env;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(_env.ContentRootPath)
                .AddJsonFile($"RepoConfig.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (_env.IsProduction() || _env.IsEnvironment("Test"))
            {
                configBuilder.AddEnvironmentVariables();
            }
            else
            {
                configBuilder.AddUserSecrets<Startup>();
            }

            _configuration = configBuilder.Build();
            if (!_env.IsProduction())
            {
                var keys = _configuration.AsEnumerable().ToList();

                _logger.LogInformation($"Config Loaded: {JsonConvert.SerializeObject(keys)}");
            }

            services.AddSingleton(_configuration);

            var httpClientHandler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            var httpClient = new HttpClient(httpClientHandler);

            services.AddAWSService<IAmazonS3>();
            services.AddSingleton(httpClient);

            services.AddLogging(l =>
            {
                l.AddAWSProvider(_configuration.GetAWSLoggingConfigSection());
                l.SetMinimumLevel(LogLevel.Debug);
            });

            services.TryAddSingleton<IHealthCheckServices, HealthCheckServices>();
            services.TryAddTransient<IQueryAppsServices, QueryAppsServices>();
            services.AddHttpClient<IQueryAppsServices, QueryAppsServices>();

            services.AddCors(options =>
                 {
                     options.AddPolicy("AllowAll",
                         builder =>
                         {
                             builder
                                 .AllowAnyOrigin()
                                 .AllowAnyHeader()
                                 .AllowAnyMethod();
                         });
                     options.AddPolicy("AllowCredentials",
                        builder =>
                        {
                            builder
                                .AllowAnyMethod()
                                .AllowAnyHeader()

                                .AllowCredentials()
                                .SetIsOriginAllowed(hostName => true);
                        });
                 });


            services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = $"Document API - {_env.EnvironmentName}", Version = "v1.0" });
                var xmlFile = Path.ChangeExtension(typeof(Startup).Assembly.Location, ".xml");
                c.IncludeXmlComments(xmlFile);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app
                .UseCors("AllowAll")
                .UseCors("AllowCredentials");

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Document API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (env.IsProduction())
            {
                app.UseExceptionHandler(config =>
                {
                    config.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        context.Response.ContentType = "application/json";

                        var error = context.Features.Get<IExceptionHandlerFeature>();
                        if (error != null)
                        {
                            var ex = error?.Error;
                            var message = ex.Message;
                            var description = ex;

                            _logger.LogError("An Error Has Occurred", ex);

                            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                            {
                                Message = message,
                                Description = description
                            }));

                        }
                    });
                });
            }
        }
    }
}
