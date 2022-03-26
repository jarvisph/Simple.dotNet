using System;
using System.Collections.Generic;
using System.Text;

namespace Simple.dotNet.Core.Helper
{
    public static class ConsoleHelper
    {
        public static object _lock = new object();

        public static string WriteLine(string message, ConsoleColor color)
        {
            lock (_lock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            return message;
        }

        public static string Write(string message, ConsoleColor color)
        {
            lock (_lock)
            {
                Console.ForegroundColor = color;
                Console.Write(message);
                Console.ResetColor();
            }
            return message;
        }
        public static void OverWrite(string context)
        {
            lock (_lock)
            {
                Console.WriteLine(context); Console.SetCursorPosition(0, Console.CursorTop - 1);
            }

        }
    }
}
