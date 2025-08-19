using BRGateway24.Helpers;
using BRGateway24.Models;
using BRGateway24.Repository.Common;
using BRGateway24.Repository.TISS;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BRGateway24.Controllers
{
    [ApiController]
    [TypeFilter(typeof(ValidateRequest))]
    [Route("interface/participants")]
    public class TISSParticipantController : ControllerBase
    {
        private readonly ILogger<TISSParticipantController> _logger;
        private readonly AppSettings _appSettings;
        private readonly ICommonRepo _commonRepo;
        private readonly ITISSParticipantRepo _tissRepo;

        public TISSParticipantController(
            ILogger<TISSParticipantController> logger,
            AppSettings appSettings,
            ICommonRepo commonRepo,
            ITISSParticipantRepo tissRepo)
        {
            _logger = logger;
            _appSettings = appSettings;
            _commonRepo = commonRepo;
            _tissRepo = tissRepo;
        }

        [HttpGet("businessDate")]
        public async Task<IActionResult> GetBusinessDate()
        {
            var headers = (TissApiHeaders)HttpContext.Items["TissHeaders"];
            var response = await _tissRepo.GetBusinessDateAsync(headers);
            return HandleResponse(response);
        }

        [HttpGet("currentTimetableEvent")]
        public async Task<IActionResult> GetCurrentTimetableEvent()
        {
            var headers = (TissApiHeaders)HttpContext.Items["TissHeaders"];
            var response = await _tissRepo.GetCurrentTimetableEventAsync(headers);
            return HandleResponse(response);
        }

        [HttpPost("message")]
        public async Task<IActionResult> PostMessage([FromBody] TissSendMessageRequest request)
        {
            // The global ValidateRequest filter will have already handled the token validation.
            var headers = (TissApiHeaders)HttpContext.Items["TissHeaders"];

            // Validate content-type header for POST requests
            if (!Request.Headers.TryGetValue("Content-Type", out var contentType) ||
                (!contentType.ToString().Contains("text/xml") && !contentType.ToString().Contains("application/xml")))
            {
                return BadRequest("Invalid Content-Type. Must be text/xml or application/xml");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            headers.MsgId = request.Reference;

            var response = await _tissRepo.SendMessageAsync(
                request.MessageType,
                request.PayloadXML,
                request.Reference,
                headers);

            return HandleResponse(response);
        }

        [HttpGet("pendingTransactions")]
        public async Task<IActionResult> GetPendingTransactions(
            [FromQuery] string participantId = null,
            [FromQuery] string currency = "TZS")
        {
            var headers = (TissApiHeaders)HttpContext.Items["TissHeaders"];
            var response = await _tissRepo.GetPendingTransactionsAsync(participantId, currency, headers);
            return HandleResponse(response);
        }

        [HttpGet("accountsActivity")]
        public async Task<IActionResult> GetAccountsActivity(
            [FromQuery] string participantId = null,
            [FromQuery] string accountId = null,
            [FromQuery] string fromDate = null,
            [FromQuery] string toDate = null,
            [FromQuery] string currency = "TZS")
        {
            var headers = (TissApiHeaders)HttpContext.Items["TissHeaders"];

            DateTime? fromDateDt = null;
            DateTime? toDateDt = null;

            if (!string.IsNullOrEmpty(fromDate) && !DateTime.TryParse(fromDate, out var parsedFromDate))
            {
                return BadRequest("Invalid fromDate format");
            }
            if (!string.IsNullOrEmpty(toDate) && !DateTime.TryParse(toDate, out var parsedToDate))
            {
                return BadRequest("Invalid toDate format");
            }

            var response = await _tissRepo.GetAccountActivitiesAsync(
                participantId,
                accountId,
                fromDateDt,
                toDateDt,
                currency,
                headers);

            return HandleResponse(response);
        }

        private IActionResult HandleResponse(MainResponse response)
        {
            if (response.resp.Status == "000")
                return Ok(response.resp.OutputJSON);

            if (response.resp.Status == "404")
                return NotFound(response.resp.Message);

            return StatusCode(int.TryParse(response.resp.Status, out var statusCode) ? statusCode : 500,
                new { Error = response.resp.Message });
        }
    }
}