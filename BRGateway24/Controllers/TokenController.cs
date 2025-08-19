using BRGateway24.Helpers;
using BRGateway24.Jwt;
using BRGateway24.Models;
using BRGateway24.Repository.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SeCy;
using SwiftApi.Repository.Security;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

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
        private Int64 tokenExpiry = 0;

        public TokenController(ISystemSecurity systemSecurity, ICommonRepo commonRepo, AppSettings appSettings)
        {
            _systemSecurity = systemSecurity;
            _commonRepo = commonRepo;
            _appSettings = appSettings;
            usrKey = _appSettings.UserKey;
            tokenExpiry = Convert.ToInt64(_appSettings.Expiry);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Generate([FromBody] HeaderRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { Status = "999", Message = "Invalid Request Parameters" });
            }

            try
            {
                string username = EnAction128.Funua(request.ConsumerKey, usrKey);
                string password = EnAction128.Funua(request.ConsumerSecret, usrKey);

                // Validate credentials
                Response validationResponse = new Response();
                if (!_systemSecurity.ValidRequest(request, out validationResponse))
                {
                    return Unauthorized(new { Status = "999", Message = "Invalid Credentials" });
                }

                // Generate proper JWT token with expiration
                var token = GenerateJwtToken(username, "YourBranchID", tokenExpiry);

                var mainResponse = new MainResponse()
                {
                    OurBranchID = "",
                    resp = new Response
                    {
                        Status = "000",
                        Message = "SUCCESS",
                        OutputJSON = null
                    }
                };

                var loginReq = new LoginReq
                {
                    response = mainResponse,
                    AccessToken = token,
                    ExpiresIn = tokenExpiry * 60 // Convert minutes to seconds
                };

                return Ok(loginReq);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "999", Message = "Internal server error", Details = ex.Message });
            }
        }

        private string GenerateJwtToken(string username, string branchId, long expireMinutes)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.JwtSecret ?? "YourSuperSecretKeyAtLeast32CharactersLong!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("branchId", branchId),
                    new Claim("consumerKey", username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
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
    }
}