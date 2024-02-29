using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
        public interface IGenericRepository<T> where T : class
        {
            //Get object by id
            T GetById(int? id);
            //Get
            T Get(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null);
            //Same as get but async
            Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool trackChanges = false, string? includes = null);

            IEnumerable<T> GetAll(Expression<Func<T, bool>>? predicate = null, Expression<Func<T, int>>? orderBy = null, string? includes = null);

            Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int>>? orderBy = null, string? includes = null);

            void Add(T entity);

            void Delete(T entity);
            //Delete a range of items in an object
            void Delete(IEnumerable<T> entities);

            void Update(T entity);
            // Increment and Decrement Shopping Cart
           
        
    }
}
