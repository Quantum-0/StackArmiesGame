using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    class HeavyUnitAttachment : HeavyWarrior
    {
        protected HeavyWarrior unit;

        public HeavyUnitAttachment(HeavyWarrior heavyWarrior)
        {
            unit = heavyWarrior;
        }

        public HeavyWarrior TakeOff()
        {
            return unit;
        }

        public override void GetHit(int strength)
        {
            if (rnd.NextDouble() < 0.25)
            {
                Army unitArmy;
                if (Engine.Instance.ArmyA.Contains(this))
                    unitArmy = Engine.Instance.ArmyA;
                else
                    unitArmy = Engine.Instance.ArmyB;

                var index = unitArmy.IndexOf(this);
                unitArmy[index] = TakeOff();
            }
        }
    }

    class HeavyUnitHelmet : HeavyUnitAttachment
    {
        public HeavyUnitHelmet(HeavyWarrior heavyWarrior) : base(heavyWarrior)
        {
        }

        public override int Armor
        {
            get
            {
                return unit.Armor + 5;
            }
        }
    }

    class HeavyUnitSpear : HeavyUnitAttachment
    {
        public HeavyUnitSpear(HeavyWarrior heavyWarrior) : base(heavyWarrior)
        {
        }

        public override int Damage
        {
            get
            {
                return unit.Damage + 7;
            }
        }
    }

    class HeavyUnitShield : HeavyUnitAttachment
    {
        public HeavyUnitShield(HeavyWarrior heavyWarrior) : base(heavyWarrior)
        {
        }

        public override int Armor
        {
            get
            {
                return unit.Armor + 7;
            }
        }
    }

    class HeavyUnitHorse : HeavyUnitAttachment
    {
        public HeavyUnitHorse(HeavyWarrior heavyWarrior) : base(heavyWarrior)
        {
        }

        public override int Armor
        {
            get
            {
                return unit.Armor + 3;
            }
        }

        public override int Damage
        {
            get
            {
                return unit.Damage + 3;
            }
        }
    }
}
