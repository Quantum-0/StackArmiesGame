using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    class Army : List<IUnit>
    {
        public int Cost
        {
            get
            {
                return this.Sum(u => u.Cost);
            }
        }

        public override string ToString()
        {
            return "Army {" + string.Join(", ", this) + "}";
        }
    }
}
