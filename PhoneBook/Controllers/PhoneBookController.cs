using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PhoneBook.BusinessLogic.Mapping;
using PhoneBook.Contracts;
using PhoneBook.DataAccess.RepositoryInterfaces;
using PhoneBook.DataAccess.Uow;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneBook.Controllers
{
    [Route("api/[action]")]
    [ApiController]
    public class PhoneBookController : ControllerBase
    {
        private readonly UnitOfWork _uow;

        public PhoneBookController(UnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public List<ContactDto> GetContacts([FromQuery]string searchString)
        {
            return _uow.GetRepository<IContactRepository>()
                .GetAll()
                .Select(MappingExtensions.ToDto)
                .Where(c => (c.Name + c.LastName).Contains(searchString ?? "", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        [HttpPost]
        public bool AddContact(ContactDto contact)
        {
            _uow.GetRepository<IContactRepository>()
                .Add(contact.ToModel());

            _uow.Save();

            return true;
        }

        [HttpPost]
        public bool DeleteContact([FromBody]int id)
        {
            var repository = _uow.GetRepository<IContactRepository>();

            if (repository.Delete(repository.GetById(id)))
            {
                _uow.Save();
                return true;
            }

            return false;
        }

        [HttpPost]
        public void DeleteContacts(int[] ids)
        {
            var repository = _uow.GetRepository<IContactRepository>();
            var contacts = repository.GetContactsByIds(ids);
            repository.DeleteContacts(contacts);

            _uow.Save();
        }

        [HttpGet]
        public FileResult DownloadFile()
        {
            var contacts = _uow.GetRepository<IContactRepository>()
                .GetAll()
                .Select(MappingExtensions.ToDto)
                .ToList();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var excelPackage = new ExcelPackage();

            var worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
            worksheet.Cells["A1"].LoadFromCollection(contacts, true);
            worksheet.Row(1).Style.Font.Bold = true;

            var cells = worksheet.Cells[worksheet.Dimension.Address];
            cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            cells.AutoFitColumns();

            var fileName = $"ContactsList-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

            return File(excelPackage.GetAsByteArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}