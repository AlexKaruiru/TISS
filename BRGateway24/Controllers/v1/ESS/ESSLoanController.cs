using BRGateway24.Helpers;
using BRGateway24.Helpers.ESS.LoanHelpter;
using BRGateway24.Models;
using BRGateway24.Repository.Common;
using BRGateway24.Repository.ESS.Interfaces;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;




namespace BRGateway24.Controllers.v1.ESS
{
    //[TypeFilter(typeof(ValidateRequest))]
    [ApiController]
    [Route("[controller]")]
    public class ESSLoanController : ControllerBase
    {
        private readonly ICommonRepo _commonRepo;
        private readonly ILogger<ESSLoanController> _logger;
        private readonly AppSettings _appSettings;
        private readonly IESSLoansRepo _ESSloanRepo;
        string EncryptPassword = string.Empty;


        public ESSLoanController(ILogger<ESSLoanController> logger, ICommonRepo commonRepo, AppSettings appSettings, IESSLoansRepo ESSloanRepo)
        {
            _logger = logger;
            _appSettings = appSettings;
            _commonRepo = commonRepo;
            _ESSloanRepo = ESSloanRepo;
        }

        //New loans
        // 1.   - Loan Charges Request from Employee to FSP
        // 1.1  - Loan Charges Response from FSP to Employee

        // 2   Loan Offer Request from Employee to FSP
        // 2.1 Loan Offer Approval Notification from FSP to ESS

        //3    Loan Verification and Approval
        //3.1  Disbursement Notification from FSP to ESS
        //3.2  Disbursement Failure Notification from FSP to ESS



        /// <summary>
        /// Loan Application
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("ESSLoanApplication")]
        public async Task<MainResponse> ESSLoanApplication()
        {
            MainResponse mainResponse = new MainResponse();
            Response response = new Response();
            UserInfo userInfo = new UserInfo();
            LoanDataProcessor processor = new LoanDataProcessor();
            using var reader = new StreamReader(Request.Body);

            var xmlContent = await reader.ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(xmlContent))

            {
                var (ESSNewLoanModel, ESSNewLoanTable) = processor.ProcessRequestDynamic(xmlContent);
                string strESSNewLoanTableRequest = JsonConvert.SerializeObject(ESSNewLoanTable);
                try
                {
                    if (strESSNewLoanTableRequest is not null)
                    {
                        mainResponse = await _ESSloanRepo.ESSLoanApplicationAsync(strESSNewLoanTableRequest);
                        if (mainResponse is not null)
                        {
                            mainResponse = new MainResponse()
                            {
                                resp = new Response()
                                {
                                    Status = mainResponse.resp.Status,
                                    OutputJSON = mainResponse.resp.OutputJSON,
                                    Message = mainResponse.resp.Message
                                }
                            };
                        }

                    }
                    else
                    {
                        mainResponse = _commonRepo.InvokeResponse();
                    }
                    ////////////////////Log these details start ////////////////
                    string strRequest = JsonConvert.SerializeObject(xmlContent);
                    string strResponse = JsonConvert.SerializeObject(mainResponse);
                    string auditID = Guid.NewGuid().ToString();
                    APIAuditLogRequest reqAudit = new APIAuditLogRequest()
                    {
                        Action = "POST",
                        AuditID = auditID,
                        DetailRecords = "",
                        MethodName = " Loan Application",
                        Request = strRequest,
                        Response = strResponse
                    };

                    await _commonRepo.APIAuditLogAsync(reqAudit);
                    ////////////////////Log these details end ////////////////
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().DeclaringType.Name, userInfo.strUser, userInfo.MachineIP);
                }
                //}
            }


            return mainResponse;
        }






    }
}
