using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3_2
{
    public class NotificationService  //Dependency Injection для устранения жесткой привязки
    {
        private readonly INotificationSender _notificationSender;
        private readonly ILogger _logger;

        public NotificationService(INotificationSender notificationSender, ILogger logger)
        {
            _notificationSender = notificationSender ?? throw new ArgumentNullException(nameof(notificationSender));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void SendNotification(string message, string recipient)
        {
            string formattedMessage = $"Уведомление: {message}";

            _notificationSender.Send(recipient, formattedMessage);
            _logger.Log($"Отправлено {_notificationSender.Type} уведомление для {recipient}");
        }
    }
}
