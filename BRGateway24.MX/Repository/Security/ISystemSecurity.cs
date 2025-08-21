using BRGateway24.Helpers;
using BRGateway24.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SwiftApi.Repository.Security
{
    public interface ISystemSecurity
    {        
        Task PassLogInformation(UserInfo usrInfo, string SessionID, string RequestName, string RequestParameter, string ResponseCode, string ResponseMessage);
        Task LogDetails(UserInfo usrInfo, string RequestName, string strParameters = "", string strResponse = "");
        Task LogExceptionDetails(UserInfo usrInfo, Exception ex, string RequestName = "");
        Task LogExceptionDetail(UserInfo usrInfo, Exception ex,string exString, string RequestName = "");       
        public Task<List<AuthenticateUser>> AuthenticateUser(UserInfo usrInfo, string _connectionString);
        public void UpdateLoginStatus(UserInfo usrInfo, char CSuccess, int IMessageID);
        public void Loginformation(UserInfo usrInfo, string SessionID, string RequestName, string RequestParameter, string ResponseCode, string ResponseMessage);

        public UserInfo GetUserInfo();
        public string GetConnectionString(string DBType, string DBServerName, string DatabaseName, string DBUserName, string DBUserPassword, string AppName = "");
        void LogError(Exception ex);
        public bool ValidRequest(HeaderRequest headerRequest, out Response headerResponse);
    }
}
