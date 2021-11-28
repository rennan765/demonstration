using _4oito6.Demonstration.Person.EntryPoint.IoC;
using _4oito6.Demonstration.Person.EntryPoint.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// If using Kestrel:
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

// If using IIS:
builder.Services.Configure<IISServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.AddLogging();
builder.Services.AddSingleton<ILoggerFactory, LoggerFactory>();
builder.Services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));

builder.Services.AddAuthentication();

builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build();

    config.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddSingleton<IConfiguration>(sp => builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddPersonApi();
builder.Services.AddSwagger();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").Equals("Local"))
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();