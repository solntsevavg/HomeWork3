using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
namespace Task3_2
{
    public static class ServiceConfigurator
    {
        public static ServiceProvider ConfigureServices(string notificationType)
        {
            var services = new ServiceCollection();

            // Регистрируем логгер
            services.AddSingleton<ILogger, ConsoleLogger>();

            // Регистрируем отправителя в зависимости от выбора
            switch (notificationType.ToLower())
            {
                case "email":
                    services.AddSingleton<INotificationSender, EmailSender>();
                    break;
                case "sms":
                    services.AddSingleton<INotificationSender, SmsSender>();
                    break;
                default:
                    throw new ArgumentException("Неизвестный тип уведомления");
            }

            // Регистрируем основной сервис
            services.AddSingleton<NotificationService>();

            return services.BuildServiceProvider();
        }
    }
}
