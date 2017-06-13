using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Armor 30
 * Cost 25
 * Damage 0
 * Health 75
 */
namespace StackArmyGame
{
    class GulyayGorod : AUnit
    {
        private SpecialUnits.GulyayGorod gg = new SpecialUnits.GulyayGorod(75, 30, 25);

        public override int Armor
        {
            get
            {
                return gg.GetDefence();
            }
        }

        public override int Cost
        {
            get
            {
                return gg.GetCost();
            }
        }

        public override int Damage
        {
            get
            {
                return gg.GetStrength();
            }
        }

        protected override int _Health
        {
            get
            {
                return gg.GetCurrentHealth();
            }
            set
            {
                var cur = gg.GetCurrentHealth();
                var dmg = cur - value;
                GetHit(dmg);
            }
        }

        public override void GetHit(int strength)
        {
            if (strength < 0)
            {
                var hp = gg.GetCurrentHealth();
                gg = new SpecialUnits.GulyayGorod(75, 30, 25);
                gg.TakeDamage(gg.GetCurrentHealth() + gg.GetDefence() - hp + strength);
            }

            else if (!gg.IsDead)
            {
                gg.TakeDamage(strength + gg.GetDefence());

                if (gg.IsDead)
                    observers.ForEach(o => o.Update(this, gg.GetCurrentHealth()));
            }
        }
    }
}
