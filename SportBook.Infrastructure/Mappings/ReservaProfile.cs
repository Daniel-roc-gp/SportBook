
using AutoMapper;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;

namespace SportBook.Infrastructure.Mappings
{
    public class ReservaProfile : Profile
    {
        public ReservaProfile()
        {
            CreateMap<Reserva, ReservaDto>();

            CreateMap<CrearReservaDto, Reserva>()
                .ForMember(dest => dest.IdReserva,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Codigo,
                    opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IdCanchaNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IdEstadoNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IdHorarioNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuarioNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.AuditoriaReservas,
                    opt => opt.Ignore());

            CreateMap<ReservaDto, Reserva>()
                .ForMember(dest => dest.IdReserva,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Codigo,
                    opt => opt.Ignore())
                .ForMember(dest => dest.FechaRegistro,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IdCanchaNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IdEstadoNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IdHorarioNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuarioNavigation,
                    opt => opt.Ignore())
                .ForMember(dest => dest.AuditoriaReservas,
                    opt => opt.Ignore());
        }
    }
}
