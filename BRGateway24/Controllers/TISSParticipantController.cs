using BRGateway24.Helpers;
using BRGateway24.Models;
using BRGateway24.Repository.Common;
using BRGateway24.Repository.TISS;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BRGateway24.Controllers
{
    [ApiController]
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

        private async Task<(bool isValid, string participantId, TissApiHeaders headers, ValidationResult validation)> ValidateRequestHeaders()
        {
            var headers = new TissApiHeaders();

            if (!Request.Headers.TryGetValue("Authorization", out var authHeader))
                return (false, null, null, new ValidationResult("Authorization header is required"));
            headers.Authorization = authHeader;

            if (!Request.Headers.TryGetValue("sender", out var senderHeader))
                return (false, null, null, new ValidationResult("sender header is required"));
            headers.Sender = senderHeader;

            if (!Request.Headers.TryGetValue("msgid", out var msgIdHeader))
                return (false, null, null, new ValidationResult("msgid header is required"));
            headers.MsgId = msgIdHeader;

            // Set default consumer if not provided
            if (Request.Headers.TryGetValue("consumer", out var consumerHeader))
                headers.Consumer = consumerHeader;
            else
                headers.Consumer = "TANZTZTX"; // Default to Central Bank BIC

            // Validate token
            var isValid = await _tissRepo.ValidateTokenAsync(headers.Authorization, headers.Sender);
            if (!isValid)
                return (false, null, null, new ValidationResult("Invalid or expired authorization token"));

            // Get participant ID
            var participantId = await GetParticipantIdByBic(headers.Sender);
            if (string.IsNullOrEmpty(participantId))
                return (false, null, null, new ValidationResult("Participant not found"));

            return (true, participantId, headers, ValidationResult.Success);
        }

        private async Task<string> GetParticipantIdByBic(string bic)
        {
            // Implement logic to get participant ID by BIC
            // This would typically query your TISS_Participants table
            return "PARTICIPANT1"; // Example - replace with actual implementation
        }

        [HttpGet("businessDate")]
        public async Task<IActionResult> GetBusinessDate()
        {
            var (isValid, _, headers, validation) = await ValidateRequestHeaders();
            if (!isValid)
                return Unauthorized(validation.ErrorMessage);

            var response = await _tissRepo.GetBusinessDateAsync(headers);
            return HandleResponse(response);
        }

        [HttpGet("currentTimetableEvent")]
        public async Task<IActionResult> GetCurrentTimetableEvent()
        {
            var (isValid, _, headers, validation) = await ValidateRequestHeaders();
            if (!isValid)
                return Unauthorized(validation.ErrorMessage);

            var response = await _tissRepo.GetCurrentTimetableEventAsync(headers);
            return HandleResponse(response);
        }

        [HttpPost("message")]
        public async Task<IActionResult> PostMessage([FromBody] TissSendMessageRequest request)
        {
            var (isValid, participantId, headers, validation) = await ValidateRequestHeaders();
            if (!isValid)
                return Unauthorized(validation.ErrorMessage);

            // Validate content-type header for POST requests
            if (!Request.Headers.TryGetValue("content-type", out var contentType) ||
                (contentType != "text/xml" && contentType != "application/xml"))
                return BadRequest("Invalid content-type. Must be text/xml or application/xml");

            if (!Request.Headers.TryGetValue("payload_type", out var payloadType) || payloadType != "XML")
                return BadRequest("Invalid payload_type. Must be XML");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get msgid from header
            var msgId = Request.Headers["msgid"].ToString();

            var response = await _tissRepo.SendMessageAsync(
                msgId,
                participantId,
                request.MessageType,
                request.PayloadXML,
                request.Reference,
                headers);

            return HandleResponse(response);
        }

        [HttpGet("pendingTransactions")]
        public async Task<IActionResult> GetPendingTransactions([FromQuery] string currency = "TZS")
        {
            var (isValid, participantId, headers, validation) = await ValidateRequestHeaders();
            if (!isValid)
                return Unauthorized(validation.ErrorMessage);

            var response = await _tissRepo.GetPendingTransactionsAsync(participantId, currency, headers);
            return HandleResponse(response);
        }

        [HttpGet("accountsActivity")]
        public async Task<IActionResult> GetAccountsActivity(
            [FromQuery] string accountId = null,
            [FromQuery] string fromDate = null,
            [FromQuery] string toDate = null,
            [FromQuery] string currency = "TZS")
        {
            var (isValid, participantId, headers, validation) = await ValidateRequestHeaders();
            if (!isValid)
                return Unauthorized(validation.ErrorMessage);

            DateTime? fromDateDt = null;
            DateTime? toDateDt = null;

            // Parse fromDate
            if (!string.IsNullOrEmpty(fromDate))
            {
                if (DateTime.TryParse(fromDate, out var parsedFromDate))
                {
                    fromDateDt = parsedFromDate;
                }
                else
                {
                    return BadRequest("Invalid fromDate format");
                }
            }

            // Parse toDate
            if (!string.IsNullOrEmpty(toDate))
            {
                if (DateTime.TryParse(toDate, out var parsedToDate))
                {
                    toDateDt = parsedToDate;
                }
                else
                {
                    return BadRequest("Invalid toDate format");
                }
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

            return StatusCode(500, response.resp.Message);
        }
    }
}