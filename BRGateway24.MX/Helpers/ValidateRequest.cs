using BRGateway24.Models;
using BRGateway24.Repository.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SeCy;
using SwiftApi.Repository.Security;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BRGateway24.Helpers
{
    public class ValidateRequest : ActionFilterAttribute
    {
        private readonly ISystemSecurity _sysSecurity;
        private readonly AppSettings _appSettings;
        private readonly ICommonRepo _commonRepo;
        public static string _errorLogPath = string.Empty;

        public ValidateRequest(AppSettings appSettings, ISystemSecurity systemSecurity, ICommonRepo commonRepo)
        {
            _appSettings = appSettings;
            _sysSecurity = systemSecurity;
            _commonRepo = commonRepo;
            _errorLogPath = _appSettings.ErrorLogPath;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                string token = string.Empty;

                // First try to get token from standard Authorization header
                token = context.HttpContext.Request.Headers["Authorization"];

                // If not found in Authorization header, try the custom token header
                if (string.IsNullOrEmpty(token))
                {
                    token = context.HttpContext.Request.Headers["token"];
                }

                if (string.IsNullOrEmpty(token))
                {
                    context.Result = new UnauthorizedObjectResult(new { Status = "999", Message = "Authorization token missing" });
                    return;
                }

                // Remove "Bearer " prefix if present
                string cleanToken = ExtractBearerToken(token);

                if (string.IsNullOrEmpty(cleanToken))
                {
                    context.Result = new UnauthorizedObjectResult(new { Status = "999", Message = "Invalid token format" });
                    return;
                }

                // Validate JWT token
                if (!ValidateJwtToken(cleanToken, out var principal))
                {
                    context.Result = new UnauthorizedObjectResult(new { Status = "999", Message = "Invalid or expired token" });
                    return;
                }

                var username = principal.FindFirst(ClaimTypes.Name)?.Value;
                var branchId = principal.FindFirst("branchId")?.Value;

                if (string.IsNullOrEmpty(username))
                {
                    context.Result = new UnauthorizedObjectResult(new { Status = "999", Message = "Invalid token claims" });
                    return;
                }

                context.HttpContext.Items["Username"] = username;
                context.HttpContext.Items["BranchId"] = branchId;
                context.HttpContext.Items["Token"] = cleanToken;
            }
            catch (SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedObjectResult(new { Status = "999", Message = "Token has expired" });
            }
            catch (Exception ex)
            {
                context.Result = new UnauthorizedObjectResult(new { Status = "999", Message = "Token validation failed", Details = ex.Message });
            }
        }

        private string ExtractBearerToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return null;

            // Remove "Bearer " prefix if present (case insensitive)
            if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return token.Substring("Bearer ".Length).Trim();
            }

            // Also handle "bearer " (lowercase)
            if (token.StartsWith("bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return token.Substring("bearer ".Length).Trim();
            }

            // If no prefix, assume it's already a clean token
            return token.Trim();
        }

        private bool ValidateJwtToken(string token, out ClaimsPrincipal principal)
        {
            principal = null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // Check if token is a valid JWT format first
                if (!tokenHandler.CanReadToken(token))
                {
                    return false;
                }

                var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret ?? "YourSuperSecretKeyAtLeast32CharactersLong!");

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    // For better error messages, you can add:
                    // ValidIssuer = _appSettings.JwtIssuer,
                    // ValidAudience = _appSettings.JwtAudience
                };

                principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                // Token is valid but expired
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                // Invalid signature
                return false;
            }
            catch (SecurityTokenMalformedException)
            {
                // Malformed token
                return false;
            }
            catch (Exception)
            {
                // Any other error
                return false;
            }
        }
    }
}