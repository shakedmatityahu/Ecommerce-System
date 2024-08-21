using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketBackend.Services.Interfaces
{
    public interface IRepository<T>
{
        public T GetById(int id);
        public void Add(T entity);
        public IEnumerable<T> getAll();
        public void Update(T entity);
        public void Delete(T entity);
}
        

}