using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    interface IAbility
    {
        double Chance { get; }
        int Strength { get; }
        int Distance { get; }
        void DoAction(IEnumerable<IUnit> allies, IEnumerable<IUnit> enemies, ref List<ICommand> commands);
    }
}
