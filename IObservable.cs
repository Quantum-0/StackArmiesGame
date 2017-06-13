namespace StackArmyGame
{
    interface IObservable
    {
        void Subscribe(IObserver observer);
        void Unsubscribe(IObserver observer);
    }

    interface IObserver
    {
        void Update(IObservable sender, int health);
    }
}