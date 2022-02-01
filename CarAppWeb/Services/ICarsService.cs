using System.Threading.Tasks;
using CarAppWeb.Dtos;
using CarAppWeb.Http;

namespace CarAppWeb.Services
{
    public interface ICarsService
    {
        public Task<ResponseState<CarDto>> GetSingleCar(int id);
        public Task<ResponseState<CarAppPagedModel<CarDto>>> GetCarsPaged(int page);
    }
}