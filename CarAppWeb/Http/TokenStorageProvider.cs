using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CarAppWeb.Dtos;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;

namespace CarAppWeb.Http
{
    public static class TokenStorageProvider
    {

        public async static Task<bool> SetTokenClaims(TokenDto tokenDto, IHttpContextAccessor httpContextAccessor)
        {
            try
            {
                var auth = await httpContextAccessor.HttpContext.AuthenticateAsync("Cookies");
                var authProperties = new AuthenticationProperties();
                authProperties.StoreTokens(new List<AuthenticationToken>()
                {
                    new AuthenticationToken()
                    {
                        Name = nameof(TokenDto.AccessToken),
                        Value = tokenDto.AccessToken
                    },
                    new AuthenticationToken()
                    {
                        Name = nameof(TokenDto.RefreshToken),
                        Value = tokenDto.RefreshToken
                    },
                });
                var tokenHandler = new JwtSecurityTokenHandler();
                var decodedToken = tokenHandler.ReadToken(tokenDto.AccessToken);
                var tokenS = decodedToken as JwtSecurityToken;

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, tokenS.Claims.First(x => x.Type == "unique_name").Value),
                    new Claim(ClaimTypes.Role, tokenS.Claims.First(x => x.Type == "role").Value),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}