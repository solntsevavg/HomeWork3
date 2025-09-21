using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3_2
{
    public interface INotificationSender //Интерфейс INotificationSender и реализацию EmailSender
    {
        void Send(string recipient, string message);
        string Type { get; }
    }

    public class EmailSender : INotificationSender
    {
        public string Type => "Email";
        public void Send(string recipient, string message)
        {
            Console.WriteLine($"Email для {recipient}: {message}");
        }
    }
    public class SmsSender : INotificationSender //Новая реализация SmsSender без изменения существующего кода
    {
        public string Type => "SMS";
        public void Send(string recipient, string message)
        {
            Console.WriteLine($"SMS для {recipient}: {message}");
        }
    }
}
