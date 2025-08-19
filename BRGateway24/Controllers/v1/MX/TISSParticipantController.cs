using BRGateway24.Helpers;
using BRGateway24.Models;
using BRGateway24.Repository.TISS;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;

[ApiController]
[TypeFilter(typeof(ValidateRequest))]
[Route("v1/MX/interface/participants")]
[Tags("MX")]
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
    public async Task<IActionResult> GetBusinessDate([FromQuery] string currency = "TZS")
    {
        try
        {
            var headers = await _tissRepo.GetTissApiHeaders();
            if (headers == null)
            {
                return StatusCode(500, new { Error = "Service configuration error" });
            }

            // Set currency parameter
            headers.Currency = currency;

            var response = await _tissRepo.GetBusinessDateAsync(headers);
            return HandleTissResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetBusinessDate");
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }

    [HttpGet("currentTimetableEvent")]
    public async Task<IActionResult> GetCurrentTimetableEvent([FromQuery] string currency = "TZS")
    {
        try
        {
            var headers = await _tissRepo.GetTissApiHeaders();
            if (headers == null)
            {
                return StatusCode(500, new { Error = "Service configuration error" });
            }

            // Set currency parameter
            headers.Currency = currency;

            var response = await _tissRepo.GetCurrentTimetableEventAsync(headers);
            return HandleTissResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetCurrentTimetableEvent");
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }

    [HttpPost("message")]
    public async Task<IActionResult> PostMessage([FromBody] TissSendMessageRequest request)
    {
        try
        {
            if (!Request.Headers.TryGetValue("Content-Type", out var contentType) || !contentType.ToString().Contains("application/json"))
            {
                return BadRequest(new { Error = "Invalid Content-Type. Must be text/xml or application/xml" });
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

            // Set message-specific headers
            headers.MsgId = request.Reference;
            headers.ContentType = contentType.ToString();
            headers.PayloadType = "XML";

            _logger.LogInformation("Sending message with Reference: {Reference}", request.Reference);

            var response = await _tissRepo.SendMessageAsync(
                request.MessageType,
                request.PayloadXML,
                request.Reference,
                headers);

            return HandleTissResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in PostMessage");
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }

    [HttpGet("pendingTransactions")]
    public async Task<IActionResult> GetPendingTransactions(
        [FromQuery] string sender, // Required parameter
        [FromQuery] string currency = "TZS")
    {
        try
        {
            if (string.IsNullOrEmpty(sender))
            {
                return BadRequest(new { Error = "sender parameter is required" });
            }

            var headers = await _tissRepo.GetTissApiHeaders();
            if (headers == null)
            {
                return StatusCode(500, new { Error = "Service configuration error" });
            }

            // Set required parameters
            headers.Sender = sender;
            headers.Currency = currency;

            var response = await _tissRepo.GetPendingTransactionsAsync(sender, currency, headers);
            return HandleTissResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetPendingTransactions");
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }

    [HttpGet("accountsActivity")]
    public async Task<IActionResult> GetAccountsActivity(
        [FromQuery] string sender, // Required parameter
        [FromQuery] string accountId = null,
        [FromQuery] string fromDate = null,
        [FromQuery] string toDate = null,
        [FromQuery] string currency = "TZS")
    {
        try
        {
            if (string.IsNullOrEmpty(sender))
            {
                return BadRequest(new { Error = "sender parameter is required" });
            }

            var headers = await _tissRepo.GetTissApiHeaders();
            if (headers == null)
            {
                return StatusCode(500, new { Error = "Service configuration error" });
            }

            // Set required parameters
            headers.Sender = sender;
            headers.Currency = currency;

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
                sender,
                accountId,
                fromDateDt,
                toDateDt,
                currency,
                headers);

            return HandleTissResponse(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetAccountsActivity");
            return StatusCode(500, new { Error = "Internal server error", Details = ex.Message });
        }
    }

    private IActionResult HandleTissResponse(MainResponse response)
    {
        if (response == null)
        {
            return StatusCode(500, new
            {
                Error = "Null response from TISS service",
                Code = "500"
            });
        }

        if (response.resp == null)
        {
            return StatusCode(500, new
            {
                Error = "Null response body from TISS service",
                Code = "500"
            });
        }

        // Return the exact response from TISS without modification
        return StatusCode(
            int.TryParse(response.resp.Status, out var statusCode) ? statusCode : 500,
            new
            {
                Status = response.resp.Status,
                Message = response.resp.Message,
                Data = response.resp.OutputJSON,
                Timestamp = DateTime.UtcNow,
            });
    }
}