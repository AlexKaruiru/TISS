using Azure.Core;
using BRGateway24.DataAccess;
using BRGateway24.Helpers;
using BRGateway24.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SwiftApi.Repository.Security;
using System.Data;

namespace BRGateway24.Repository.ProductRepo
{
    public class ProductRepo : IProductRepo
    {
        private readonly AppSettings _appSettings;
        public string _connString = string.Empty;
        private readonly ISystemSecurity _systemSecurity;



        public ProductRepo(AppSettings appSettings, ISystemSecurity systemSecurity)
        {
            _appSettings = appSettings;
            _systemSecurity = systemSecurity;
        }

        public async Task<MainResponse> GetProductWorkflowAsync(ProductWorkflowModel productworkflowmodel)
        {
            MainResponse mainresponse = new MainResponse();
            try
            {
                string strproductworkflowrequest = JsonConvert.SerializeObject(productworkflowmodel);
                UserInfo userInfo = _appSettings.userInfo;
                _connString = _systemSecurity.GetConnectionString(_appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName, _appSettings.BRUserName, _appSettings.BRUserPassword, "BRGateway24API");
                DateTime currentDateTime = DateTime.Now;

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand(Constants.GETPRODUCTWORKFLOW, sql))
                    {
                        SqlParameter param;

                        cmd.CommandType = CommandType.StoredProcedure;
                        param = cmd.Parameters.Add("@OurBranchID", SqlDbType.NVarChar, 15);
                        param.Value = userInfo.strBranch;
                        param = cmd.Parameters.Add("@DataJSON", SqlDbType.NVarChar, 200);
                        param.Value = strproductworkflowrequest;
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

                            if (res.Status == "000" || res.Status == "999")
                            {
                                while (await reader.ReadAsync())
                                {
                                    mainresponse = reader.ConvertToObject<MainResponse>();
                                }

                            }

                            mainresponse = new MainResponse()
                            {
                                resp = res,
                                OurBranchID = "",

                            };

                        }
                    }
                }
                return mainresponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<MainResponse> GetFDRatesAsync(FDRatesModel request)
        {
            MainResponse mainresponse = new MainResponse();
            try
            {
                string strRequest = JsonConvert.SerializeObject(request);
                UserInfo userInfo = _appSettings.userInfo;
                _connString = _systemSecurity.GetConnectionString(_appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName, _appSettings.BRUserName, _appSettings.BRUserPassword, "BRGateway24API");
                DateTime currentDateTime = DateTime.Now;

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand(Constants.GETFDRATES, sql))
                    {
                        SqlParameter param;

                        cmd.CommandType = CommandType.StoredProcedure;
                        param = cmd.Parameters.Add("@OurBranchID", SqlDbType.NVarChar, 15);
                        param.Value = userInfo.strBranch;
                        param = cmd.Parameters.Add("@DataJSON", SqlDbType.NVarChar, 200);
                        param.Value = strRequest;
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

                            if (res.Status == "000" || res.Status == "999")
                            {
                                while (await reader.ReadAsync())
                                {
                                    mainresponse = reader.ConvertToObject<MainResponse>();
                                }

                            }

                            mainresponse = new MainResponse()
                            {
                                resp = res,
                                OurBranchID = "",

                            };

                        }
                    }
                }
                return mainresponse;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<MainResponse> GetProductListAsync()
        {

             MainResponse mainresponse = new MainResponse();
            try
            {
                //string strOnBoardingRequest = JsonConvert.SerializeObject(divRebatesRequest);
                UserInfo userInfo = _appSettings.userInfo;
                _connString = _systemSecurity.GetConnectionString(_appSettings.DBType, _appSettings.DBServerName, _appSettings.DatabaseName, _appSettings.BRUserName, _appSettings.BRUserPassword, "BRGateway24API");
                DateTime currentDateTime = DateTime.Now;

                using (SqlConnection sql = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = new SqlCommand(Constants.GETPRODUCTLIST, sql))
                    {
                        SqlParameter param;

                        cmd.CommandType = CommandType.StoredProcedure;
                        param = cmd.Parameters.Add("@OurBranchID", SqlDbType.NVarChar, 15);
                        param.Value = userInfo.strBranch;
                        //param = cmd.Parameters.Add("@DataJSON", SqlDbType.NVarChar, 200);
                        //param.Value = strOnBoardingRequest;
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

                            if (res.Status == "000" || res.Status == "999")
                            {
                                while (await reader.ReadAsync())
                                {
                                    mainresponse = reader.ConvertToObject<MainResponse>();
                                }

                            }

                            mainresponse = new MainResponse()
                            {
                                resp = res,
                                OurBranchID = "",
                             
                            };

                        }
                    }
                }
                return mainresponse;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

     
    }
}
