using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    class ProxyArcher : Archer
    {
        private Archer archer;

        public ProxyArcher()
        {
            archer = new Archer();
        }

        public override void DoAction(IEnumerable<IUnit> allies, IEnumerable<IUnit> enemies, ref List<ICommand> commands)
        {
            IUnit target;

            if (rnd.NextDouble() < Chance)
            {
                if (allies.Count() == 0)
                {
                    CUI.Log("Лучник не смог попасть цель");
                    return;
                }
                target = allies.ElementAt(rnd.Next(allies.Count()));
            }
            else
            {
                if (enemies.Count() == 0)
                {
                    CUI.Log("Лучник не смог найти цель");
                    return;
                }
                target = enemies.ElementAt(rnd.Next(enemies.Count()));
            }

            var before = target.Health;
            Army army;
            if (Engine.Instance.ArmyA.Contains(target))
                army = Engine.Instance.ArmyA;
            else
                army = Engine.Instance.ArmyB;

            var hp = rnd.Next(target.Armor) - Strength;
            var cmd = new AddHealthCommand(army, army.IndexOf(target), hp);
            cmd.Do();
            commands.Add(cmd);
            CUI.Log("Лучник целится, выстреливает и попадает в " + target + " отняв ему " + (-hp) + "hp");
        }

        public override void GetHit(int strength)
        {
            if (strength > 0)
                CUI.Log("Лучник получает урон и теряет " + strength + " единиц жизни");
            else if (strength < 0)
                CUI.Log("Лучнику восстановленно " + (-strength) + " единиц жизни");
            base.GetHit(strength);
        }
    }
}
