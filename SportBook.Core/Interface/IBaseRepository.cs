
using SportBook.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportBook.Core.Interfaces
{
    public interface IBaseRepository<T>
        where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById(long id);
        Task Add(T entity);
        Task Update(T entity);
        Task Delete(long id);
    }
}
