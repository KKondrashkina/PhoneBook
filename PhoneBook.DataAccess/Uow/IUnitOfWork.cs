using System;
using PhoneBook.DataAccess.RepositoryInterfaces;

namespace PhoneBook.DataAccess.Uow
{
    interface IUnitOfWork : IDisposable
    {
        void Save();

        T GetRepository<T>() where T : class, IRepository;

        void BeginTransaction();

        void RollbackTransaction();
    }
}
