using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task3_2
{
    public interface ILogger //Логирование вынесено отдельно
    {
        void Log(string message);
    }
    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine($" LOG: {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }
}
