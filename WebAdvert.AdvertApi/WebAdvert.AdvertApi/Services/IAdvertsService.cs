using System.Collections.Generic;
using System.Threading.Tasks;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Services
{
    public interface IAdvertsService
    {
        Task<string> CreateAsync(AdvertModel model);

        Task ConfirmAsync(ConfirmAdvertModel model);

        Task<List<AdvertModel>> GetAllAsync();

        Task<AdvertModel> GetByIdAsync(string id);

        Task<bool> CheckHealthAsync();
    }
}
