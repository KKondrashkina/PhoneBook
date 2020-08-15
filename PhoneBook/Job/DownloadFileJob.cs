using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PhoneBook.BusinessLogic.Mapping;
using PhoneBook.DataAccess.RepositoryInterfaces;
using PhoneBook.DataAccess.Uow;

namespace PhoneBook.Job
{
    public class DownloadFileJob : IHostedService, IDisposable
    {
        private readonly ILogger<DownloadFileJob> _logger;
        private readonly DownloadFileOptions _options;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;

        public DownloadFileJob(ILogger<DownloadFileJob> logger, IOptions<DownloadFileOptions> optionsAccessor, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _options = optionsAccessor.Value;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var uow = scope.ServiceProvider.GetRequiredService<UnitOfWork>();

                var contacts = uow.GetRepository<IContactRepository>()
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

                var filePath = _options.FilePath + $"ContactsList-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

                excelPackage.SaveAs(new FileInfo(filePath));
            }

            _logger.LogInformation(
                "Timed Hosted Service is working.");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
