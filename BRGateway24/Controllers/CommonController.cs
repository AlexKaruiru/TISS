
using BRGateway24.Helpers;
using BRGateway24.Models;
using BRGateway24.Repository.Common;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace BRGateway24.Controllers
{


    [TypeFilter(typeof(ValidateRequest))]
    [ApiController]
    [Route("[controller]")]

    public class CommonController : ControllerBase
    {
        private readonly ICommonRepo _commonRepo;
        private readonly ILogger<CommonController> _logger;
        private readonly AppSettings _appSettings;


        public CommonController(ILogger<CommonController> logger, AppSettings appSettings, ICommonRepo commonRepo)
        {
            _logger = logger;
            _appSettings = appSettings;
            _commonRepo = commonRepo;
        }


        /// <summary>
        /// Get Account details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpPost("GetCodes")]
        private async Task<MainResponse> GetCodeDetails(CodeDetails codeDetails)
        {
            MainResponse mainResponse = new MainResponse();
            Response response = new Response();
            UserInfo userInfo = new UserInfo();

            string OurBranchID = string.Empty;

            try
            {

                if (codeDetails != null)
                {
                    mainResponse = await _commonRepo.GetCodesAsync(codeDetails);

                    if (mainResponse != null)
                    {
                        mainResponse = new MainResponse()
                        {
                            resp = new Response()
                            {
                                OutputJSON = mainResponse.resp.OutputJSON,
                                Status = mainResponse.resp.Status,
                                Message = mainResponse.resp.Message
                            }
                        };
                    }
                }
                else
                {
                    mainResponse = _commonRepo.InvokeResponse();
                }

                ////////////////////Log these details start ///////////////////
                string strRequest = JsonConvert.SerializeObject(codeDetails);
                string strResponse = JsonConvert.SerializeObject(mainResponse);
                string auditID = Guid.NewGuid().ToString();
                APIAuditLogRequest reqAudit = new APIAuditLogRequest()
                {
                    Action = "POST",
                    AuditID = auditID,
                    DetailRecords = "",
                    MethodName = "GetCodes",
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

            return mainResponse;
        }
    }
}
