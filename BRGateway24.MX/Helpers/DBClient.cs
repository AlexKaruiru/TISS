using BRNetSecurity;

namespace BRGateway24.Repository
{
    public static class DBClient
    {
        public static string BRUserName(string strUserName)
        {
            return BRAccess.BRUserName(strUserName);
        }
        public static string BRUserPassword(string strUserPassword)
        {
            return BRAccess.BRUserPassword(strUserPassword);
        }
    }
}
