using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            services.AddMvc();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure custom services
            services.AddSingleton<ICouchbaseService>(new CouchbaseService());
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IAuthTokenService, AuthTokenService>();
            services.AddSingleton<IFlightService, FlightService>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // normally index would be provided via MCV, but we want to use a static html file instead
            var options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("index.html");
            app.UseDefaultFiles(options);

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}
