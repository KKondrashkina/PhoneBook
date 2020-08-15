using Microsoft.EntityFrameworkCore;
using PhoneBook.DataAccess.Models;
using PhoneBook.DataAccess.RepositoryInterfaces;
using System.Collections.Generic;
using System.Linq;

namespace PhoneBook.DataAccess.RepositoryClasses
{
    class ContactRepository : BaseEfRepository<Contact>, IContactRepository
    {
        public ContactRepository(DbContext db) : base(db)
        {

        }

        public void DeleteContacts(List<Contact> contacts)
        {
            DbSet.RemoveRange(contacts);
        }

        public List<Contact> GetContactsByIds(int[] ids)
        {
            return ids.Select(id => DbSet.Find(id)).ToList();
        }
    }
}
