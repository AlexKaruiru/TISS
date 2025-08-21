using Azure.Core;
using BRGateway24.DataAccess;
using BRGateway24.Helpers;
using BRGateway24.Models;
using ImagesStuff;
using iText.Layout.Element;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwiftApi.Repository.Security;
using System.Data;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Xml;
using Formatting = Newtonsoft.Json.Formatting;

namespace BRGateway24.Repository.AuditRepo
{
    public class AuditRepo : IAuditRepo
    {
        private readonly AppSettings _appSettings;
        public string _connString = string.Empty;
        private readonly ISystemSecurity _systemSecurity;
        static string imgHost =string.Empty;



        public AuditRepo(AppSettings appSettings, ISystemSecurity systemSecurity)
        {
            _appSettings = appSettings;
            _systemSecurity = systemSecurity;
            imgHost = _appSettings.ImageProxyURL;
        }

        public async Task<AuditResponseModel> GetEventsAsync(AuditRequestModel filter)
        {
            var mainResponse = new AuditResponseModel();

            try
            {
                // 1) Build connection string and open connection
                UserInfo userInfo = _appSettings.userInfo;
                _connString = _systemSecurity.GetConnectionString(
                    _appSettings.DBType,
                    _appSettings.DBServerName,
                    _appSettings.DatabaseName,
                    _appSettings.BRUserName,
                    _appSettings.BRUserPassword,
                    "BRGateway24API"
                );

                await using var sql = new SqlConnection(_connString);
                await sql.OpenAsync();

                // 2) Build the parameterized SQL text
                var sqlBuilder = new StringBuilder();
                sqlBuilder.AppendLine(@"
            SELECT
                event_time                AS EventTime,
                server_principal_name     AS ServerPrincipalName,
                database_name             AS DatabaseName,
                PARSENAME(object_name, 2) AS SchemaName,
                PARSENAME(object_name, 1) AS ObjectName,
                action_id                 AS ActionId,
                statement                 AS Statement
            FROM sys.fn_get_audit_file(
                   'C:\PerfLogs\BRAudit_AllDML_*.sqlaudit',  -- file pattern (always first)
                   @StartTimeParam,                          -- second: DATETIME or NULL
                   @EndTimeParam                             -- third: DATETIME or NULL
                 ) AS AuditData
            WHERE 1 = 1
              AND (
                    @ActionParam IS NULL
                    OR action_id = @ActionParam
                  )
              AND (
                    @TablesCsv IS NULL
                    OR object_name IN (
                          SELECT LTRIM(RTRIM(value))
                          FROM STRING_SPLIT(@TablesCsv, ',')
                       )
                  )
              AND (
                    @UserParam IS NULL
                    OR server_principal_name = @UserParam
                  )
            ORDER BY event_time ASC;
        ");

                string selectSql = sqlBuilder.ToString();

                // 3) Create SqlCommand with CommandType = Text
                await using var cmd = new SqlCommand(selectSql, sql)
                {
                    CommandType = CommandType.Text
                };

                // 4) Bind @StartTimeParam and @EndTimeParam
                cmd.Parameters.Add(new SqlParameter("@StartTimeParam", SqlDbType.DateTime)
                {
                    Value = filter.StartTime.HasValue
                                ? (object)filter.StartTime.Value
                                : DBNull.Value
                });
                cmd.Parameters.Add(new SqlParameter("@EndTimeParam", SqlDbType.DateTime)
                {
                    Value = filter.EndTime.HasValue
                                ? (object)filter.EndTime.Value
                                : DBNull.Value
                });

                // 5) Bind @ActionParam ("DL"/"UP") or NULL
                if (string.IsNullOrWhiteSpace(filter.Action))
                {
                    cmd.Parameters.Add(new SqlParameter("@ActionParam", SqlDbType.NVarChar, 2)
                    {
                        Value = DBNull.Value
                    });
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@ActionParam", SqlDbType.NVarChar, 2)
                    {
                        Value = filter.Action.Trim().ToUpperInvariant() // e.g. "DL" or "UP"
                    });
                }

                // 6) Bind @TablesCsv (comma‐delimited) or NULL
                if (filter.Tables == null || filter.Tables.Count == 0)
                {
                    cmd.Parameters.Add(new SqlParameter("@TablesCsv", SqlDbType.NVarChar, 4000)
                    {
                        Value = DBNull.Value
                    });
                }
                else
                {
                    if (filter.Tables[0] != null)
                    {
                        // Join into e.g. "dbo.t_transaction,Sales.DailySales"
                        string csv = string.Join(",", filter.Tables.Select(t => t.Trim()));
                        cmd.Parameters.Add(new SqlParameter("@TablesCsv", SqlDbType.NVarChar, 4000)
                        {
                            Value = csv
                        });
                    }
                }

                // 7) Bind @UserParam (server_principal_name) or NULL
                if (string.IsNullOrWhiteSpace(filter.User))
                {
                    cmd.Parameters.Add(new SqlParameter("@UserParam", SqlDbType.NVarChar, 128)
                    {
                        Value = DBNull.Value
                    });
                }
                else
                {
                    cmd.Parameters.Add(new SqlParameter("@UserParam", SqlDbType.NVarChar, 128)
                    {
                        Value = filter.User.Trim()
                    });
                }

                // 8) Execute reader and map results to AuditResponseModel
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    AuditResponseModel temp = null;
                    while (await reader.ReadAsync())
                    {
                        temp = reader.ConvertToObject<AuditResponseModel>();
                    }

                    if (temp != null)
                    {
                        mainResponse = temp;
                    }
                }

                return mainResponse;
            }
            catch (Exception)
            {
                // Optionally log the exception here
                return mainResponse;
            }
        }



    }
}
