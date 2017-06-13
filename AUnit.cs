using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    abstract class AUnit : IUnit, IObservable
    {
        protected List<IObserver> observers = new List<IObserver>();
        protected Random rnd = new Random();
        public abstract int Armor { get; }
        public abstract int Cost { get; }
        public abstract int Damage { get; }
        protected abstract int _Health { get; set; }

        public int Health
        {
            get
            {
                return _Health;
            }
            protected set
            {
                _Health = value;
            }
        }

        private static IEnumerable<Type> UnitTypes;
        public static IEnumerable<Type> GetAllUnitTypes()
        {
            if (UnitTypes == null)
            {
                Type iunit = typeof(IUnit); ;
                UnitTypes = System.Reflection.Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t => t != iunit && t.IsClass && !t.IsAbstract && iunit.IsAssignableFrom(t) && !t.IsSubclassOf(typeof(HeavyWarrior)));
            }

            return UnitTypes;
        }

        public virtual void GetHit(int strength)
        {
            // Маленький костыль для observer'ов
            var healthBefore = _Health;

            _Health = Math.Max(0, _Health - strength);

            if (_Health <= 0 && healthBefore > 0)
                observers.ForEach(o => o.Update(this, _Health));
        }

        public override string ToString()
        {
            return this.GetType().Name + "[" + _Health + "HP]";
        }

        public void Subscribe(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            observers.Remove(observer);
        }
    }
}
