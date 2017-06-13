using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Armor 3
 * Cost 35
 * Damage 5
 * Health 20
 */
namespace StackArmyGame
{
    class Wizard : AUnit, IAbility, IClonable, IHealable
    {
        public override int Armor
        {
            get
            {
                return 3;
            }
        }

        public override int Cost
        {
            get
            {
                return 5;
            }
        }

        public override int Damage
        {
            get
            {
                return 5;
            }
        }

        protected override int _Health { get; set; } = 20;
        public int MaxHealth { get; } = 20;

        public double Chance
        {
            get
            {
                return 0.05;
            }
        }

        public int Strength
        {
            get
            {
                return 0;
            }
        }

        public int Distance
        {
            get
            {
                return 1;
            }
        }

        public void DoAction(IEnumerable<IUnit> allies, IEnumerable<IUnit> enemies, ref List<ICommand> commands)
        {
            if (rnd.NextDouble() > Chance)
                return;
            
            var clonable = allies
                .Where(u => u.GetType().GetInterfaces().Contains(typeof(IClonable)))
                .Cast<IClonable>().ToArray();

            if (clonable.Length == 0)
                return;
            var unit = clonable[rnd.Next(clonable.Length)].Clone();

            Army myArmy;
            if (Engine.Instance.ArmyA.Contains(this))
                myArmy = Engine.Instance.ArmyA;
            else
                myArmy = Engine.Instance.ArmyB;

            var pos = myArmy.FindIndex(u => u.Equals(this));
            //myArmy.Insert(pos, unit);
            var cmd = new CloneCommand(myArmy, pos, unit);
            cmd.Do();
            commands.Add(cmd);
            CUI.Log(this + " клонировал " + unit.ToString());
        }

        public IUnit Clone()
        {
            return (IUnit)this.MemberwiseClone();
        }

        public void Heal(int value)
        {
            Health = Math.Min(Health + value, MaxHealth);
        }
    }
}
