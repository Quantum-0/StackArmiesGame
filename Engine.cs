using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    class NoUnitsInArmyException : Exception
    {
        public NoUnitsInArmyException(string message) : base(message)
        {
        }
    }

    class ArmyIsNotSettedException : Exception
    {
        public ArmyIsNotSettedException(string message) : base(message)
        {
        }
    }

    class NoSavedStateException : Exception
    {
        public NoSavedStateException(string message) : base(message)
        {
        }
    }

    class UnitIsDeadException : Exception
    { }

    class Engine : IEngine
    {
        private static Engine _Instance;
        private Random Rnd = new Random();
        public static Engine Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Engine();

                return _Instance;
            }
        }
        public Army ArmyA { get; private set; }
        public Army ArmyB { get; private set; }
        private object Sync = new object();
        private IStrategy Strategy = new OneVsOne();

        private Engine()
        {
        }

        public void SetStrategy(IStrategy strategy)
        {
            Strategy = strategy;
        }

        public void SetArmies(Army a, Army b)
        {
            ArmyA = a;
            ArmyB = b;
            RedoCommands.Clear();
            UndoCommands.Clear();
        }

        /// <exception cref="NoUnitsInArmyException"/>
        /// <exception cref="ArmyIsNotSettedException"/>
        public void MakeTurn()
        {
            lock (Sync)
            {
                if (ArmyA == null || ArmyB == null)
                    throw new ArmyIsNotSettedException("Армия не выбрана");

                if (ArmyA.Count == 0)
                    throw new NoUnitsInArmyException($"В армии {nameof(ArmyA)} отсутствуют юниты");
                if (ArmyB.Count == 0)
                    throw new NoUnitsInArmyException($"В армии {nameof(ArmyB)} отсутствуют юниты");

                Army first, second;
                List<ICommand> commands = new List<ICommand>();
                ChooseArmies(out first, out second);
                FightNearestUnits(first, second, ref commands);
                RemoveDead(ref commands);
                FightRangedUnits(first, second, ref commands);
                FightRangedUnits(second, first, ref commands);
                RemoveDead(ref commands);
                UndoCommands.Push(new TurnCommand(commands));
                RedoCommands.Clear();
            }
        }

        private void RemoveDead(ref List<ICommand> commands)
        {
            List<RemoveUnitCommand> removedeadcmds = new List<RemoveUnitCommand>();

            foreach (var unit in ArmyA.Where(u => u.Health <= 0))
            {
                var cmd = new RemoveUnitCommand(ArmyA, ArmyA.IndexOf(unit));
                removedeadcmds.Add(cmd);
            }

            foreach (var unit in ArmyB.Where(u => u.Health <= 0))
            {
                var cmd = new RemoveUnitCommand(ArmyB, ArmyB.IndexOf(unit));
                removedeadcmds.Add(cmd);
            }

            removedeadcmds.Reverse();
            removedeadcmds.ForEach(c => c.Do());

            commands.AddRange(removedeadcmds);

            /*CUI.Log("После удаления юнитов:");
            CUI.Log(ArmyA.ToString());
            CUI.Log(ArmyB.ToString());

            commands.ForEach(c => c.Undo());

            CUI.Log("Восстановление юнитов:");
            CUI.Log(ArmyA.ToString());
            CUI.Log(ArmyB.ToString());*/

            //ArmyA.RemoveAll(a => a.Health <= 0);
            //ArmyB.RemoveAll(a => a.Health <= 0);
        }

        private void ChooseArmies(out Army first, out Army second)
        {
            if (Rnd.NextDouble() < 0.5)
            {
                first = ArmyA;
                second = ArmyB;
            }
            else
            {
                first = ArmyB;
                second = ArmyA;
            }
        }

        private void FightNearestUnits(Army first, Army second, ref List<ICommand> commands)
        {
            var list = Strategy.GetNearest(first, second);
            foreach (var item in list)
                FightNearest(item, ref commands);
        }

        private void FightNearest(Tuple<IUnit, IUnit> tuple, ref List<ICommand> commands)
        {
            var dmg = tuple.Item1.Damage;
            if (Engine.Instance.HitAndCheckAlive(tuple.Item2, dmg, ref commands))
            {
                dmg = tuple.Item2.Damage;
                if (!Engine.Instance.HitAndCheckAlive(tuple.Item1, dmg, ref commands))
                    CUI.Log(tuple.Item2.ToString() + " убил " + tuple.Item1.ToString());
            }
            else
                CUI.Log(tuple.Item1.ToString() + " убил " + tuple.Item2.ToString());
        }

        private void FightRangedUnits(Army from, Army to, ref List<ICommand> commands)
        {
            for (int i = 0; i < from.Count; i++)
            {
                var unit = from[i];

                if (!(unit is IAbility))
                    continue;

                var range = Strategy.GetRanged(unit, from, to);
                ((IAbility)unit).DoAction(range.Item1, range.Item2, ref commands);
            }
        }

        private bool HitAndCheckAlive(IUnit unit, int strength, ref List<ICommand> commands)
        {
            var before = unit.Health;
            Army army;
            if (ArmyA.Contains(unit))
                army = ArmyA;
            else
                army = ArmyB;

            ICommand cmd = null;
            if (strength > 0)
                cmd = new AddHealthCommand(army, army.IndexOf(unit), Math.Min(strength, Rnd.Next(unit.Armor)) - strength);
            else if (strength < 0)
                ;

            if (cmd != null)
            {
                cmd.Do();
                commands.Add(cmd);
                CUI.Log(unit.ToString() + " получил " + (before - unit.Health) + " урона");
            }
            return unit.Health > 0;
        }

        /// <exception cref="NoSavedStateException"/>
        public void Undo()
        {
            var cmd = UndoCommands.Pop();
            cmd.Undo();
            RedoCommands.Push(cmd);

            CUI.Log("Ход отменён");
            CUI.Log(ArmyA.ToString());
            CUI.Log(ArmyB.ToString());
        }

        /// <exception cref="NoSavedStateException"/>
        public void Redo()
        {
            var cmd = RedoCommands.Pop();
            cmd.Do();
            UndoCommands.Push(cmd);

            CUI.Log("Ход повторен");
            CUI.Log(ArmyA.ToString());
            CUI.Log(ArmyB.ToString());
        }

        private Stack<ICommand> UndoCommands = new Stack<ICommand>();
        private Stack<ICommand> RedoCommands = new Stack<ICommand>();

        public bool CanUndo
        {
            get
            {
                return UndoCommands.Count > 0;
            }
        }
        public bool CanRedo
        {
            get
            {
                return RedoCommands.Count > 0;
            }
        }
        public bool CanMakeTurn
        {
            get
            {
                return ArmyA?.Count > 0 && ArmyB?.Count > 0;
            }
        }
    }
}
