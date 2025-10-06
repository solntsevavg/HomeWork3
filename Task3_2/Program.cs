
using Microsoft.Extensions.DependencyInjection;
namespace Task3_2
{
    class Program
    {
        static void Main()
        {
            try
            {
                // Запрашиваем у пользователя тип уведомления
                Console.WriteLine("Выберите тип уведомления:");
                Console.WriteLine("1 - Email");
                Console.WriteLine("2 - SMS");
                Console.Write("Ваш выбор (1 или 2): ");

                var choice = Console.ReadLine();
                if (string.IsNullOrEmpty(choice) ||
               (choice != "1" && choice != "2"))
                {
                    throw new ArgumentException("Неверный выбор. Введите 1 или 2");
                }
                string notificationType = choice == "2" ? "sms" : "email";

                // Настраиваем IoC-контейнер
                using var serviceProvider = ServiceConfigurator.ConfigureServices(notificationType);

                // Получаем сервис из контейнера
                var notificationService = serviceProvider.GetRequiredService<NotificationService>();

                // Данные для отправки
                Console.Write("\n Введите сообщение: ");
                string message = Console.ReadLine();

                Console.Write("Введите получателя: ");
                string recipient = Console.ReadLine();

                
                // Отправляем уведомление
                notificationService.SendNotification(message, recipient);

                Console.WriteLine("\n Уведомление отправлено успешно!");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка ввода: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }


            Console.WriteLine("\n Нажмите любую клавишу для выхода...");
            Console.ReadKey();

        }
    }
}
