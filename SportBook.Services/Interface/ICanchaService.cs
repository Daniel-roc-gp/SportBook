using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Services.Interfaces
{
    public interface ICanchaService
    {
        Task<IEnumerable<Cancha>> GetAllCanchasAsync();
        Task<Cancha?> GetCanchaByIdAsync(long id);
        Task<bool> InsertCancha(Cancha cancha);
        Task<bool> UpdateCancha(Cancha cancha);
        Task DeleteCancha(long id);
    }
}
