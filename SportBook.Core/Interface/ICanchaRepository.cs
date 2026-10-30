
using SportBook.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportBook.Core.Interfaces
{
    public interface ICanchaRepository
    {
        Task<IEnumerable<Cancha>> GetAllCanchasAsync();

        Task<Cancha?> GetCanchaByIdAsync(long id);

        Task InsertCancha(Cancha cancha);

        Task UpdateCancha(Cancha cancha);

        Task DeleteCancha(Cancha cancha);

        Task<bool> TipoCanchaExisteAsync(long idTipoCancha);
    }
}
