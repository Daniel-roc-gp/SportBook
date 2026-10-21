using AutoMapper;
using SportBook.Core.DTOs;
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportBook.Infrastructure.Mappings
{
    public class RolProfile : Profile
    {
        public RolProfile()
        {
            CreateMap<Rol, RolDto>();
        }
    }
}