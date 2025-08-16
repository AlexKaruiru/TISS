using BRGateway24.Helpers;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using SwiftApi.Repository.Security;
using System.Text.Json;
using ConfigurationManager = Microsoft.Extensions.Configuration.ConfigurationManager;
var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;


var appSettings = configuration.GetSection("AppSettings");
builder.Services.ConfigureAppSettings<AppSettings>(appSettings);
builder.Services.RegisterServices(appSettings.Get<AppSettings>());

builder.Services.AddSingleton<SystemSecurity>();


var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)  
    .Enrich.FromLogContext();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext() 
    .CreateLogger();

var columnOptions = new ColumnOptions();
columnOptions.Store.Remove(StandardColumn.Properties);
if (!columnOptions.Store.Contains(StandardColumn.LogEvent))
{
    columnOptions.Store.Add(StandardColumn.LogEvent);
}

Log.Logger = loggerConfig.CreateLogger();
builder.Host.UseSerilog((hostingContext, services, loggerConfig) =>
{    
    loggerConfig.ReadFrom.Configuration(hostingContext.Configuration)
          .Enrich.FromLogContext();    
    var systemSecurity = services.GetRequiredService<SystemSecurity>();    
    var appSettings = hostingContext.Configuration
                         .GetSection("AppSettings")
                         .Get<AppSettings>();

    string connString = systemSecurity.GetConnectionString(appSettings.DBType, appSettings.DBServerName, appSettings.DatabaseName, appSettings.BRUserName, appSettings.BRUserPassword, "BRGateway24API");

    
    var columnOptions = new ColumnOptions();
    // Remove the "Properties" column if you don't need it:
    columnOptions.Store.Remove(StandardColumn.Properties);
    // Ensure the XML LogEvent column is present:
    columnOptions.Store.Add(StandardColumn.LogEvent);

    loggerConfig.WriteTo.MSSqlServer(
        connectionString: connString,
        tableName: "t_ApiLogs",
        schemaName: "dbo",
        autoCreateSqlTable: true,
        restrictedToMinimumLevel: LogEventLevel.Information,
        columnOptions: columnOptions
    );
});

// Add services to the container.

builder.Services
  .AddControllers()
  .AddJsonOptions(opts =>
  {
      opts.JsonSerializerOptions.PropertyNamingPolicy = null;
      opts.JsonSerializerOptions.DictionaryKeyPolicy = null;
      opts.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
  });


builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BR Gateway API", Version = "v1" });
    c.SwaggerDoc("v2", new OpenApiInfo { Title = "BR Gateway API", Version = "v2" });
    c.OperationFilter<AuthFilter>(); // Add custom filter here
});

var app = builder.Build();

/////////////////////////DO HEALTH CHECKS BEFORE HANDLING REQUESTS///////////////////////////////////

using (var scope = app.Services.CreateScope())
{
    var spChecker = scope.ServiceProvider.GetRequiredService<HealthyChecks>();
    await spChecker.CheckProceduresAsync(); 
}

//////////////////////////////////////////////////////////////////////////////////////////////////////

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
