using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CarAppWeb.Dtos;
using CarAppWeb.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace CarAppWeb.Services
{
    public class CarsService : BaseService, ICarsService
    {
        private const string serviceUrl = "Car/";

        public CarsService(IConfiguration configuration, HttpClient httpClient,
            IHttpContextAccessor httpContextAccessor) :
            base(configuration, httpClient, serviceUrl, httpContextAccessor)
        {
        }

        public async Task<ResponseState<CarDto>> GetSingleCar(int id)
        {
            var httpServiceData = new HttpServiceData("Login", HttpMethod.Get);
            var queryString = new Dictionary<string, string>()
            {
                {"id", id.ToString()}
            };
            var result = await HandleQueryRequest<CarDto>(httpServiceData, queryString);
            return result;
        }

        public async Task<ResponseState<CarAppPagedModel<CarDto>>> GetCarsPaged(int page)
        {
            var httpServiceData = new HttpServiceData("GetAllCars", HttpMethod.Get);
            var queryString = new Dictionary<string, string>()
            {
                {"page", page.ToString()}
            };
            var result = await HandleQueryRequest<CarAppPagedModel<CarDto>>(httpServiceData, queryString);
            return result;
        }
    }
}