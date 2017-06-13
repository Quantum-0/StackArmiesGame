using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    interface IStrategy
    {
        String Name { get; }
        List<Tuple<IUnit, IUnit>> GetNearest(Army first, Army second);
        Tuple<IEnumerable<IUnit>, IEnumerable<IUnit>> GetRanged(IUnit unit, Army from, Army to);
    }

    class OneVsOne : IStrategy
    {
        public string Name
        {
            get
            {
                return "1 vs 1";
            }
        }

        public Tuple<IEnumerable<IUnit>,IEnumerable<IUnit>> GetRanged(IUnit unit, Army from, Army to)
        {
            var range = (unit as IAbility).Distance;

            int pos = from.IndexOf(unit);
            int alliesFrom = Math.Max(0, pos - range);
            int alliesTo = Math.Min(from.Count - 1, pos + range);
            int enemiesTo = Math.Min(to.Count - 1, range - pos - 1);
            List<IUnit> unitsA = new List<IUnit>(range * 2);
            List<IUnit> unitsE = new List<IUnit>(range * 2);
            for (int j = alliesFrom; j <= alliesTo; j++)
            {
                if (j == pos)
                    continue;
                unitsA.Add(from[j]);
            }
            for (int j = 0; j <= enemiesTo; j++)
                unitsE.Add(to[j]);
            
            return new Tuple<IEnumerable<IUnit>, IEnumerable<IUnit>>(unitsA, unitsE);
        }

        public List<Tuple<IUnit, IUnit>> GetNearest(Army first, Army second)
        {
            var list = new List<Tuple<IUnit, IUnit>>();
            list.Add(new Tuple<IUnit, IUnit>(first.First(), second.First()));
            return list;
        }
    }

    class ThreeVsThree : IStrategy
    {
        private const int n = 3;

        public string Name
        {
            get
            {
                return "3 vs 3";
            }
        }

        public Tuple<IEnumerable<IUnit>, IEnumerable<IUnit>> GetRanged(IUnit unit, Army from, Army to)
        {
            int unitX, unitY;

            // For Allies
            unitX = from.IndexOf(unit) % n;
            unitY = from.IndexOf(unit) / n;

            var unitsA = GetUnitsInRadius(unitX, unitY, ((IAbility)unit).Distance, from);

            // For Enemies
            //unitX = Math.Abs(from.IndexOf(unit) % n - (n - 1)); // заменить
            unitX = from.IndexOf(unit) % n; // перевернул систему координат так, чтоб она была зеркальным отражением системы For Allies
            unitY = -from.IndexOf(unit) / n - 1;

            var unitsE = GetUnitsInRadius(unitX, unitY, ((IAbility)unit).Distance, to);

            return new Tuple<IEnumerable<IUnit>, IEnumerable<IUnit>>(unitsA, unitsE);
        }

        private IEnumerable<IUnit> GetUnitsInRadius(int x, int y, int r, Army army)
        {
            for (int i = 0; i < army.Count; i++)
            {
                var x2 = i % n;
                var y2 = i / n;

                int r2 = GetDistance(x, y, x2, y2);

                if (r2 <= r)
                    yield return army[i];
            }
            yield break;
        }

        private int GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Abs(x1 - x2) + Math.Abs(y1 - y2); // не стандартная метрика для R2
        }

        public List<Tuple<IUnit, IUnit>> GetNearest(Army first, Army second)
        {
            var count = Math.Min(n, Math.Min(first.Count, second.Count));

            var list = new List<Tuple<IUnit, IUnit>>();

            for (int i = 0; i < count; i++)
                list.Add(new Tuple<IUnit, IUnit>(first[i], second[i]));

            return list;
        }
    }

    class AllVsAll : IStrategy
    {
        public string Name
        {
            get
            {
                return "All vs All";
            }
        }

        public Tuple<IEnumerable<IUnit>, IEnumerable<IUnit>> GetRanged(IUnit unit, Army from, Army to)
        {
            int pos = from.IndexOf(unit);
            int range = (unit as IAbility).Distance;
            int indexFrom = Math.Max(0, pos - range);
            int indexTo = Math.Min(from.Count - 1, pos + range);

            var unitsA = from.Skip(indexFrom).Take(indexTo - indexFrom);
            
            indexFrom = Math.Max(0, pos - range + 1);
            indexTo = Math.Min(to.Count - 1, pos + range - 1);

            var unitsE = from.Skip(indexFrom).Take(indexTo - indexFrom);

            return new Tuple<IEnumerable<IUnit>, IEnumerable<IUnit>>(unitsA, unitsE);
        }

        public List<Tuple<IUnit, IUnit>> GetNearest(Army first, Army second)
        {
            var count = Math.Min(first.Count, second.Count);

            var list = new List<Tuple<IUnit, IUnit>>();

            for (int i = 0; i < count; i++)
                list.Add(new Tuple<IUnit, IUnit>(first[i], second[i]));

            return list;
        }
    }
}
