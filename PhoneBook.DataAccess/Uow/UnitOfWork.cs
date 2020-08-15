using System;
using Microsoft.EntityFrameworkCore;
using PhoneBook.DataAccess.RepositoryClasses;
using PhoneBook.DataAccess.RepositoryInterfaces;

namespace PhoneBook.DataAccess.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _db;

        private bool _isTransactionExist;

        public UnitOfWork(DbContext db)
        {
            _db = db;
        }

        public virtual void BeginTransaction()
        {
            _isTransactionExist = true;
            _db.Database.BeginTransaction();
        }

        public virtual void RollbackTransaction()
        {
            _isTransactionExist = false;
            _db.Database.RollbackTransaction();
        }

        public void Save()
        {
            if (_isTransactionExist)
            {
                _db.Database.CommitTransaction();
            }

            _db.SaveChanges();
        }

        public void Dispose()
        {
            if (_isTransactionExist)
            {
                RollbackTransaction();
            }

            _db.Dispose();
        }

        public T GetRepository<T>() where T : class, IRepository
        {
            if (typeof(T) == typeof(IContactRepository))
            {
                return new ContactRepository(_db) as T;
            }

            throw new Exception("Неизвестный тип репозитория: " + typeof(T));
        }
    }
}
