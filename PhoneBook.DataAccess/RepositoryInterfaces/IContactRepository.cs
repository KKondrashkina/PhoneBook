using System.Collections.Generic;
using PhoneBook.DataAccess.Models;

namespace PhoneBook.DataAccess.RepositoryInterfaces
{
    public interface IContactRepository : IRepository<Contact>
    {
        List<Contact> GetContactsByIds(int[] ids);

        void DeleteContacts(List<Contact> contacts);
    }
}
