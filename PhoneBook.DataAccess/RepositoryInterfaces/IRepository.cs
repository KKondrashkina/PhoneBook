using System.Collections.Generic;

namespace PhoneBook.DataAccess.RepositoryInterfaces
{
    public interface IRepository
    {

    }

    public interface IRepository<T> : IRepository where T : class
    {
        void Add(T entity);

        void AddRange(List<T> entities);

        void Update(T entity);

        bool Delete(T entity);

        List<T> GetAll();

        T GetById(int id);
    }
}
