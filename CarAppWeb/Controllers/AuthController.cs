using System;
using CarAppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using CarAppWeb.Dtos;
using CarAppWeb.Http;
using CarAppWeb.Services;
using CarAppWeb.Views.Auth.Models;
using Microsoft.AspNetCore.Authentication;

namespace CarAppWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        
        public IActionResult Login()
        {
            return View("Login");
        }
        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginFormViewModel loginFormViewModel)
        {
            var loginDto = new LoginDto
            {
                Username = loginFormViewModel.Username,
                Password = loginFormViewModel.Password,
            };
            
            
            var response =  await _authService.AttemptToLogin(loginDto);

            switch (response)
            {
                case ResponseState<TokenDto>.Data data:
                    return RedirectToAction("Index", "Home");
                case ResponseState<TokenDto>.Error error:
                    break;
            }
            
            return View(loginFormViewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index","Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterFormViewModel registerFormViewModel)
        {
            var registerDto = new RegisterDto
            {
                Password = registerFormViewModel.Password,
                Username = registerFormViewModel.Username
            };
            
            var response =  await _authService.AttemptToRegister(registerDto);
            
            switch (response)
            {
                case ResponseState<TokenDto>.Data:
                    return RedirectToAction("Index", "Home");
                case ResponseState<TokenDto>.Error error:
                    if (error.Message.Contains("PASSWORD_TOO_SHORT"))
                    {
                        ModelState.AddModelError(nameof(RegisterFormViewModel.Password), "Password too short");
                    }
                    break;
                default:
                    break;
            }

            var newRegisterFormViewModel = new RegisterFormViewModel
            {
                Username = registerFormViewModel.Username
            };
            
            return View(newRegisterFormViewModel);
        }
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}