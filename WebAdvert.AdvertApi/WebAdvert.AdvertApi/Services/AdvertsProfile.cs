using AutoMapper;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Services
{
    public class AdvertsProfile : Profile
    {
        public AdvertsProfile()
        {
            CreateMap<AdvertModel, AdvertDbModel>().ReverseMap();
        }
    }
}
