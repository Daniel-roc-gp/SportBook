
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Services.Services
{
    public class CanchaService : ICanchaService
    {
        public readonly IBaseRepository<Cancha> _canchaRepository;
        public readonly ICanchaRepository _repository;

        public CanchaService(
            IBaseRepository<Cancha> canchaRepository,
            ICanchaRepository repository)
        {
            _canchaRepository = canchaRepository;
            _repository = repository;
        }

        public async Task<IEnumerable<Cancha>> GetAllCanchasAsync()
        {
            return await _repository.GetAllCanchasAsync();
        }

        public async Task<Cancha?> GetCanchaByIdAsync(long id)
        {
            return await _repository.GetCanchaByIdAsync(id);
        }

        public async Task<bool> InsertCancha(Cancha cancha)
        {
            if (!await _repository.TipoCanchaExisteAsync(
                cancha.IdTipoCancha))
            {
                return false;
            }

            await _canchaRepository.Add(cancha);
            return true;
        }

        public async Task<bool> UpdateCancha(Cancha cancha)
        {
            if (!await _repository.TipoCanchaExisteAsync(
                cancha.IdTipoCancha))
            {
                return false;
            }

            await _canchaRepository.Update(cancha);
            return true;
        }

        public async Task DeleteCancha(long id)
        {
            await _canchaRepository.Delete(id);
        }
    }
}
