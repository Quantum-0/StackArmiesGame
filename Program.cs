using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

/*
 * 
 * Функционал                | Паттерн             | Класс
 * ---------------------------------------------------------------------------------------
 * Создание юнита            | фабричный метод     | UnitFactory.cs
 * Движок                    | синглтон            | Engine.cs
 * Отмена/Повтор хода        | команда             | Engine.cs
 * Стратегия боя             | стратегия           | Engine.cs Strategy.cs
 * Наблюдатель (смерть)      | наблюдатель         | IObservable.cs Observers.cs AUnit.cs
 * Гуляй город               | адаптер             | GulyayGorod.cs
 * Клонирование юнитов магом | прототип            | IClonable, некоторые юниты
 * Логирование действий      | прокси              | ProxyArcher.cs
 * Улучшения Heavy           | декоратор           | HeavyUnitAttachments.cs
 * 
 * 
 */

namespace StackArmyGame
{
    class Program
    {
        static Dictionary<string, Army> Armies = new Dictionary<string, Army>();
        static MenuItemWithAction Undo, Redo, Turn;
        static IObserver Consoleobserver = new ConsoleObserver();
        static IObserver Beepobserver = new BeepObserver();
        static IEngine engine;

        static void Main(string[] args)
        {
            int n = 0;
            List<MenuItem> menu = new List<MenuItem>();
            var createarmy = new MenuItemWithSubmenu("Create Army");
            var startgame = new MenuItemWithAction("Start Game", () => { StartGame(); });
            var chooseStrategy = new MenuItemWithSubmenu("Select strategy");
            menu.Add(createarmy);
            menu.Add(startgame);
            Turn = new MenuItemWithAction("Make turn", () => { MakeTurn(); }, enabled: false);
            menu.Add(Turn);
            Undo = new MenuItemWithAction("Undo", () => { engine.Undo(); UpdateUndoRedoMakeTurnEnabled(); }, enabled: false);
            menu.Add(Undo);
            Redo = new MenuItemWithAction("Redo", () => { engine.Redo(); UpdateUndoRedoMakeTurnEnabled(); }, enabled: false);
            menu.Add(Redo);
            menu.Add(chooseStrategy);
            menu.Add(new MenuItemWithAction("Exit", () => { CUI.Exit(); }));
            createarmy.AddSubmenu(new MenuItemWithAction("Random", () => { CreateArmy(new RandomUnitFactory()); }));
            createarmy.AddSubmenu(new MenuItemWithAction("AllInOrder", () => { CreateArmy(new AllInOrderUnitFactory(false)); }));
            createarmy.AddSubmenu(new MenuItemWithAction("AllInRandomOrder", () => { CreateArmy(new AllInOrderUnitFactory(true)); }));
            createarmy.AddSubmenu(new MenuItemWithAction("OnlyLight", () => { CreateArmy(new LightUnitFactory()); }));
            chooseStrategy.AddSubmenu(new MenuItemWithAction("1 vs 1", () => { SetStrategy(new OneVsOne()); }));
            chooseStrategy.AddSubmenu(new MenuItemWithAction("3 vs 3", () => { SetStrategy(new ThreeVsThree()); }));
            chooseStrategy.AddSubmenu(new MenuItemWithAction("All vs All", () => { SetStrategy(new AllVsAll()); }));
            CUI.MenuRoot.SetSubmenu(menu.ToArray());
            CUI.RedrawAll();

            engine = Engine.Instance;

            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.Enter:
                        CUI.ProcessInteraction(MenuInteraction.Enter);
                        break;
                    case ConsoleKey.Escape:
                        CUI.ProcessInteraction(MenuInteraction.Esc);
                        break;
                    case ConsoleKey.LeftArrow:
                        CUI.ProcessInteraction(MenuInteraction.Left);
                        break;
                    case ConsoleKey.UpArrow:
                        CUI.ProcessInteraction(MenuInteraction.Up);
                        break;
                    case ConsoleKey.RightArrow:
                        CUI.ProcessInteraction(MenuInteraction.Right);
                        break;
                    case ConsoleKey.DownArrow:
                        CUI.ProcessInteraction(MenuInteraction.Down);
                        break;
                    default:
                        Console.Beep();
                        break;
                }
                if (CUI.Exited)
                    break;
            }


