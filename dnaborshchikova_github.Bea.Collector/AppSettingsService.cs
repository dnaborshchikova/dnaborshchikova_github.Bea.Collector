using dnaborshchikova_github.Bea.Collector.Core.Models;
using dnaborshchikova_github.Bea.Generator;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dnaborshchikova_github.Bea.Collector.App
{
    public class AppSettingsService
    {
        private readonly IConfiguration _configuration;

        public AppSettingsService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AppSettings CreateAppSettings()
        {
            var errors = new List<string>();

            var isGenerateFile = false;
            if (!bool.TryParse(_configuration["GenerateFile"], out isGenerateFile))
                errors.Add($"Не задан параметр isGenerateFile.");
            
            var threadCount = 0;
            if (!int.TryParse(_configuration["ThreadCount"], out threadCount))
                errors.Add($"Не задан параметр ThreadCount.");

            var processType = _configuration["ProcessType"];
            if (string.IsNullOrEmpty(processType))
                errors.Add("Не указаан способ обрабоки.");

            if (errors.Count > 0)
            {
                throw new Exception($"Конфигурационный файл не настроен." +
                    $"Список ошибок:\n{string.Concat("\n", errors)}\n");
            }

            var filePath = string.Empty;
            var fileFormat = string.Empty;
            var isRunAsProcess = false;
            var paidBillEventCount = 0;
            var cancelledBillEventCount = 0;

            if (!isGenerateFile)
            {
                filePath = _configuration["FilePath"];
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("Конфигурационный файл не настроен." +
                        "Не указан путь к файлу. Укажите путь или поставьте значение false для параметра GenerateFile.");
            }
            else
            {
                if (!bool.TryParse(_configuration["RunAsProcess"], out isRunAsProcess))
                    throw new Exception($"Конфигурационный файл не настроен. Не задан параметр RunAsProcess.");

                if (!isRunAsProcess)
                {
                    errors = new List<string>();
                    if (!int.TryParse(_configuration["PaidBillEventCount"], out paidBillEventCount))
                        errors.Add("PaidBillEventCount");

                    if (!int.TryParse(_configuration["CancelledBillEventCount"], out cancelledBillEventCount))
                        errors.Add("CancelledBillEventCount");

                    fileFormat = _configuration["FileFormat"];
                    if (string.IsNullOrWhiteSpace(fileFormat))
                        errors.Add("FileFormat");

                    if (errors.Count > 0)
                        throw new Exception($"Конфигурационный файл не настроен.\nНе заданы параметры:\n {string.Join("\n", errors)}");

                }
            }

            var appsettings = new AppSettings(isRunAsProcess, isGenerateFile, filePath, fileFormat,
                paidBillEventCount, cancelledBillEventCount, threadCount, processType);
            return appsettings;
        }
    }
}
