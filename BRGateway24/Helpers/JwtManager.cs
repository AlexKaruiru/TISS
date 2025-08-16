﻿using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
//using BR.Configuration;
using System.Web.Http.Filters;
using System.Web.Http;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using System.Linq;

namespace BRGateway24.Jwt
{
    public static class JwtManager
    {
        /// <summary>
        /// Use the below code to generate symmetric Secret Key
        ///     var hmac = new HMACSHA256();
        ///     var key = Convert.ToBase64String(hmac.Key);
        /// </summary>
        private const string Secret = "Vm10a1IyTXlSalZSYmtKcVpWVktjVmxWWkZkaFIwNUVUa2RrVmsxdGFESmFTR3hEWkVad1ZGRnFRbWhTTVZadVZWUkJOVkpXU2xSU1ZEQTk=";


        public static string GenerateToken(string username, string BranchID, Int64 expireMinutes)
        {
            var symmetricKey = Convert.FromBase64String(Secret);
            var tokenHandler = new JwtSecurityTokenHandler();

            var now = DateTime.UtcNow;
            try
            {
                if (expireMinutes == -1)
                {
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username),
                            new Claim(ClaimTypes.UserData, BranchID)
                        }),

                        Expires = now.AddYears(5),

                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var stoken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(stoken);
                    return token;
                }
                else
                {
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, username)
                        }),

                        Expires = now.AddMinutes(Convert.ToInt64(expireMinutes)),

                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var stoken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(stoken);
                    return token;
                }

            }
            catch (Exception exx)
            {
                return exx.Message.ToString();

            }
        }

        public static ClaimsPrincipal GetPrincipal(string token)
        {
            dynamic principal = null;
            HttpAuthenticationContext context;

            try
            {
                //token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImRhdmlzIiwibmJmIjoxNTAzNTc5OTQzLCJleHAiOjE1MDM1ODExNDMsImlhdCI6MTUwMzU3OTk0M30.1VqSj-dbb-5Ef7aZmOvgVZk6k2IN1dLDrjRobvh_h8E";
                var tokenHandler = new JwtSecurityTokenHandler();

                var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

                if (jwtToken == null)
                    return null;

                var symmetricKey = Convert.FromBase64String(Secret);

                var validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                SecurityToken securityToken;

                principal = tokenHandler.ValidateToken(token, validationParameters, out securityToken);


                return principal;
            }

            catch (SecurityTokenException ste)
            {
                return principal;
            }
        }
        public static void InvalidateClaims(string token, string usrName, string branchID)
        {
            ClaimsPrincipal princ = JwtManager.GetPrincipal(token);
            var identity = princ.Identities.ToList<ClaimsIdentity>();
            ClaimsIdentity sbj = new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Name, usrName),
                            new Claim(ClaimTypes.UserData, branchID)
                        });
            foreach (var idty in identity)
            {

                foreach (var claim in idty.Claims)
                {
                    if (claim.Subject == sbj)
                    {
                        if (claim.Value == usrName && claim.Type == ClaimTypes.Name)
                            idty.RemoveClaim(claim);
                        if (claim.Value == branchID && claim.Type == ClaimTypes.UserData)
                            idty.RemoveClaim(claim);
                    }
                }
            }

        }
    }

    public class JSONActionResult : IHttpActionResult
    {
        private readonly string _jsonString;

        public JSONActionResult(string jsonString)
        {
            _jsonString = jsonString;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var content = new StringContent(_jsonString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = content };
            return Task.FromResult(response);
        }
    }


}