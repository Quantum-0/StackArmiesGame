namespace StackArmyGame
{
    interface IEngine
    {
        Army ArmyA { get; }
        Army ArmyB { get; }

        void SetArmies(Army a, Army b);
        void MakeTurn();
        void Undo();
        void Redo();

        bool CanUndo { get; }
        bool CanRedo { get; }
        bool CanMakeTurn { get; }
    }
}