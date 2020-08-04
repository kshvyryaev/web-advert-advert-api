using System.Threading.Tasks;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Services
{
    public interface IAdvertsService
    {
        Task<string> CreateAsync(AdvertModel model);

        Task ConfirmAsync(ConfirmAdvertModel model);

        Task<bool> CheckHealthAsync();
    }
}
