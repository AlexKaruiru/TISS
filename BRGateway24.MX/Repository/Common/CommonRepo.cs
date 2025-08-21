using BRGateway24.DataAccess;
using BRGateway24.Helpers;
using BRGateway24.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using SwiftApi.Repository.Security;
using System.Data;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace BRGateway24.Repository.Common
{
    public class CommonRepo: ICommonRepo
    {
        private readonly AppSettings _appsettings;
        private string aUsername = string.Empty;
        private string aPassword = string.Empty;
        string strCode = string.Empty;
        string strStatus = string.Empty;
        string indented = string.Empty;
        public string _connString = string.Empty;
        private readonly ISystemSecurity _systemSecurity;
        string AuditPath = string.Empty;


        public CommonRepo(AppSettings appSettings, ISystemSecurity systemSecurity)
        {
            _appsettings = appSettings;
            aUsername = _appsettings.aUsername;
            aPassword = _appsettings.aPassword;
            _systemSecurity = systemSecurity;
            AuditPath = _appsettings.AuditLogPath;

        }

        public MainResponse InvokeResponse()
        {
            MainResponse mainResponse = new MainResponse();
            Response response = new Response();

            response = new Response()
            {
                Status = "999",
                Message = "An Error has Occured"
            };

            mainResponse = new MainResponse()
            {
                OurBranchID = "",
                resp = response
            };

            return mainResponse;
        }

        public MainResponse InvokeResponse(string msg)
        {
            MainResponse mainResponse = new MainResponse();
            Response response = new Response();

            response = new Response()
            {
                Status = "999",
                Message = msg
            };

            mainResponse = new MainResponse()
            {
                OurBranchID = "",
                resp = response
            };

            return mainResponse;
        }






        public MainResponse InvokeResponse(string msg, string code)
        {
            MainResponse mainResponse = new MainResponse();
            Response response = new Response();

            response = new Response()
            {
                Message = msg,
                Status = code

            };

            mainResponse = new MainResponse()
            {
                OurBranchID = "",
                resp = response
            };

            return mainResponse;
        }

        public MainResponse WaitingMessageResponse()
        {
            MainResponse mainResponse = new MainResponse();
            Response response = new Response();

            mainResponse = new MainResponse()
            {
                OurBranchID = "",
                resp = response
            };

            response = new Response()
            {
                Status = "00",
                Message = "Request Received and is being processed"

            };

            return mainResponse;
        }


        public async Task<AuditResp> APIAuditLogAsync(APIAuditLogRequest request)
        {
            try
            {
                await LogRequestAsync(request);
            }

            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message.ToString());
            }

            AuditResp mainresponse = new AuditResp();
            try
            {

                UserInfo userInfo = _appsettings.userInfo;
                _connString = _systemSecurity.GetConnectionString(_appsettings.DBType, _appsettings.DBServerName, _appsettings.DatabaseName, _appsettings.BRUserName, _appsettings.BRUserPassword, "BRGateway24API");


                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand(Constants.AUDITLOG, sql))
                    {
                        SqlParameter param;

                        cmd.CommandType = CommandType.StoredProcedure;
                        param = cmd.Parameters.Add("@MethodName", SqlDbType.NVarChar, 30);
                        param.Value = request.MethodName;
                        param = cmd.Parameters.Add("@Action", SqlDbType.VarChar, 30);
                        param.Value = request.Action;
                        param = cmd.Parameters.Add("@AuditID", SqlDbType.VarChar, 30);
                        param.Value = request.AuditID;
                        param = cmd.Parameters.Add("@DetailRecords", SqlDbType.Xml);
                        param.Value = request.DetailRecords;
                        param = cmd.Parameters.Add("@Request", SqlDbType.VarChar, 8000);
                        param.Value = request.Request;
                        param = cmd.Parameters.Add("@Response", SqlDbType.VarChar,8000);
                        param.Value = request.Response;
                        param = cmd.Parameters.Add("@OperatorID", SqlDbType.NVarChar, 20);
                        param.Value = userInfo.strUser;



                        Response resp = new Response();
                        InvokeResponse invokeResponse = new InvokeResponse();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            //Log has been Saved
                        }


                    }
                }
                return mainresponse;
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex.Message.ToString());
                return mainresponse;
            }
        }

        public async Task LogRequestAsync(object request)
        {
            const string specialMethod = "Validate Request";

            // 1) Validate inputs
            string baseLogFolder = AuditPath?.ToString();
            if (string.IsNullOrWhiteSpace(baseLogFolder))
                throw new ArgumentException("Base log folder cannot be empty.", nameof(baseLogFolder));
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // 2) Reflect out MethodName, Request, Response
            var type = request.GetType();
            string methodName = type.GetProperty("MethodName")?
                                    .GetValue(request)?
                                    .ToString() ?? "";
            var rawReq = type.GetProperty("Request")?.GetValue(request);
            var rawResp = type.GetProperty("Response")?.GetValue(request);

            // 3) Parse JSON‐strings into objects for nested JSON
            object parsedReq = rawReq is string rj1 ? JsonConvert.DeserializeObject(rj1) : rawReq;
            object parsedResp = rawResp is string rj2 ? JsonConvert.DeserializeObject(rj2) : rawResp;

            // 4) If Response.resp.OutputJSON is still a JSON‐string, parse it too
            if (parsedResp is JObject respJObj
                && respJObj.TryGetValue("resp", out JToken respSection)
                && respSection is JObject respObj
                && respObj.TryGetValue("OutputJSON", out JToken outJsonToken)
                && outJsonToken.Type == JTokenType.String)
            {
                respObj["OutputJSON"] = JToken.Parse(outJsonToken.ToString());
            }

            // 5) Prepare today's folder
            string todayFolder = Path.Combine(baseLogFolder, DateTime.Now.ToString("yyyy-MM-dd"));
            Directory.CreateDirectory(todayFolder);

            // 6) Once‐per‐day marker for the special method
            if (string.Equals(methodName, specialMethod, StringComparison.Ordinal))
            {
                string markerFile = Path.Combine(todayFolder, "ValidateRequest.logged");
                if (File.Exists(markerFile))
                    return;
                File.WriteAllText(markerFile, DateTime.Now.ToString("o"));
            }

            // 7) Build the JSON payload
            var logPayload = new
            {
                Timestamp = DateTime.Now.ToString("o"),
                MethodName = methodName,
                Request = parsedReq,
                Response = parsedResp
            };
            string json = JsonConvert.SerializeObject(logPayload, Formatting.Indented);

            // 8) Build START/END markers
            string ts1 = DateTime.Now.ToString("M-d-yyyy h:mm:ss tt");
            string ts2 = DateTime.Now.ToString("M/d/yyyy h:mm:ss tt");
            string startLine = $">>>REQUEST_START>>>";
            string endLine = $">>>REQUEST_END>>>";
            string combinedLine = $"{startLine} {ts2}    {endLine} {ts2}";

            // 9) Write everything out
            string logFile = Path.Combine(todayFolder, "Gateway24_APIAudit.txt");
            var sb = new StringBuilder()
                .AppendLine(new string('=', 80))
                .AppendLine(combinedLine)
                .AppendLine(json)
                .AppendLine(new string('=', 80))
                .AppendLine();

            await File.AppendAllTextAsync(logFile, sb.ToString());
        }





        public async Task<MainResponse> GetCodesAsync(CodeDetails codeDetails)
        {
            string Type = codeDetails.ID; //default to same

            MainResponse mainresponse = new MainResponse();
            try
            {
                Match sysMatch = Regex.Match(codeDetails.ID, "System");
                Match userMatch = Regex.Match(codeDetails.ID, "User");
                Match staticMatch = Regex.Match(codeDetails.ID, "Static");
                Match countryMatch = Regex.Match(codeDetails.ID, "Country");
                Match cityMatch = Regex.Match(codeDetails.ID, "City");
                Match employerMatch = Regex.Match(codeDetails.ID, "Employer");



                if (sysMatch.Success)
                {
                    Type = sysMatch.Groups[0].Value;
                    int index = Type.Length;
                    codeDetails.ID = codeDetails.ID.Substring(index);
                }

                if (userMatch.Success)
                {
                    Type = userMatch.Groups[0].Value;
                    int index = Type.Length;
                    codeDetails.ID = codeDetails.ID.Substring(index);
                }

                if (staticMatch.Success)
                {
                    Type = staticMatch.Groups[0].Value;
                    int index = Type.Length;
                    codeDetails.ID = codeDetails.ID.Substring(index);
                }

                if (countryMatch.Success)
                {
                    Type = countryMatch.Groups[0].Value;
                    int index = Type.Length;
                    codeDetails.ID = codeDetails.ID.Substring(index);
                }

                if (cityMatch.Success)
                {
                    Type = cityMatch.Groups[0].Value;
                    int index = Type.Length;
                    codeDetails.ID = codeDetails.ID.Substring(index);
                }

                if (employerMatch.Success)
                {
                    Type = employerMatch.Groups[0].Value;
                    int index = Type.Length;
                    codeDetails.ID = codeDetails.ID.Substring(index);
                }

                var clientSrch = new List<CodeDetails>
                {
                    codeDetails
                };

                string strRequest = JsonConvert.SerializeObject(clientSrch);

                ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                UserInfo userInfo = _appsettings.userInfo;
                _connString = _systemSecurity.GetConnectionString(_appsettings.DBType, _appsettings.DBServerName, _appsettings.DatabaseName, _appsettings.BRUserName, _appsettings.BRUserPassword, "BRGateway24API");
                DateTime currentDateTime = DateTime.Now;

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand(Constants.GETCODES, sql))
                    {
                        SqlParameter param;

                        cmd.CommandType = CommandType.StoredProcedure;
                        param = cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 100);
                        param.Value = codeDetails.ID;
                        param = cmd.Parameters.Add("@Type", SqlDbType.NVarChar,50);
                        param.Value = Type;
                        param = cmd.Parameters.Add("@OperatorID", SqlDbType.NVarChar, 20);
                        param.Value = userInfo.strUser;



                        Response res = new Response();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                res = reader.ConvertToObject<Response>();
                            }

                            if (res.Status == "000" || res.Status == "999")
                            {
                                if (await reader.NextResultAsync())
                                {
                                    while (await reader.ReadAsync())
                                    {
                                        mainresponse = reader.ConvertToObject<MainResponse>();
                                    }
                                }
                            }

                            mainresponse = new MainResponse()
                            {
                                resp = res,
                                OurBranchID = ""
                            };
                        }


                    }
                }
                return mainresponse;
            }
            catch (Exception ex)
            {
                return mainresponse;
            }
        }


        private class OrderedContractResolver : DefaultContractResolver
        {
            protected override System.Collections.Generic.IList<JsonProperty> CreateProperties(System.Type type, MemberSerialization memberSerialization)
            {
                return base.CreateProperties(type, memberSerialization).OrderBy(p => p.PropertyName).ToList();
            }
        }

    }
}
