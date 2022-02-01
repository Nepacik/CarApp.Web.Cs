using CarAppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CarAppWeb.Dtos;
using CarAppWeb.Http;
using CarAppWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CarAppWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICarsService _carsService;

        public HomeController(ILogger<HomeController> logger, ICarsService carsService)
        {
            _logger = logger;
            _carsService = carsService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Cars(int id = 1)
        {
            CarAppPagedModel<CarDto> listOfCars = new CarAppPagedModel<CarDto>();
            var response = await _carsService.GetCarsPaged(id);

            if (response is ResponseState<CarAppPagedModel<CarDto>>.Data data)
            {
                listOfCars = data.Content;
            }

            if (id == 1)
            {
                return View(listOfCars);
            }
            else
            {
                return PartialView(listOfCars);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddModel()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
