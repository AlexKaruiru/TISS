using BRGateway24.Models;
using BRGateway24.Repository;
using BRGateway24.Repository.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using SeCy;
using Serilog.Context;
using SwiftApi.Repository.Security;
using System.Security.Cryptography;
using System.Text;

namespace BRGateway24.Helpers
{
    public class ValidateRequest : ActionFilterAttribute
    {              
        private readonly ISystemSecurity _sysSecurity;
        private readonly AppSettings _appSettings;        
        string token = String.Empty;
        private readonly ICommonRepo _commonRepo;
        public static string _errorLogPath = string.Empty;


        public ValidateRequest(AppSettings appSettings,ISystemSecurity systemSecurity, ICommonRepo commonRepo)
        {            
            _appSettings = appSettings;
            _sysSecurity = systemSecurity;
            _commonRepo = commonRepo;
            _errorLogPath = _appSettings.ErrorLogPath;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Response resp = new Response();
            try
            {
                
                if (context.HttpContext.Request.Headers["token"].Count > 0)
                {
                    token = Convert.ToString(context.HttpContext.Request.Headers["token"][0]);
                }


                if (token == null || token == string.Empty)
                {
                    resp.Status = "999";                    
                    resp.Message = "User is Not Authorized";

                    context.Result = new UnauthorizedObjectResult(resp);
                    return;
                }

                ////Log to file////
                if (_errorLogPath.ToString() != "")
                {
                    string strFileName = $"{_errorLogPath}API";
                    System.IO.Directory.CreateDirectory(strFileName);

                    //string AppendErrorMessage = Environment.NewLine + "Token Used" + ":" + token +
                    //    " Date" + ":" + DateTime.Now + Environment.NewLine + "--------------------------" + Environment.NewLine;
                    //System.IO.File.AppendAllText(strFileName + "_Error_" + DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "") + ".txt", AppendErrorMessage);
                }
                ///////////////////////////////
                ///
                token = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                HeaderRequest headerReq = JsonConvert.DeserializeObject<HeaderRequest>(token);

                ////////////////////Log these details start ////////////////
                string strRequest = JsonConvert.SerializeObject(headerReq);
                string strResponse = string.Empty;
                string auditID = Guid.NewGuid().ToString();
                APIAuditLogRequest reqAudit = new APIAuditLogRequest()
                {
                    Action = "POST",
                    AuditID = auditID,
                    DetailRecords = "",
                    MethodName = "Validate Request",
                    Request = strRequest,
                    Response = strResponse
                };

                _commonRepo.APIAuditLogAsync(reqAudit);
                ////////////////////Log these details end ////////////////

                

                if (_sysSecurity.ValidRequest(headerReq, out resp))
                {

                    //context.Result = new OkResult();
                   
                    return;
                }
                else
                {
                    //resp.Status = "999";                    
                    //resp.Message = "Bad Request";

                    context.Result = new BadRequestObjectResult(resp);
                    return;
                }


            }
            catch (Exception ex)
            {
                resp.Status = "999";                
                resp.Message = "Severe Error has occured";

                context.Result = new BadRequestObjectResult(resp);
                return;
            }


        }


       
    }
}
