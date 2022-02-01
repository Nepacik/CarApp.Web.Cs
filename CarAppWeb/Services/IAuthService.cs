using System.Threading.Tasks;
using CarAppWeb.Dtos;
using CarAppWeb.Http;

namespace CarAppWeb.Services
{
    public interface IAuthService
    {
        public Task<ResponseState<TokenDto>> AttemptToLogin(LoginDto loginDto);
        public Task<ResponseState<TokenDto>> AttemptToRegister(RegisterDto register);
    }
}