using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Armor 2
 * Cost 32
 * Damage 6
 * Health 20
 */
namespace StackArmyGame
{
    class Cleric : AUnit, IAbility, IClonable, IHealable
    {
        public override int Armor
        {
            get
            {
                return 2;
            }
        }

        public override  int Cost
        {
            get
            {
                return 32;
            }
        }

        public override int Damage
        {
            get
            {
                return 6;
            }
        }

        public int Distance
        {
            get
            {
                return 2;
            }
        }

        protected override int _Health { get; set; } = 20;
        public int MaxHealth { get; } = 20;

        public double Chance
        {
            get
            {
                return 0.25;
            }
        }

        public int Strength
        {
            get
            {
                return 15;
            }
        }

        public void DoAction(IEnumerable<IUnit> allies, IEnumerable<IUnit> enemies, ref List<ICommand> commands)
        {
            if (rnd.NextDouble() > Chance)
                return;

            var healable = allies
                .Where(u => u.GetType().GetInterfaces().Contains(typeof(IHealable)))
                /*.Cast<IHealable>()*/.ToArray();
            if (healable.Length < 1)
                return;

            var target = healable[rnd.Next(healable.Length)];
            //target.Heal(Strength);

            Army myArmy;
            if (Engine.Instance.ArmyA.Contains(this))
                myArmy = Engine.Instance.ArmyA;
            else
                myArmy = Engine.Instance.ArmyB;

            var cmd = new AddHealthCommand(myArmy, myArmy.IndexOf(target), Strength);
            cmd.Do();
            commands.Add(cmd);

            CUI.Log(this.ToString() + " вылечил " + target);
        }

        public void Heal(int value)
        {
            Health = Math.Min(Health + value, MaxHealth);
        }

        public IUnit Clone()
        {
            return (IUnit)this.MemberwiseClone();
        }
    }
}