            /*
            var ArmyA = new Army();
            var units = UnitFabric.GetAllUnitTypes();
            ArmyA.AddRange(units.Select(T =>  ));

            var ArmyB = new Army();
            for (int i = 0; i < ArmyA.Count; i++)
                ArmyB.Add(UnitFabric.CreateLightWarrior());
            
            Engine.Instance.SetArmies(ArmyA, ArmyB);
            Engine.Instance.MakeTurn();

            Console.ReadLine();
            
            
            
            Console.WriteLine("  Unit Type  | Armor | Cost | Damage | Health |");
            foreach (var item in ArmyA)
            {
                Console.WriteLine("{0,-13}|{1,6} |{2,5} |{3,7} |{4,7} |", item.GetType().Name, item.Armor, item.Cost, item.Damage, item.Health);
            }

            Console.ReadLine();
            */
        }

        private static void SetStrategy(IStrategy strategy)
        {
            CUI.Log("Выбрана стратегия: " + strategy.Name);
            Engine.Instance.SetStrategy(strategy);
            CUI.ProcessInteraction(MenuInteraction.Esc); // для выхода в меню
        }

        private static void UpdateUndoRedoMakeTurnEnabled()
        {
            CUI.MenuRoot.SubItems.Find(i => i.Text == "Make turn").Enabled = Engine.Instance.CanMakeTurn;
            CUI.MenuRoot.SubItems.Find(i => i.Text == "Undo").Enabled = Engine.Instance.CanUndo;
            CUI.MenuRoot.SubItems.Find(i => i.Text == "Redo").Enabled = Engine.Instance.CanRedo;
            CUI.RedrawMenu();
        }

        private static void MakeTurn()
        {
            engine.MakeTurn();
            UpdateUndoRedoMakeTurnEnabled();
            CUI.Log("First Army: " + Engine.Instance.ArmyA);
            CUI.Log("Second Army: " + Engine.Instance.ArmyB);

            if (engine.CanMakeTurn == false)
            {
                string key;
                if (engine.ArmyA.Count == 0)
                {
                    CUI.Log("Вторая армия победила");
                    key = Armies.First(a => a.Value == engine.ArmyA).Key;
                }
                else
                {
                    CUI.Log("Первая армия победила");
                    key = Armies.First(a => a.Value == engine.ArmyB).Key;
                }
                Armies.Remove(key);
                CUI.Log($"Армия {key} удалена, т.к. не содержит в себе юнитов");
            }

            CUI.Log(""); // чтоб отделить ходы
        }

        private static void StartGame()
        {
            if (Armies.Count < 2)
            {
                CUI.Log("Ошибка. Прежде чем начать игру нужно создать 2 армии");
                return;
            }

            Army First, Second;
            while (true)
            {
                var first = CUI.InputString("First Army");
                if (!Armies.ContainsKey(first))
                {
                    CUI.Log($"Армия '{first}'не найдена");
                    continue;
                }
                else
                {
                    First = Armies[first];
                    break;
                }
            }

            while (true)
            {
                var second = CUI.InputString("Second Army");
                if (!Armies.ContainsKey(second))
                {
                    CUI.Log($"Армия '{second}'не найдена");
                    continue;
                }
                else
                {
                    Second = Armies[second];
                    break;
                }
            }

            engine.SetArmies(First, Second);
            UpdateUndoRedoMakeTurnEnabled();
            CUI.Log("Новая игра создана");
        }

        private static void CreateArmy(IUnitFactory factory)
        {
            var coststr = CUI.InputString("Army Cost");
            int cost = 0;
            if (!int.TryParse(coststr, out cost))
            {
                CUI.Log($"Не удалось создать армию стоимостью '{coststr}'");
                return;
            }

            var army = new Army();
            while(true)
            {
                var unit = factory.CreateUnit();
                ((IObservable)unit).Subscribe(Beepobserver);
                if (unit.Cost > cost)
                    break;
                cost -= unit.Cost;
                army.Add(unit);
            }

            CUI.Log($"Создана армия, стоимостью {army.Cost}, размером {army.Count}");
            foreach (var unit in army)
            {
                CUI.Log(unit.ToString());
            }

            while (true)
            {
                var armyname = CUI.InputString("Army Name");
                if (!Armies.ContainsKey(armyname))
                {
                    Armies.Add(armyname, army);
                    CUI.Log($"Армия '{armyname}' сохранена");
                    return;
                }
                CUI.Log("Армия уже существует. Введите другое название");
            }
        }
    }
}
