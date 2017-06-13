using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Armor 4
 * Cost 35
 * Damage 5
 * RangedAttack 10
 * Health 30
 */
namespace StackArmyGame
{
    class Archer : AUnit, IAbility, IClonable, IHealable
    {
        public override int Armor
        {
            get
            {
                return 4;
            }
        }

        public override int Cost
        {
            get
            {
                return 35;
            }
        }
    
        public override int Damage
        {
            get
            {
                return 5;
            }
        }

        public int Distance
        {
            get
            {
                return 10;
            }
        }

        public int Strength
        {
            get
            {
                return 10;
            }
        }

        public double Chance
        {
            get
            {
                return 0.05;
            }
        }

        protected override int _Health { get; set; } = 40;
        public int MaxHealth { get; } = 40;

        public virtual void DoAction(IEnumerable<IUnit> allies, IEnumerable<IUnit> enemies, ref List<ICommand> commands)
        {
            IUnit target;
            if (rnd.NextDouble() < Chance)
            {
                if (allies.Count() == 0)
                    return;
                target = allies.ElementAt(rnd.Next(allies.Count()));
            }
            else
            {
                if (enemies.Count() == 0)
                    return;
                target = enemies.ElementAt(rnd.Next(enemies.Count()));
            }

            var before = target.Health;
            Army army;
            if (Engine.Instance.ArmyA.Contains(target))
                army = Engine.Instance.ArmyA;
            else
                army = Engine.Instance.ArmyB;

            var cmd = new AddHealthCommand(army, army.IndexOf(target), rnd.Next(target.Armor) - Strength);
            cmd.Do();
            commands.Add(cmd);

            //CUI.Log(this.ToString() + " попал в " + target.ToString() + ", отняв " + (before - target.Health) + "hp");
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
