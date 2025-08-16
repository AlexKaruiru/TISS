using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BRGateway24.Helpers
{
    public class AppSettings
    {
        public string DBType { get; set; }
        public string DBServerName { get; set; }
        public string DatabaseName { get; set; }
        public string BRUserName { get; set; }
        public string BRUserPassword { get; set; }
        public string UserKey { get; set; }
        public string DefaultBranchID { get; set; }
        public string ErrorLogPath { get; set; }
        public string AuditLogPath { get; set; }
        public string SQLLogPath { get; set; }
        public UserInfo userInfo { get; set; }
        public string host { get; set; }
        public string crbHost { get; set; }
        public string aUsername { get; set; }
        public string aPassword { get; set; }

        public string cUsername { get; set; }
        public string cPassword { get; set; }
        public string cBranchCode { get; set; } 
        public string elmaHost { get; set; }
        public int Expiry { get; set; }
        public string CountryCode { get; set; }
        public string FromEmail { get; set; }
        public string FromPass { get; set; }
        public string SMTP { get; set; }
        public string SMTPPort { get; set; }
        public string EnableSSL { get; set; }
        public string EmailMessage { get; set; }
        public string RequestEmailMessage { get; set; }
        public string EmailFromDisplayName { get; set; }
        public string ImageProxyURL { get; set; }
        public string EDMSEndpoint { get; set; }
        public string TopazBaseRoute { get; set; }
        public string EDMSFilePath    { get;set; }
        public string edmsBackupFilePath { get; set;}
    }
}
