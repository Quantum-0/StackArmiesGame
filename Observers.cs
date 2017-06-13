using System;
using System.IO;

namespace StackArmyGame
{
    internal class BeepObserver : IObserver
    {
        public void Update(IObservable sender, int health)
        {
            Console.Beep();
        }
    }

    internal class FileLoggerObserver : IObserver
    {
        public void Update(IObservable sender, int health)
        {
            File.AppendAllText("log.txt", sender.GetType().Name + " health is " + ((AUnit)sender).Health);
        }
    }
}