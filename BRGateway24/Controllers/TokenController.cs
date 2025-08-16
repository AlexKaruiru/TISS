using BRGateway24.Helpers;
using System.Configuration;
using System.Data;
using System.Net;
using SeCy;
using System.Security.Cryptography;
using System.Web.Http;
using ConfigurationManager = System.Configuration.ConfigurationManager;
using SwiftApi.Repository.Security;
using BRGateway24.Repository.Common;
using BRGateway24.Models;
using BRGateway24.Jwt;
using System.Web.Http.Description;
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = System.Web.Http.RouteAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using Newtonsoft.Json;
using System;
using BRGateway24.DataAccess;

namespace BRGateway24.Controllers
{
    
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ISystemSecurity _systemSecurity;
        private readonly ICommonRepo _commonRepo;
        private readonly AppSettings _appSettings;
        private string usrKey = string.Empty;
        Int64 tokenExpiry = 0;

        //Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IkRBTlNPIiwibmJmIjoxNTQ5ODc3MTAxLCJleHAiOjE3MDc2NDM1MDEsImlhdCI6MTU0OTg3NzEwMX0.0YZ3zjKesMCKO6xJIx6VvARPPVVx1YCmhvMrTxVUpCU

        /*{
             "ConsumerKey": "8cMMd15mGuEakmheqaUgaA5kCwJybD/nU+AINqHeo18CCAWABpE6FBz3q9f3x06rVs5qP2cygtueY8X2xvkkqkxEwZmtNIdRTuJgvfPKJQPbbNJ6KBcqmkxawIHXS5Ff",
             "ConsumerSecret": "iN0BPRLJb/v2aRgd8WDAVWkbsJOnWSWJn7JdAZgWithJhn7AdCISy1oCj1Y2A6YnTpNR/inT2HG+l77thRwNSOawuxE1Ul0AfD3sFXRb117Ihi6vdVHDiEjTqtM99Zny"
          }*/

        public TokenController(ISystemSecurity systemSecurity,ICommonRepo commonRepo, AppSettings appSettings)
        {
            _systemSecurity = systemSecurity;  
            _commonRepo = commonRepo;
            _appSettings = appSettings;

            usrKey = _appSettings.UserKey;
            tokenExpiry = Convert.ToInt64(_appSettings.Expiry);
        }




        /// <summary>
        /// This gives you time bound access token to call the allowed APIs
        /// </summary>
        /// <param name="usrData"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [AllowAnonymous]        
        public LoginReq Generate(HeaderRequest request)
        {
            
            UserInfo usrData = new UserInfo();
            MainResponse mainResponse = new MainResponse();
            LoginReq loginReq  = new LoginReq();
            string username = string.Empty;
            string password = string.Empty;
            string consumerOurBranchID = string.Empty;
            
            string Token = string.Empty;

            if (request == null)
            {                
                mainResponse = _commonRepo.InvokeResponse("Invalid Request Parameters", "999");

                loginReq = new LoginReq
                {
                    response = mainResponse,
                    AccessToken = ""
                };
                return loginReq;
            }

            try
            {
                if (request != null)
                {                    
                    username = EnAction128.Funua(request.ConsumerKey, usrKey);
                    password = EnAction128.Funua(request.ConsumerSecret, usrKey);
                    

                    usrData.strUser = username;
                    usrData.strBRUserPassword = password;
                    
                    HeaderRequest headerRequest = new HeaderRequest()
                    {
                        ConsumerKey = request.ConsumerKey,
                        ConsumerSecret = request.ConsumerSecret
                    };

                    Response response = new Response();

                    if (_systemSecurity.ValidRequest(headerRequest, out response))
                    {
                        
                        response.Status = "000";
                        response.Message = "SUCCESS";

                        mainResponse = new MainResponse()
                        {
                            OurBranchID = "",
                            resp = response
                        };

                        string token = JsonConvert.SerializeObject(headerRequest);
                        token = Constants.ConvertToBase64(token);

                        loginReq = new LoginReq
                        {
                            response = mainResponse,
                            AccessToken = token
                        };
                    }
                    else
                    {                        
                        mainResponse = _commonRepo.InvokeResponse("Invalid Credentials", "999");
                        loginReq = new LoginReq
                        {
                            response = mainResponse,
                            AccessToken = ""
                        };
                        return loginReq;
                    }

                }
                else
                {                    
                    mainResponse = _commonRepo.InvokeResponse("Invalid Credentials", "999");
                    loginReq = new LoginReq
                    {
                        response = mainResponse,
                        AccessToken = ""
                    };
                    return loginReq;
                }
            }
            catch (Exception ex)
            {
                mainResponse = _commonRepo.InvokeResponse("Invalid Credentials", "999");
                loginReq = new LoginReq
                {
                    response = mainResponse,
                    AccessToken = ""
                };
                return loginReq;
            }

            return loginReq;
        }


        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        //[Route(""), HttpGet]
        //[ApiExplorerSettings(IgnoreApi = true)]
        //public HttpResponseMessage RedirectToSwaggerUi()
        //{
        //    var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Found);
        //    httpResponseMessage.Headers.Location = new Uri("/ui/index", UriKind.Relative);
        //    return httpResponseMessage;
        //}
    }
}
