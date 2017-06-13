using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Armor 10
 * Cost 20
 * Damage 20
 * Health 45
 */
namespace StackArmyGame
{
    class HeavyWarrior : AUnit, IClonable
    {
        public override int Armor
        {
            get
            {
                return 10;
            }
        }

        public override int Cost
        {
            get
            {
                return 20;
            }
        }

        public override int Damage
        {
            get
            {
                return 20;
            }
        }

        protected override int _Health { get; set; } = 45;

        public IUnit Clone()
        {
            return (IUnit)this.MemberwiseClone();
        }
    }
}
