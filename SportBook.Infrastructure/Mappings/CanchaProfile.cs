using AutoMapper;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;

namespace SportBook.Infrastructure.Mappings
{
    public class CanchaProfile : Profile
    {
        public CanchaProfile()
        {
            CreateMap<Cancha, CanchaDto>();

            CreateMap<CanchaDto, Cancha>()
                .ForMember(dest => dest.IdTipoCanchaNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Reservas,
                    opt => opt.Ignore());
        }
    }
}