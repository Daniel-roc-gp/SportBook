
using Microsoft.EntityFrameworkCore;
using SportBook.Core.Entities;
using SportBook.Core.Interfaces;
using SportBook.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T>
        where T : BaseEntity
    {
        private readonly SportBookDbContext _context; //Base de datos
        private readonly DbSet<T> _entities; //Tablas

        public BaseRepository(SportBookDbContext context)
        {
            _context = context;
            _entities = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _entities.ToListAsync();
        }

        public async Task<T> GetById(long id)
        {
            return await _entities.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            _entities.Add(entity); //Insert
            await _context.SaveChangesAsync(); //Commit
        }

        public async Task Update(T entity)
        {
            _entities.Update(entity);
            await _context.SaveChangesAsync(); //Commit
        }

        public async Task Delete(long id)
        {
            T entity = await GetById(id);
            _entities.Remove(entity);
            await _context.SaveChangesAsync(); //Commit
        }
    }
}
