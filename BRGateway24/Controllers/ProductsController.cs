using BRGateway24.Helpers;
using BRGateway24.Models;
using BRGateway24.Repository.Common;
using BRGateway24.Repository.ProductRepo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Reflection;

namespace BRGateway24.Controllers
{
    [TypeFilter(typeof(ValidateRequest))]
    [ApiController]
    [Route("[controller]")]

    public class ProductsController : ControllerBase
    {
        private readonly ICommonRepo _commonRepo;
        private readonly ILogger<ProductsController> _logger;
        private readonly AppSettings _appSettings;
        private readonly IProductRepo _productRepo;

        public ProductsController(ILogger<ProductsController> logger, AppSettings appSettings, ICommonRepo commonRepo, IProductRepo productRepo)
        {
            _logger = logger;
            _appSettings = appSettings;
            _commonRepo = commonRepo;
            _productRepo = productRepo;
        }

        /// <summary>
        /// Get product list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("GetProductList")]
        public async Task<MainResponse> GetProductList()
        {
            MainResponse mainResponse = new MainResponse();
            Response response = new Response();
            UserInfo userInfo = new UserInfo();
            string OurBranchID = string.Empty;


            try
            {
                mainResponse = await _productRepo.GetProductListAsync();
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
             

                ////////////////////Log these details start ///////////////////
                //string strRequest = JsonConvert.SerializeObject(divRebatesRequest);
                string strResponse = JsonConvert.SerializeObject(mainResponse);
                string auditID = Guid.NewGuid().ToString();
                APIAuditLogRequest reqAudit = new APIAuditLogRequest()
                {
                    Action = "POST",
                    AuditID = auditID,
                    DetailRecords = "",
                    MethodName = "GetProductList",
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