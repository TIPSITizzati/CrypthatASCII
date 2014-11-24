using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypthat_Common
{
    // Classe per il Debug
    public class Debug
    {
        public enum LogType
        {
            ERROR = ConsoleColor.Red,
            INFO = ConsoleColor.White,
            WARNING = ConsoleColor.DarkYellow
        }

        // Scrive con colori diversi nella console
        // Utilizza la Reflection per ricavare nome del metodo, tipo di metodo (costruttore o metodo) e classe di appartenenza
        public static void Log(object Message, LogType type = LogType.INFO)
        {
            //Stack delle chiamate
            StackTrace stackTrace = new StackTrace();

            ConsoleColor CurrentColor = Console.ForegroundColor;
            Console.ForegroundColor = (ConsoleColor)type;

            Console.WriteLine("[{0}]{1} {2}: {3}", DateTime.Now.ToShortTimeString(), Enum.GetName(typeof(LogType), type),
                "(" + stackTrace.GetFrame(1).GetMethod().DeclaringType.Name + ") " + stackTrace.GetFrame(1).GetMethod().MemberType.ToString() + " " + stackTrace.GetFrame(1).GetMethod().Name, Message.ToString());

            Console.ForegroundColor = CurrentColor;
        }
    }
}
