using GeoTagAPI_Project.ApiKey;
using GeoTagAPI_Project.Data;
using GeoTagAPI_Project.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;

namespace GeoTagAPI_Project
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

            services.AddControllers();


            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(2,0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });

            services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GeoTagAPI_Project", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "GeoTagAPI_Project", Version = "v2" });

                var docsPath = Path.Combine(AppContext.BaseDirectory, "Documentation.xml"); 
                c.IncludeXmlComments(docsPath);
                //c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });


            services.AddDbContext<GeoTagDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("GeoTagDbContextConnection")));


            services.AddIdentityCore<User>()
                .AddEntityFrameworkStores<GeoTagDbContext>();


            services.AddAuthorization(options =>
            {
                options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
            });

            services.AddAuthentication("ApiTokenScheme")
                .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>("ApiTokenScheme", null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => 
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GeoTagAPI_Project v1");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "GeoTagAPI_Project v2");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
