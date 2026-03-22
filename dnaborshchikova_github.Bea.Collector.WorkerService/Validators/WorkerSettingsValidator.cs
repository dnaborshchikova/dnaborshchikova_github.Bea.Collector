using dnaborshchikova_github.Bea.Collector.Core.Models.Settings;
using dnaborshchikova_github.Bea.Collector.Core.Validators;

namespace dnaborshchikova_github.Bea.Collector.WorkerService.Validators
{
    public class WorkerSettingsValidator : SettingsValidatorBase
    {
        public override void ValidateProcessingSettings(ProcessingSettings processingSettings)
        {
            if (processingSettings == null)
                throw new ArgumentNullException(nameof(processingSettings));

            if (string.IsNullOrEmpty(processingSettings.FilePath) && string.IsNullOrEmpty(processingSettings.InputFolder))
                throw new InvalidOperationException("Для Worker необходимо указать FilePath или InputFolder");

            if (processingSettings.ThreadCount <= 0)
                throw new InvalidOperationException("ThreadCount должен быть > 0");

            if (string.IsNullOrEmpty(processingSettings.ProcessType)
                || (processingSettings.ProcessType != "Thread" && processingSettings.ProcessType != "Task"))
                throw new InvalidOperationException("ProcessType должен быть 'Thread' или 'Task'");

            if (string.IsNullOrEmpty(processingSettings.RunMode) 
                || (processingSettings.RunMode != "OneTime" && processingSettings.RunMode != "ScheduledService"))
                throw new InvalidOperationException("RunMode должен быть 'OneTime' или 'ScheduledService'");
        }
    }
}
