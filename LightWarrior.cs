using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Armor 5
 * Cost 16
 * Damage 16
 * Health 35
 */
namespace StackArmyGame
{
    class LightWarrior : AUnit, IClonable, IHealable, IAbility
    {
        //private List<IObserver> Observers = new List<IObserver>();

        public override int Armor
        {
            get
            {
                return 5;
            }
        }

        public override int Cost
        {
            get
            {
                return 16;
            }
        }

        public override int Damage
        {
            get
            {
                return 16;
            }
        }

        protected override int _Health { get; set; } = 35;
        public int MaxHealth { get; } = 35;

        public double Chance
        {
            get
            {
                return 0.2;
            }
        }

        public int Strength
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Distance
        {
            get
            {
                return 1;
            }
        }

        /*public void Subscribe(IObserver observer)
        {
            Observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            Observers.Remove(observer);
        }*/

        public IUnit Clone()
        {
            return (IUnit)this.MemberwiseClone();
        }

        public void Heal(int value)
        {
            Health = Math.Min(Health + value, MaxHealth);
        }

        public void DoAction(IEnumerable<IUnit> allies, IEnumerable<IUnit> enemies, ref List<ICommand> commands)
        {
            if (rnd.NextDouble() > Chance)
                return;

            var heavy = allies.Where(a => a is HeavyWarrior).OrderBy(a => rnd.Next()).FirstOrDefault();
            
            if (heavy != null)
            {
                Army myArmy;
                if (Engine.Instance.ArmyA.Contains(this))
                    myArmy = Engine.Instance.ArmyA;
                else
                    myArmy = Engine.Instance.ArmyB;

                var index = myArmy.IndexOf(heavy);
                var Attachment = System.Reflection.Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t != typeof(HeavyUnitAttachment) && t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(HeavyUnitAttachment)))
                    .OrderBy(t => rnd.Next())
                    .FirstOrDefault();
                CUI.Log((IUnit)this + " надел " + Attachment.GetType().Name + " на " + myArmy[index]);
                var cmd = new ClotheCommand(myArmy, index, Attachment);
                cmd.Do();
                commands.Add(cmd);
                //myArmy[index] = (HeavyUnitAttachment)Activator.CreateInstance(Attachment, heavy);
            }
        }
    }
}
