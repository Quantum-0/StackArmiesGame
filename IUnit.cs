using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    interface IUnit
    {
        int Health { get; }
        int Damage { get; }
        int Cost { get; }
        int Armor { get; }
        void GetHit(int strength);
        string ToString();
    }
}
