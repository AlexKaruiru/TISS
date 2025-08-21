using BRGateway24.Repository.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SwiftApi.Repository.Security;
using System;
using System.Collections.Generic;

namespace BRGateway24.Helpers
{
    public class HealthyChecks
    {
        private readonly IConfiguration _config;
        private readonly ILogger<HealthyChecks> _logger;
        private readonly ISystemSecurity _systemSecurity;
        private readonly AppSettings _appsettings;
        private readonly ICommonRepo _commonRepo;
        private readonly List<string> _criticalProcedures = new()
    {
        "p_AddEditAPIAuditLogGwy24",
        
    };

        public HealthyChecks(IConfiguration config, ILogger<HealthyChecks> logger, AppSettings appSettings, ISystemSecurity systemSecurity,ICommonRepo commonRepo)
        {
            _config = config;
            _logger = logger;
            _systemSecurity = systemSecurity;
            _appsettings = appSettings;
            _commonRepo = commonRepo;
        }

        public async Task CheckProceduresAsync()
        {
            string   _connString = _systemSecurity.GetConnectionString(_appsettings.DBType, _appsettings.DBServerName, _appsettings.DatabaseName, _appsettings.BRUserName, _appsettings.BRUserPassword, "BRGateway24API");            
            using var conn = new SqlConnection(_connString);
            conn.Open();

            foreach (var proc in _criticalProcedures)
            {
                using var cmd = new SqlCommand(@"
                SELECT COUNT(*) FROM sys.objects WHERE type = 'P' AND name = @spName", conn);
                cmd.Parameters.AddWithValue("@spName", proc);

                int count = (int)cmd.ExecuteScalar();
                if (count == 0)
                {
                    string msg = $"CRITICAL: Stored procedure '{proc}' is missing!";
                    _logger.LogCritical(msg);


                   
                        //Log then prevent app startup
                        await _commonRepo.LogRequestAsync(msg);                       
                        throw new Exception(msg);                   
                }                
            }
        }
    }
}