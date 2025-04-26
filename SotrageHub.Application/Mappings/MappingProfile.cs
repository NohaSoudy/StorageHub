using AutoMapper;
using StorageHub.Domain;
using StorageHub.Infrastructure;
namespace SotrageHub.Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<FileHub, FileHubDTO>().ReverseMap();
            CreateMap<FileHubDTO, FileHub>().ReverseMap();
        }
    }
}



