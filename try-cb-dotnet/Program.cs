using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using try_cb_dotnet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: "_FrontendOrigin",
        builder => builder
            .WithOrigins("http://localhost:8081")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Travel Sample", Version = "v1" });
});

builder.Services.AddRazorPages();

// configure custom services
builder.Services.AddSingleton<ICouchbaseService, CouchbaseService>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuthTokenService, AuthTokenService>();
builder.Services.AddSingleton<IFlightService, FlightService>();
builder.Services.AddSingleton<IAirportsService, AirportsService>();
builder.Services.AddSingleton<IHotelService, HotelService>();

builder.Services.AddCouchbase(builder.Configuration.GetSection("Couchbase"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("_FrontendOrigin");

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

app.Run();