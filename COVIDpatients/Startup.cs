using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using COVIDpatients.Model;
using COVIDpatients.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Serilog;

namespace COVIDpatients
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
            services.AddApplicationInsightsTelemetry();

            services.AddControllers(options =>
            {
                options.RespectBrowserAcceptHeader = true;
            }
            ).AddXmlSerializerFormatters();

            services.AddControllers();

            // Que ServiceBus
            services.AddScoped<ServiceBusSender>();

            // Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                // ClientId: 0a862973-d546-4aca-a248-1d6743ab05cd
                // Directory Id: 61a84301-8c97-40f0-aa97-66d871c63d8f

                options.Authority = "https://login.microsoftonline.com/61a84301-8c97-40f0-aa97-66d871c63d8f/v2.0/";
                options.Audience = "api://0a862973-d546-4aca-a248-1d6743ab05cd";
                options.TokenValidationParameters.ValidateIssuer = false;

                //PR
                // ClientId: fce95216-40e5-4a34-b041-f287e46532be
                // Directory ID: 146ab906-a33d-47df-ae47-fb16c039ef96

                //options.Authority = "https://login.microsoftonline.com/146ab906-a33d-47df-ae47-fb16c039ef96/v2.0/";
                //options.Audience = "api://fce95216-40e5-4a34-b041-f287e46532be";
                //options.TokenValidationParameters.ValidateIssuer = false;

                options.IncludeErrorDetails = true;
            });

            IdentityModelEventSource.ShowPII = true;

            services.AddDbContext<DpDataContext>(options =>
                {
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                });
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();

            app.UseSwaggerUI(swagger =>
            {
                swagger.SwaggerEndpoint("/swagger/v1/swagger.json", "Name");
             });
            
        }
    }
}
