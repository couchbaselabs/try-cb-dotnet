using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure custom services
            services.AddSingleton<ICouchbaseService>(new CouchbaseService());
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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
