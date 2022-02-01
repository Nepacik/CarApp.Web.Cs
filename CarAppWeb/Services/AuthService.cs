using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using CarAppWeb.Dtos;
using CarAppWeb.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CarAppWeb.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private const string serviceUrl = "Auth/";

        public AuthService(IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor) :
            base(configuration, httpClient, serviceUrl, httpContextAccessor)
        {
        }

        public async Task<ResponseState<TokenDto>> AttemptToLogin(LoginDto loginDto)
        {
            var httpServiceData = new HttpServiceData("Login", HttpMethod.Post);
            var result = await HandleRequest<TokenDto, LoginDto>(httpServiceData, loginDto);
            if (result is ResponseState<TokenDto>.Data data)
            {
                if (await TokenStorageProvider.SetTokenClaims(data.Content, HttpContextAccessor))
                {
                    return result;
                }

                return new ResponseState<TokenDto>.UnknownError();
            }
            
            return result;
        }

        public async Task<ResponseState<TokenDto>> AttemptToRegister(RegisterDto register)
        {
            var httpServiceData = new HttpServiceData("Register", HttpMethod.Post);
            var result = await HandleRequest<TokenDto, RegisterDto>(httpServiceData, register);
            if (result is ResponseState<TokenDto>.Data data)
            {
                if (await TokenStorageProvider.SetTokenClaims(data.Content, HttpContextAccessor))
                {
                    return result;
                }

                return new ResponseState<TokenDto>.UnknownError();
            }
            
            return result;
        }
    }
}