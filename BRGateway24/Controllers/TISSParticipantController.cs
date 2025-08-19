using BRGateway24.Models;
using BRGateway24.Repository.TISS;
using Microsoft.AspNetCore.Mvc;

namespace BRGateway24.Controllers
{
    [ApiController]
    [Route("interface/participants")]
    public class TISSParticipantController : ControllerBase
    {
        private readonly ILogger<TISSParticipantController> _logger;
        private readonly ITISSParticipantRepo _tissRepo;

        public TISSParticipantController(
            ILogger<TISSParticipantController> logger,
            ITISSParticipantRepo tissRepo)
        {
            _logger = logger;
            _tissRepo = tissRepo;
        }

        [HttpGet("businessDate")]
        public async Task<IActionResult> GetBusinessDate()
        {
            try
            {
                var headers = await _tissRepo.GetTissApiHeaders();
                if (headers == null)
                {
                    _logger.LogError("Failed to get TISS API headers");
                    return StatusCode(500, new { Error = "Service configuration error" });
                }

                _logger.LogInformation("Processing request with MsgId: {MsgId}", headers.MsgId);

                var response = await _tissRepo.GetBusinessDateAsync(headers);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetBusinessDate");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpGet("currentTimetableEvent")]
        public async Task<IActionResult> GetCurrentTimetableEvent()
        {
            try
            {
                var headers = await _tissRepo.GetTissApiHeaders();
                if (headers == null)
                {
                    return StatusCode(500, new { Error = "Service configuration error" });
                }

                var response = await _tissRepo.GetCurrentTimetableEventAsync(headers);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCurrentTimetableEvent");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpPost("message")]
        public async Task<IActionResult> PostMessage([FromBody] TissSendMessageRequest request)
        {
            try
            {
                if (!Request.Headers.TryGetValue("Content-Type", out var contentType) 
                    || !contentType.ToString().Contains("application/json"))
                {
                    return BadRequest("Invalid Content-Type");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var headers = await _tissRepo.GetTissApiHeaders();
                if (headers == null)
                {
                    return StatusCode(500, new { Error = "Service configuration error" });
                }

                headers.MsgId = request.Reference;

                _logger.LogInformation("Sending message with Reference: {Reference}", request.Reference);

                var response = await _tissRepo.SendMessageAsync(
                    request.MessageType,
                    request.PayloadXML,
                    request.Reference,
                    headers);

                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in PostMessage");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpGet("pendingTransactions")]
        public async Task<IActionResult> GetPendingTransactions(
            [FromQuery] string participantId = null,
            [FromQuery] string currency = "TZS")
        {
            try
            {
                var headers = await _tissRepo.GetTissApiHeaders();
                if (headers == null)
                {
                    return StatusCode(500, new { Error = "Service configuration error" });
                }

                var response = await _tissRepo.GetPendingTransactionsAsync(participantId, currency, headers);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPendingTransactions");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        [HttpGet("accountsActivity")]
        public async Task<IActionResult> GetAccountsActivity(
            [FromQuery] string participantId = null,
            [FromQuery] string accountId = null,
            [FromQuery] string fromDate = null,
            [FromQuery] string toDate = null,
            [FromQuery] string currency = "TZS")
        {
            try
            {
                var headers = await _tissRepo.GetTissApiHeaders();
                if (headers == null)
                {
                    return StatusCode(500, new { Error = "Service configuration error" });
                }

                DateTime? fromDateDt = null;
                DateTime? toDateDt = null;

                if (!string.IsNullOrEmpty(fromDate) && DateTime.TryParse(fromDate, out var parsedFromDate))
                {
                    fromDateDt = parsedFromDate;
                }

                if (!string.IsNullOrEmpty(toDate) && DateTime.TryParse(toDate, out var parsedToDate))
                {
                    toDateDt = parsedToDate;
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAccountsActivity");
                return StatusCode(500, new { Error = "Internal server error" });
            }
        }

        private IActionResult HandleResponse(MainResponse response)
        {
            if (response.resp == null)
            {
                return StatusCode(500, new { Error = "Null response from service" });
            }

            if (response.resp.Status == "000")
            {
                return Ok(new
                {
                    Status = "Success",
                    Data = response.resp.OutputJSON,
                    Timestamp = DateTime.UtcNow
                });
            }

            if (response.resp.Status == "404")
            {
                return NotFound(new
                {
                    Status = "Not Found",
                    Message = response.resp.Message,
                    Timestamp = DateTime.UtcNow
                });
            }

            return StatusCode(
                int.TryParse(response.resp.Status, out var statusCode) ? statusCode : 500,
                new
                {
                    Status = "Error",
                    Code = response.resp.Status,
                    Message = response.resp.Message,
                    Timestamp = DateTime.UtcNow
                });
        }
    }

}