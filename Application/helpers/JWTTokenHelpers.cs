using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Application.helpers
{
    public class JwtTokenHelpers
    {
        public JwtTokenHelpers()
        {
        }

        public string? GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                if (tokenHandler.CanReadToken(token))
                {
                    var jwtToken = tokenHandler.ReadJwtToken(token);

                    var userIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "nameid");

                    return userIdClaim?.Value;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
