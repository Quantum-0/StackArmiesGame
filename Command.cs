using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    interface ICommand
    {
        void Do();
        void Undo();
    }

    class TurnCommand : ICommand
    {
        List<ICommand> Subcommands;

        public TurnCommand(List<ICommand> subcommands)
        {
            Subcommands = subcommands;
        }

        public void Do()
        {
            Subcommands.ForEach(c => c.Do());
        }

        public void Undo()
        {
            Subcommands.Reverse();
            Subcommands.ForEach(c => c.Undo());
            Subcommands.Reverse();
        }
    }

    class CloneCommand : ICommand
    {
        int Pos;
        Army Army;
        IUnit Unit;

        public CloneCommand(Army army, int pos, IUnit unitToClone)
        {
            Army = army;
            Pos = pos;
            Unit = unitToClone;
        }

        public void Do()
        {
            Army.Insert(Pos, Unit);
        }

        public void Undo()
        {
            Army.RemoveAt(Pos);
        }
    }

    class ClotheCommand : ICommand
    {
        int Pos;
        Army Army;
        Type type;

        public ClotheCommand(Army army, int pos, Type attachmentType)
        {
            if (!attachmentType.IsSubclassOf(typeof(HeavyUnitAttachment)))
                throw new Exception("Неверный тип");

            Army = army;
            Pos = pos;
            type = attachmentType;
        }

        public void Do()
        {
            Army[Pos] = (HeavyUnitAttachment)Activator.CreateInstance(type, Army[Pos] as HeavyWarrior);
        }

        public void Undo()
        {
            Army[Pos] = (Army[Pos] as HeavyUnitAttachment).TakeOff();
        }
    }

    class AddHealthCommand : ICommand
    {
        int Pos;
        Army Army;
        int Change;

        public AddHealthCommand(Army army, int pos, int addhealth)
        {
            Change = addhealth;
            Army = army;
            Pos = pos;
        }

        public void Do()
        {
            Army[Pos].GetHit(-Change);
        }

        public void Undo()
        {
            Army[Pos].GetHit(Change);
        }
    }

    class RemoveUnitCommand : ICommand
    {
        IUnit RemovedUnit;
        int Pos;
        Army Army;

        public RemoveUnitCommand(Army army, int pos)
        {
            Army = army;
            Pos = pos;
        }

        public void Do()
        {
            RemovedUnit = Army[Pos];
            Army.RemoveAt(Pos);
        }

        public void Undo()
        {
            Army.Insert(Pos, RemovedUnit);
        }
    }
}
