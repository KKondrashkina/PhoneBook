using PhoneBook.Contracts;
using PhoneBook.DataAccess.Models;

namespace PhoneBook.BusinessLogic.Mapping
{
    public static class MappingExtensions
    {
        public static ContactDto ToDto(this Contact contact)
        {
            return new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                LastName = contact.LastName,
                PhoneNumber = contact.PhoneNumber
            };
        }

        public static Contact ToModel(this ContactDto contact)
        {
            return new Contact
            {
                Id = contact.Id,
                Name = contact.Name,
                LastName = contact.LastName,
                PhoneNumber = contact.PhoneNumber
            };
        }
    }
}
