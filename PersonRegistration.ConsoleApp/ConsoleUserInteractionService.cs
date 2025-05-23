using PersonRegistration.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistration.ConsoleApp
{
    public class ConsoleUserInteractionService : IUserInteractionService
    {
        public string GetInput(string prompt)
        {
            Console.Write(prompt + " ");
            return Console.ReadLine() ?? string.Empty;
        }
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
