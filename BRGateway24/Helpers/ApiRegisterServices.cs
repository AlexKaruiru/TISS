using BRGateway24.DataAccess;
using BRGateway24.Models;
using BRGateway24.Repository.AuditRepo;
using BRGateway24.Repository.Common;
using BRGateway24.Repository.ESS.Interfaces;
using BRGateway24.Repository.Loans;
using BRGateway24.Repository.ProductRepo;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using SwiftApi.Repository.Security;
using System.Collections.ObjectModel;
using System.Data;

namespace BRGateway24.Helpers
{
    public static class ApiRegisterServices
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, AppSettings appSettings)
        {

            services.AddTransient<ISystemSecurity, SystemSecurity>();
            services.AddTransient<ICommonRepo, CommonRepo>();
            services.AddTransient<IProductRepo, ProductRepo>();
            services.AddTransient<IESSLoansRepo, ESSLoansRepo>();
            services.AddTransient<IAuditRepo, AuditRepo>();
            services.AddSingleton<HealthyChecks>();



            string dbConn = Constants.GetConnectionString(appSettings.DBType, appSettings.DBServerName, appSettings.DatabaseName, appSettings.BRUserName, appSettings.BRUserPassword, "BRGateway24API");



            //configure Serilog Logger
            string ErrorLogFile = appSettings.ErrorLogPath + "ApiLogs.txt";
            string myOutputTemplate = "{Timestamp:yyyy-MM-dd HH: mm: ss.fff}[{ Level}] { Message }{ NewLine}{ Exception}";


            string connectionString = dbConn;
            var columnOptions = new ColumnOptions
            {
                AdditionalColumns = new Collection<SqlColumn>
               {
                   new SqlColumn("UserName", SqlDbType.NVarChar)
                 }
            };


            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()
              .WriteTo.MSSqlServer(connectionString, sinkOptions: new MSSqlServerSinkOptions { TableName = "t_Gateway24Logs" }
              , null, null, LogEventLevel.Information, null, columnOptions: columnOptions, null, null)
              .MinimumLevel.Override("Microsoft", LogEventLevel.Error)//Capture Information and error only  
              .MinimumLevel.Override("System", LogEventLevel.Warning)
               .WriteTo.File(ErrorLogFile, rollingInterval: RollingInterval.Day)//,outputTemplate: myOutputTemplate)
               .WriteTo.Console()
              .CreateLogger();

            return services;
        }
    }
}
