using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    class ConsoleObserver : IObserver
    {
        public void Update(IObservable sender, int health)
        {
            CUI.Log($"OBSERVER: {sender as AUnit} have {health}hp after hit");
        }
    }
}
