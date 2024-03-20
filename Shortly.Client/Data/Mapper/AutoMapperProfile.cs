using AutoMapper;
using Shortly.Client.Data.ViewModels;
using Shortly.Data.Models;

namespace Shortly.Client.Data.Mapper
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Url, GetUrlVM>().ReverseMap();
            CreateMap<AppUser,GetUserVM>().ReverseMap();
        }
    }
}
