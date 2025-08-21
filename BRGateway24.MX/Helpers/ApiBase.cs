using Newtonsoft.Json;
using System.Data;

namespace BRGateway24.Helpers
{
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://www.craftsilicon.com/banking/core")]
    public class UserInfo
    {
        public const string UnknownIP = "<unknown>";
        private readonly static object lockobject = new object();

        public UserInfo()
            : this(String.Empty, String.Empty, String.Empty, String.Empty, false)
        {

        }
        public UserInfo(string strSystem)
            : this(String.Empty, String.Empty, strSystem, string.Empty, false)
        {

        }
        public UserInfo(string strUser, string strBranch, string strSystem)
            : this(strUser, strBranch, strSystem, String.Empty, false)
        {

        }

        public UserInfo(string strUser, string strBranch, string strSystem, string strBank, bool IsAdmin)
        {

            this.strUser = strUser;
            this.strBranch = strBranch;
            this.strBank = strBank;
            this.strSystem = strSystem;
            this.MachineIP = UnknownIP;
            this.dtLastAccessTime = DateTime.Now;
            this.IsAdmin = IsAdmin;

        }

        public UserInfo(string strUser, string strBranch, string strSystem, string strBank, bool IsAdmin, bool isService)
        {

            this.strUser = strUser;
            this.strBranch = strBranch;
            this.strBank = strBank;
            this.strSystem = strSystem;
            this.MachineIP = UnknownIP;
            this.dtLastAccessTime = DateTime.Now;
            this.IsAdmin = IsAdmin;
            this.isService = isService;

        }

        public string strUser;
        public string strBranch;
        public string strBranchName;
        public string strBank;
        public string strSystem;
        public string strLanguage;
        public string MachineIP;
        public ushort uAccessLevel;
        public DateTime dtLastAccessTime;
        public bool IsAdmin;
        public string DefaultURL;
        public string SessionID;
        public string connectionSource;
        public string CType;
        public string strCSClientID;
        public string strCSBankID;
        public string strBankName;
        public string strShortName;
        public string strDBServerName;
        public string strDatabaseName;
        public string strBRUserName;
        public string strBRUserPassword;
        public string strBRDBType;
        public bool isService;
        public Int64 LinkLogID;

        [NonSerialized]
        public bool bOkToUse;


        
    }

    public class APIBase
    {
        public static DataSet GenResponse(string msgcode, string Response, string Message)
        {
            DataSet dsResponsex = new DataSet();
            dsResponsex.Tables.Add("status");
            dsResponsex.Tables["status"].Columns.Add("Status");
            dsResponsex.Tables["status"].Columns.Add("Response");
            dsResponsex.Tables["status"].Columns.Add("Message");

            dsResponsex.Tables["status"].Rows.Add("999");
            dsResponsex.Tables["status"].Rows[0]["Status"] = msgcode;
            dsResponsex.Tables["status"].Rows[0]["Response"] = Response;
            dsResponsex.Tables["status"].Rows[0]["Message"] = Message;

            return dsResponsex;
        }

        public static string DsToJSON(DataSet ds)
        {
            string JSONString = string.Empty;
            JSONString = JsonConvert.SerializeObject(ds, Newtonsoft.Json.Formatting.Indented);
            return JSONString;
        }
    }


    
}
