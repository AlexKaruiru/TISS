using BRGateway24.DataAccess;
using BRGateway24.Helpers;
using BRGateway24.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using SwiftApi.Repository.Security;
using System.Data;
using BRGateway24.Repository.ESS.Interfaces;

namespace BRGateway24.Repository.Loans
{
    public class ESSLoansRepo : IESSLoansRepo
    {
        private readonly AppSettings _appSettings;
        public string _connString = string.Empty;
        private readonly ISystemSecurity _systemSecurity;
        public ESSLoansRepo(AppSettings appSettings, ISystemSecurity systemSecurity)
        {
            _appSettings = appSettings;
            _systemSecurity = systemSecurity;
        }
        public async Task<MainResponse> ESSLoanApplicationAsync(string ESSRequest)
        {
            MainResponse mainResponse = new MainResponse();
            try
            {
                UserInfo userInfo = _appSettings.userInfo;
                _connString = _systemSecurity.GetConnectionString(_appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName, _appSettings.BRUserName, _appSettings.BRUserPassword, "BRGateway24API");
                DateTime currentDateTime = DateTime.Now;
                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand(Constants.ESSLOAN_CHARGES_REQUEST, sql))
                    {
                        SqlParameter param;
                        cmd.CommandType = CommandType.StoredProcedure;
                        param = cmd.Parameters.Add("@OurBranchID", SqlDbType.NVarChar, 15);
                        param.Value = userInfo.strBranch;
                        param = cmd.Parameters.Add("@DataJSON", SqlDbType.NVarChar, -1);
                        param.Value = ESSRequest;
                        param = cmd.Parameters.Add("@Datetime", SqlDbType.DateTime);
                        param.Value = currentDateTime;
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
                            reader.NextResult();

                            if (res.Status == "00" || res.Status == "99")
                            {
                                while (await reader.ReadAsync())
                                {
                                    mainResponse = reader.ConvertToObject<MainResponse>();
                                }

                            }
                            mainResponse = new MainResponse()
                            {
                                resp = res,
                                OurBranchID = "",

                            };
                        }
                    }
                }
                return mainResponse;
            }
            catch (Exception ex)
            {
                return mainResponse;
            }
        }

    }
}

