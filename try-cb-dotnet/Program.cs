using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using try_cb_dotnet.Helpers;
using try_cb_dotnet.Services;

var builder = WebApplication.CreateBuilder(args);

const string frontendOrigin = "_FrontendOrigin";

//get config from the configuration
var appSettingsSection = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettingsSection);

// Add services to the container.
builder.Services.AddSingleton<ICouchbaseService, CouchbaseService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuthTokenService, AuthTokenService>();
builder.Services.AddSingleton<IFlightService, FlightService>();
builder.Services.AddSingleton<IAirportsService, AirportsService>();
builder.Services.AddSingleton<IHotelService, HotelService>();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Travel Sample", Version = "v1" });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: frontendOrigin,
        b => b 
            .WithOrigins("http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod()
            // .AllowCredentials()
    );
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(frontendOrigin);

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger.json", "travel-sample API");
    c.RoutePrefix = "apidocs";
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();