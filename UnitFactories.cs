using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    class RandomUnitFactory : IUnitFactory
    {
        Random rnd = new Random();
        public IUnit CreateUnit()
        {
            var types = AUnit.GetAllUnitTypes();
            var id = rnd.Next(types.Count());
            return (IUnit)Activator.CreateInstance(types.ElementAt(id));
        }
    }

    class LightUnitFactory : IUnitFactory
    {
        public IUnit CreateUnit()
        {
            return new LightWarrior();
        }
    }

    class  AllInOrderUnitFactory : IUnitFactory
    {
        int pos = 0;
        Type[] UnitTypes;
        public AllInOrderUnitFactory(bool randomOrder)
        {
            Random rnd = new Random();
            if (randomOrder)
                UnitTypes = AUnit.GetAllUnitTypes().OrderBy(t => rnd.Next()).ToArray();
            else
                UnitTypes = AUnit.GetAllUnitTypes().ToArray();
        }

        public IUnit CreateUnit()
        {
            var type = UnitTypes[pos++];
            if (pos == UnitTypes.Length)
                pos = 0;
            return (IUnit)Activator.CreateInstance(type);
        }
    }
}
