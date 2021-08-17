using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using try_cb_dotnet.Helpers;
using try_cb_dotnet.Services;

namespace try_cb_dotnet
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        readonly string FrontendOrigin = "_FrontendOrigin";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: FrontendOrigin,
                    builder => builder
                        .WithOrigins("http://localhost:8081")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        // .AllowCredentials()
                );
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Travel Sample", Version = "v1" });
            });

            services.AddRazorPages();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure custom services
            services.AddSingleton<ICouchbaseService, CouchbaseService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IAuthTokenService, AuthTokenService>();
            services.AddSingleton<IFlightService, FlightService>();
            services.AddSingleton<IAirportsService, AirportsService>();
            services.AddSingleton<IHotelService, HotelService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors(FrontendOrigin);

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger.json", "travel-sample API");
                c.RoutePrefix = "apidocs";
            });

            app.UseAuthorization();
            
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
