
using Microsoft.EntityFrameworkCore;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportBook.Infrastructure.Repositories
{
    public class CanchaRepository : ICanchaRepository
    {
        private readonly SportBookDbContext _sportBookContext;

        public CanchaRepository(SportBookDbContext sportBookContext)
        {
            _sportBookContext = sportBookContext;
        }

        public async Task<IEnumerable<Cancha>> GetAllCanchasAsync()
        {
            return await _sportBookContext.Canchas
                .Where(x => x.Activa == true)
                .OrderBy(x => x.Nombre)
                .ToListAsync();
        }

        public async Task<Cancha?> GetCanchaByIdAsync(long id)
        {
            var cancha = await _sportBookContext.Canchas
                .FirstOrDefaultAsync(x => x.IdCancha == id);

            return cancha;
        }

        public async Task InsertCancha(Cancha cancha)
        {
            _sportBookContext.Canchas.Add(cancha);
            await _sportBookContext.SaveChangesAsync();
        }

        public async Task UpdateCancha(Cancha cancha)
        {
            _sportBookContext.Canchas.Update(cancha);
            await _sportBookContext.SaveChangesAsync();
        }

        public async Task DeleteCancha(Cancha cancha)
        {
            _sportBookContext.Canchas.Remove(cancha);
            await _sportBookContext.SaveChangesAsync();
        }

        public async Task<bool> TipoCanchaExisteAsync(long idTipoCancha)
        {
            return await _sportBookContext.TipoCanchas
                .AnyAsync(x => x.IdTipoCancha == idTipoCancha);
        }
    }
}
