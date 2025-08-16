using BRGateway24.DataAccess;
using BRGateway24.Helpers;
using BRGateway24.Models;
using BRGateway24.Repository;
using SeCy;
using Serilog.Context;
using System.Collections;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;

namespace SwiftApi.Repository.Security
{
    public class SystemSecurity : ISystemSecurity
    {        
        readonly AppSettings _appSettings;        
        public string _connString = string.Empty;
        public static string _dbServerName = string.Empty;
        public static string _databaseName = string.Empty;
        public static string _brUserName = string.Empty;
        public static string _brUserPassword = string.Empty;
        public static string _brDBType = string.Empty;
        public static string _defaultBranch = string.Empty;
        public static string _errorLogPath = string.Empty;
        public static UserInfo _userInfo = new UserInfo();
        public static string _usrKey = string.Empty;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SystemSecurity( AppSettings appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appSettings;
            _dbServerName = _appSettings.DBServerName;
            _databaseName = _appSettings.DatabaseName;
            _brUserName = _appSettings.BRUserName;
            _brUserPassword = _appSettings.BRUserPassword;
            _defaultBranch = _appSettings.DefaultBranchID;
            _errorLogPath = _appSettings.ErrorLogPath;
            _usrKey = _appSettings.UserKey;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task PassLogInformation(UserInfo usrInfo, string SessionID, string RequestName, string RequestParameter, string ResponseCode, string ResponseMessage)
        {
            ResponseMessage = ResponseMessage.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            RequestParameter = RequestParameter.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            string strResponse = string.Format("<BRDataSet><dt_Response>{0}</dt_Response><UserHost>{1}</UserHost></BRDataSet>", ResponseMessage, Convert.ToString(usrInfo.MachineIP));
            try
            {
                //_authCtxt.Loginformation(usrInfo, SessionID, RequestName, RequestParameter, ResponseCode, strResponse);

            }
            catch (Exception ex)
            {
                //await LogDetails(usrInfo, RequestName, string.Format("AuditLogInformation Parameter Error: OurBranchID: {0} \n MessageID: {1} \n RequestName:{2} \n RequestParameter:{3} \n Responsecode:{4} \n ResponseMessage:{5} \n OperatorID:{6} \n", usrInfo.strBranch, SessionID, RequestName, RequestParameter, ResponseCode, strResponse, usrInfo.strUser), strResponse);

                await LogExceptionDetails(usrInfo, ex, RequestName);
            }
        }
        public async Task LogExceptionDetail(UserInfo usrInfo, Exception ex, string exString, string RequestName = "")
        {
            try
            {
                string ErrorLogPath = Convert.ToString(_appSettings.ErrorLogPath);
                if (!string.IsNullOrEmpty(ErrorLogPath))
                {
                    ErrorLogPath = AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog";
                }
                usrInfo.strUser = string.IsNullOrEmpty(usrInfo.strUser) ? "Gateway24Api" : usrInfo.strUser;
                string AppendErrorMessage = string.Empty;

                if (ex != null)
                {
                    Dictionary<object, object> exDic = ex.Data.Cast<DictionaryEntry>().Where(de => de.Key is object && de.Value is object).ToDictionary(de => (object)de.Key,de => (object)de.Value);

                    AppendErrorMessage = "User ID : " + usrInfo.strUser + Environment.NewLine + "Request Name: " + RequestName + Environment.NewLine +
                         "Date" + ":" + DateTime.Now + Environment.NewLine + "Error Message : " + ex.ToString() + Environment.NewLine + "Error Data : " + string.Join(";", exDic.Select(x => x.Key + "=" + x.Value).ToArray()) +
                         Environment.NewLine + "==================================================" + Environment.NewLine + exString;                    

                }
                else
                {

                    AppendErrorMessage = "User ID : " + usrInfo.strUser + Environment.NewLine + "Request Name: " + exString;                    
                }
                string strFileName = string.Concat(ErrorLogPath, "\\", usrInfo.strSystem, "\\");
                System.IO.Directory.CreateDirectory(strFileName);
                System.IO.File.AppendAllText(strFileName + usrInfo.strUser + DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "") + ".txt", AppendErrorMessage);

            }
            catch
            {
                throw new Exception("System Exception Logging Error");
            }
        }
        public async Task LogExceptionDetails(UserInfo usrInfo, Exception ex, string RequestName = "")
        {
            try
            {
                string ErrorLogPath = Convert.ToString(_appSettings.ErrorLogPath);
                if (!string.IsNullOrEmpty(ErrorLogPath))
                {
                    ErrorLogPath = AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog";
                }
                usrInfo.strUser = string.IsNullOrEmpty(usrInfo.strUser) ? "Gateway24Api" : usrInfo.strUser;
                Dictionary<object, object> exDic = ex.Data.Cast<DictionaryEntry>()
                    .Where(de => de.Key is object && de.Value is object)
                         .ToDictionary(de => (object)de.Key,
                                       de => (object)de.Value);
                string AppendErrorMessage = "User ID : " + usrInfo.strUser + Environment.NewLine + "Request Name: " + RequestName + Environment.NewLine +
                     "Date" + ":" + DateTime.Now + Environment.NewLine + "Error Message : " + ex.ToString() + Environment.NewLine + "Error Data : " + string.Join(";", exDic.Select(x => x.Key + "=" + x.Value).ToArray()) +
                     Environment.NewLine + "==================================================" + Environment.NewLine;
                string strFileName = string.Concat(ErrorLogPath, "\\", usrInfo.strSystem, "\\");
                System.IO.Directory.CreateDirectory(strFileName);
                System.IO.File.AppendAllText(strFileName + usrInfo.strUser + DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "") + ".txt", AppendErrorMessage);

            }
            catch
            {
                throw new Exception("System Exception Logging Error");
            }
        }
        public async Task LogDetails(UserInfo usrInfo, string RequestName, string strParameters = "", string strResponse = "")
        {
            try
            {
                string AuditLogPath = Convert.ToString(_appSettings.AuditLogPath);
                if (!string.IsNullOrEmpty(AuditLogPath))
                {
                    AuditLogPath = AppDomain.CurrentDomain.BaseDirectory + "\\AuditLog";
                }
                usrInfo.strUser = string.IsNullOrEmpty(usrInfo.strUser) ? "Gateway24Api" : usrInfo.strUser;

                string AppendErrorMessage = "User ID" + ":" + usrInfo.strUser + Environment.NewLine + "Date: " + DateTime.Now + Environment.NewLine +
                   "Request Name: " + RequestName + Environment.NewLine + "Parameters: " + strParameters + Environment.NewLine + "Response: " + strResponse +
                   Environment.NewLine + "==================================================" + Environment.NewLine;
                string strFileName = string.Concat(AuditLogPath, "\\", usrInfo.strSystem, "\\");
                System.IO.Directory.CreateDirectory(strFileName);
                System.IO.File.AppendAllText(strFileName + usrInfo.strUser + DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "") + ".txt", AppendErrorMessage);

            }
            catch
            {
                throw new Exception("System Exception Details Logging Error");
            }
        }

     
        public async Task<List<AuthenticateUser>> AuthenticateUser(UserInfo usrInfo, string _connectionString)
        {
            _connString = _connectionString;
            try
            {
                //ApiUtils _apiUtils = new ApiUtils(_appSettings);
                //ApiUtils _apiUtils = new ApiUtils(_appSettings);                
                GetConnectionString(_appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName, _appSettings.BRUserName, _appSettings.BRUserPassword, "Gateway24Api");

                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(Constants.BRAUTHENTICATEUSER, sql))
                    {
                        SqlParameter param;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        param = cmd.Parameters.Add("@OurBranchID", SqlDbType.NVarChar, 15);
                        param.Value = usrInfo.strBranch;
                        param = cmd.Parameters.Add("@OperatorID", SqlDbType.NVarChar, 15);
                        param.Value = usrInfo.strUser;
                        param = cmd.Parameters.Add("@IPAddress", SqlDbType.NVarChar, 15);
                        param.Value = usrInfo.MachineIP.Split('|')[0];
                        param = cmd.Parameters.Add("@MacAddress", SqlDbType.NVarChar, 15);
                        param.Value = usrInfo.MachineIP.Split('|')[1];


                        List<AuthenticateUser> response = new List<AuthenticateUser>();
                        await sql.OpenAsync();

                        

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                                var rowValues = Enumerable.Range(0, reader.FieldCount)
        .Select(i => $"{reader.GetName(i)}={(reader.IsDBNull(i) ? "NULL" : reader.GetValue(i))}")
        .ToList();

                                string rowString = string.Join(", ", rowValues);

                                Serilog.Log.Debug("Row Data: {RowData}", rowString);

                                response.Add(MapToValue(reader));

                               
                            }

                            // ✅ Check AFTER reading is complete
                            if (!response.Any())
                            {
                                Serilog.Log.Warning("AuthenticateUser list is null or empty.");
                            }
                            else
                            {
                                string responseString = System.Text.Json.JsonSerializer.Serialize(
                                    response,
                                    new System.Text.Json.JsonSerializerOptions { WriteIndented = true }
                                );

                                Serilog.Log.Information("AuthenticateUser list: {Response}", responseString);
                            }
                        }

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }

        private AuthenticateUser MapToValue(SqlDataReader reader)
        {
            AuthenticateUser user = null;
            try
            {
                user = new AuthenticateUser()
                {
                    LanguageID = (string)reader["LanguageID"],
                    Password = (string)reader["Password"],
                    TimeLoggedIn = (System.DateTime?)reader["TimeLoggedIn"],
                    IPAddress = (string)reader["IPAddress"],
                    RoleID = (string)reader["RoleID"],
                    Status = (short)reader["Status"],
                    SessionID = (string)reader["SessionID"],
                    UserID = (string)reader["UserID"],
                    Username = (string)reader["Username"],
                    Firstname = (string)reader["Firstname"],
                    Lastname = (string)reader["Lastname"],
                    Email = (string)reader["Email"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    ProfilePic = (string)reader["ProfilePic"]

                };
            }
            catch (Exception ex)
            {
                user = null;
            }

            return user;
        }


        public async void UpdateLoginStatus(UserInfo usrInfo, char CSuccess, int IMessageID)
        {
            try
            {
                //ApiUtils _apiUtils = new ApiUtils(_appSettings);
                //ApiUtils _apiUtils = new ApiUtils(_appSettings);
                GetUserInfo();
                GetConnectionString(_appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName, _appSettings.BRUserName, _appSettings.BRUserPassword, "Gateway24Api");

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand(Constants.BRUPDATELOGINSTATUS, sql))
                    {
                        SqlParameter param;

                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        param = cmd.Parameters.Add("@OurBranchID", SqlDbType.NVarChar, 15);
                        param.Value = usrInfo.strBranch;
                        param = cmd.Parameters.Add("@OperatorID", SqlDbType.NVarChar, 15);
                        param.Value = usrInfo.strUser;
                        param = cmd.Parameters.Add("@Success", SqlDbType.Char, 1);
                        param.Value = CSuccess;
                        param = cmd.Parameters.Add("@IPAddress", SqlDbType.NVarChar, 15);
                        param.Value = usrInfo.MachineIP.Split('|')[0];
                        param = cmd.Parameters.Add("@MessageID", SqlDbType.Int);
                        param.Value = IMessageID;
                        param = cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar, 100);
                        param.Value = string.Empty;


                        List<AuthenticateUser> response = new List<AuthenticateUser>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {

                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                
            }
            
        }

        public async void Loginformation(UserInfo usrInfo, string SessionID, string RequestName, string RequestParameter, string ResponseCode, string ResponseMessage)
        {
            //ApiUtils apiUtils = new ApiUtils(_appSettings);
            //ApiUtils _apiUtils = new ApiUtils(_appSettings);
            GetUserInfo();
            GetConnectionString(_appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName, _appSettings.BRUserName, _appSettings.BRUserPassword, "Gateway24Api");

            using (SqlConnection sql = new SqlConnection(_connString))
            {
                using (SqlCommand cmd = new SqlCommand(Constants.BRAUDITLOG, sql))
                {
                    SqlParameter param;

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    param = cmd.Parameters.Add("@OurBranchID", SqlDbType.NVarChar, 15);
                    param.Value = usrInfo.strBranch;
                    param = cmd.Parameters.Add("@SessionID", SqlDbType.NVarChar, 50);
                    param.Value = SessionID;
                    param = cmd.Parameters.Add("@AccountID", SqlDbType.NVarChar, 25);
                    param.Value = RequestParameter;
                    param = cmd.Parameters.Add("@MobileNumber", SqlDbType.NVarChar, 15);
                    param.Value = ResponseCode;
                    param = cmd.Parameters.Add("@RequestName", SqlDbType.NVarChar, 15);
                    param.Value = RequestName;
                    param = cmd.Parameters.Add("@RequestParameter", SqlDbType.Xml);
                    param.Value = RequestParameter;
                    param = cmd.Parameters.Add("@ResponseCode", SqlDbType.NVarChar, 1000);
                    param.Value = ResponseCode;
                    param = cmd.Parameters.Add("@ResponseMessage", SqlDbType.NVarChar, 1000);
                    param.Value = ResponseMessage;
                    param = cmd.Parameters.Add("@@OperatorID", SqlDbType.NVarChar, 100);
                    param.Value = usrInfo.strUser;


                    List<AuthenticateUser> response = new List<AuthenticateUser>();
                    await sql.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {

                        }
                    }

                }
            }
        }

        public string GetConnectionString(string DBType, string DBServerName, string DatabaseName, string DBUserName, string DBUserPassword, string AppName = "")
        {
            string dbPort = DBServerName.Substring(DBServerName.Contains(',') ? DBServerName.IndexOf(',') : 0);
            DBUserPassword = DBClient.BRUserPassword(DBUserPassword);
            DBUserName = DBClient.BRUserName(DBUserName);
            AppName = string.IsNullOrEmpty(AppName) ? "BRGateway24API" : AppName;
            string strConnection;
            switch (DBType)
            {
                case "SQLSERVER":
                    strConnection = string.Format("Data source={0};Initial Catalog={1};User id={2};Password={3};Integrated Security=false;persist security info=True;App={4};MultipleActiveResultSets=false;TrustServerCertificate=True;",
                        DBServerName, DatabaseName, DBUserName, DBUserPassword, AppName);
                    break;
                case "MYSQL":
                    dbPort = string.IsNullOrEmpty(dbPort) ? "3306" : dbPort;
                    strConnection = string.Format("Server={0};Database={1};Uid={2};pwd={3};port={4};",
                        DBServerName, DatabaseName, DBUserName, DBUserPassword, dbPort);
                    break;
                case "ORACLE":
                    strConnection = string.Format("Data Source={0};user ID={1};password={2};",
                       DatabaseName, DBUserName, DBUserPassword);
                    break;
                default:
                    strConnection = string.Format("Data source={0};Initial Catalog={1};User id={2};Password={3};persist security info=True;App={4}",
                        DBServerName, DatabaseName, DBUserName, DBUserPassword, AppName);
                    break;
            }


            return strConnection;
        }


        //public UserInfo GetUserInfo()
        //{
        //    return new UserInfo
        //    {
        //        strBRDBType = _brDBType,
        //        strBRUserName = _brUserName,
        //        strBRUserPassword = _brUserPassword,
        //        strDatabaseName = _databaseName,
        //        strDBServerName = _dbServerName,
        //        strBranch = _defaultBranch
        //    };
        //}
        public void LogError(Exception ex)
        {
            StackTrace stackTrace = new StackTrace();
            string MethodName = stackTrace.GetFrame(1).GetMethod().Name;
            ParameterInfo[] info = stackTrace.GetFrame(1).GetMethod().GetParameters();
            DataSet dsResponse = new DataSet();

            object SessionID = null;

            foreach (ParameterInfo inf in info)
            {
                if (inf.Name.ToLower().Contains("sessionid"))
                {
                    SessionID = inf.DefaultValue;
                    break;
                }

            }

            if (_errorLogPath.ToString() != "")
            {
                string strFileName = $"{_errorLogPath}API";
                System.IO.Directory.CreateDirectory(strFileName);

                string AppendErrorMessage = Environment.NewLine + "Error Message" + ":" + ex.Message
                    + Environment.NewLine + ex.StackTrace
                     + Environment.NewLine + ex.TargetSite
                      + Environment.NewLine + ex.Source +
                    "Date" + ":" + DateTime.Now + Environment.NewLine + "--------------------------" + Environment.NewLine;
                System.IO.File.AppendAllText(strFileName + "_Error_" + DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "") + ".txt", AppendErrorMessage);
            }

        }


        public bool ValidRequest(HeaderRequest headerRequest, out Response headerResponse)
        {
            string userName = string.Empty;
            string msgCode = string.Empty;
            headerResponse = new Response();

            if (string.IsNullOrEmpty(headerRequest.ConsumerKey) || string.IsNullOrEmpty(headerRequest.ConsumerSecret))
            {
                headerResponse.Status = "400";
                headerResponse.Message = "The parameters are not valid or they are missing.";
                return false;
            }

            if (!ValidConnection(headerRequest.ConsumerKey, headerRequest.ConsumerSecret, "", out msgCode, out userName, out string resJSON))
            {
                headerResponse.Status = string.IsNullOrEmpty(msgCode) ? "401" : msgCode;
                headerResponse.Message = "UnAuthorized Access.";
                return false;
            }

            _userInfo.strUser = userName;
            _userInfo.strBranch = _appSettings.DefaultBranchID;
            _appSettings.userInfo = _userInfo;

            headerResponse.Status = "000";
            headerResponse.Message = "Authorized Access.";
            headerResponse.OutputJSON = resJSON;

            return true;
        }

        private bool ValidConnection(string consumerKey, string consumerSecret, string strUniqueID, out string msgCode, out string userName, out string resJSON)
        {
            resJSON = string.Empty;
            AuthenticateUser authenticateUser = null;
            msgCode = string.Empty;
            userName = string.Empty;
            if (string.IsNullOrEmpty(string.Concat(Convert.ToString(consumerKey), Convert.ToString(consumerSecret), Convert.ToString(_usrKey))))
            {
                msgCode = "400";
                return false;
            }
            try
            {

                consumerKey = EnAction128.Funua(consumerKey, _usrKey);
                consumerSecret = EnAction128.Funua(consumerSecret, _usrKey);

                if (CheckUser(consumerKey, consumerSecret, strUniqueID, ref authenticateUser,out resJSON))
                {
                    userName = consumerKey;
                    LogContext.PushProperty("Username", userName); //Add the userName to LogContext;  
                    return true;
                }
                else
                {
                    msgCode = "401";
                }
                return false;
            }
            catch (Exception exp)
            {
                Serilog.Log.Error(exp, "ValidConnection ", "ValidConnection", _userInfo.strUser, _userInfo.MachineIP);
                msgCode = "405";
                return false;
            }
        }

        private bool CheckUser(string username, string password, string strUniqueID, ref AuthenticateUser authenticateUser, out string resJSON)
        {
            resJSON = string.Empty;
            try
            {
                username = username.ToUpper();
                var context = _httpContextAccessor.HttpContext;
                string IPAddress = string.Empty;

                if (context != null)
                {
                    IPAddress = Convert.ToString(context.Connection.RemoteIpAddress);
                }
                
                UserInfo usrInfo = GetUserInfo();
                usrInfo.MachineIP = IPAddress + "|";
                string EncryptPassword = string.Empty;
                string strLanguageID = string.Empty;
                ushort uAccessLevel;
                string RoleID = string.Empty;
                usrInfo.strUser = username;
                EncryptPassword = EncryptText(CreateEncryptData(usrInfo, username, password));
                bool ValidUser = false;


                AuthenticateUser AppUser = AuthenticateUser(usrInfo, EncryptPassword, strUniqueID).Result;
                resJSON = AppUser.OutputJSON;
                //_sysSecurity.LogExceptionDetail(new UserInfo { strUser = "API" }, null, ApiUtils.ConvertObjectToXML(AppUser));

                if (AppUser != null)
                {
                    if (AppUser.Status == 1)
                        ValidUser = true;
                }

                authenticateUser = AppUser;
                return ValidUser;
            }
            catch (Exception exUsr)
            {
                LogExceptionDetails(new UserInfo { strUser = "BRGateway24API" }, exUsr, "CheckUser");

                return false;
            }
        }

        public UserInfo GetUserInfo()
        {
            return new UserInfo
            {
                strBRDBType = _brDBType,
                strBRUserName = _brUserName,
                strBRUserPassword = _brUserPassword,
                strDatabaseName = _databaseName,
                strDBServerName = _dbServerName,
                strBranch = _defaultBranch
            };
        }

        static public string EncryptText(string strInputText)
        {
            byte[] data = Array.ConvertAll<char, byte>(strInputText.ToCharArray(), delegate (char ch) { return (byte)ch; });
            SHA256 shaM = new SHA256Managed();
            byte[] result = shaM.ComputeHash(data);
            return Convert.ToBase64String(result);
        }
        public static string CreateEncryptData(UserInfo usrInfo, string strOperatorID, string strPassword)
        {
            return String.Format("{0}{1}", strOperatorID, strPassword);
        }

        public async Task<AuthenticateUser> AuthenticateUser(UserInfo usrInfo, string strEncryptedPassword, string UniqueID)
        {
            AuthenticateUser authUser = null;

            string ConnectionString = GetConnectionString(_appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName, _appSettings.BRUserName, _appSettings.BRUserPassword);


            try
            {
                List<AuthenticateUser> lsAuthUser = await AuthenticateUser(usrInfo, ConnectionString);

                if (lsAuthUser?.Count > 0)
                {
                    authUser = lsAuthUser.FirstOrDefault();

                    if (authUser.Password.Equals(strEncryptedPassword))
                    {
                        Serilog.Log.Debug("PASSWORD MATCHED");
                        if (authUser.Status.Equals("1") || authUser.Status == 1)
                        {
                            UpdateLoginStatus(usrInfo, 'S', 1017);
                        }
                        return authUser;
                    }
                    else
                    {
                        Serilog.Log.Debug("PASSWORD NOT MATCHED");

                        if (authUser.Status.Equals("3") || authUser.Status == 3)
                        {
                            UpdateLoginStatus(usrInfo, 'S', 1017);
                            authUser.Status = 1;
                            return authUser;
                        }

                        if (authUser.Status.Equals("268") || authUser.Status == 268)
                        {
                            UpdateLoginStatus(usrInfo, 'S', 000268);
                            return authUser;
                        }
                        else
                        {
                            UpdateLoginStatus(usrInfo, 'F', 000283);
                        }
                    }
                }
                return null; // authUser;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public string GetConnectionString(string DBType, string DBServerName, string DatabaseName, string DBUserName, string DBUserPassword, string AppName = "")
        //{
        //    string dbPort = DBServerName.Substring(DBServerName.Contains(',') ? DBServerName.IndexOf(',') : 0);
        //    DBUserPassword = DBClient.BRUserPassword(DBUserPassword);
        //    DBUserName = DBClient.BRUserName(DBUserName);
        //    AppName = string.IsNullOrEmpty(AppName) ? "API" : AppName;
        //    string strConnection;
        //    switch (DBType)
        //    {
        //        case "SQLSERVER":
        //            strConnection = string.Format("Data source={0};Initial Catalog={1};User id={2};Password={3};Integrated Security=false;persist security info=True;App={4};MultipleActiveResultSets=false;",
        //                DBServerName, DatabaseName, DBUserName, DBUserPassword, AppName);
        //            break;
        //        case "MYSQL":
        //            dbPort = string.IsNullOrEmpty(dbPort) ? "3306" : dbPort;
        //            strConnection = string.Format("Server={0};Database={1};Uid={2};pwd={3};port={4};",
        //                DBServerName, DatabaseName, DBUserName, DBUserPassword, dbPort);
        //            break;
        //        case "ORACLE":
        //            strConnection = string.Format("Data Source={0};user ID={1};password={2};",
        //               DatabaseName, DBUserName, DBUserPassword);
        //            break;
        //        default:
        //            strConnection = string.Format("Data source={0};Initial Catalog={1};User id={2};Password={3};persist security info=True;App={4}",
        //                DBServerName, DatabaseName, DBUserName, DBUserPassword, AppName);
        //            break;
        //    }


        //    return strConnection;
        //}

    }
}
