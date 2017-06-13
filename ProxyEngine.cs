namespace StackArmyGame
{
    /*
     * No no no no no
     * 
     * 
    class ProxyEngine : IEngine
    {
        IEngine engine = Engine.Instance;

        private static ProxyEngine _Instance;
        public static ProxyEngine Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ProxyEngine();

                return _Instance;
            }
        }

        private ProxyEngine()
        { }

        public Army ArmyA
        {
            get
            {
                return engine.ArmyA;
            }
        }

        public Army ArmyB
        {
            get
            {
                return engine.ArmyB;
            }
        }

        public bool CanMakeTurn
        {
            get
            {
                return engine.CanMakeTurn;
            }
        }

        public bool CanRedo
        {
            get
            {
                return engine.CanRedo;
            }
        }

        public bool CanUndo
        {
            get
            {
                return engine.CanUndo;
            }
        }

        public void MakeTurn()
        {
            CUI.Log("Выполнение хода:");
            CUI.Log('>' + ArmyA[0].ToString() + " vs " + ArmyB[0].ToString());
            engine.MakeTurn();
        }

        public void Redo()
        {
            CUI.Log("Повторение хода");
            engine.Redo();
        }

        public void SetArmies(Army a, Army b)
        {
            CUI.Log("Армии выбраны");
            engine.SetArmies(a, b);
        }

        public void Undo()
        {
            CUI.Log("Отмена хода");
            engine.Undo();
        }
    }
    */
}
