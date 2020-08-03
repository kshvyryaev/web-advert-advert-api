using System.Threading.Tasks;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Services
{
    public interface IAdvertsService
    {
        Task<string> Create(AdvertModel model);

        Task Confirm(ConfirmAdvertModel model);
    }
}
