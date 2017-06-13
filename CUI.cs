using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StackArmyGame
{
    public abstract class MenuItem
    {
        public string Text { get; protected set; }
        public MenuItemWithSubmenu Parent { get; protected set; }
        public bool Enabled { get; set; }
    }

    public class MenuItemWithSubmenu : MenuItem
    {
        public List<MenuItem> SubItems { get; private set; }

        public MenuItemWithSubmenu(string text, MenuItemWithSubmenu parent = null, bool enabled = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Не указан текст пункта меню", nameof(text));
            
            Text = text;
            Enabled = enabled;
            Parent = parent ?? CUI.MenuRoot;
        }

        private MenuItemWithSubmenu()
        {

        }

        public void AddSubmenu(MenuItem item)
        {
            if (SubItems == null)
                SubItems = new List<MenuItem>();
            if (item is MenuItemWithSubmenu)
                SubItems.Add(new MenuItemWithSubmenu(item.Text, this, item.Enabled) { SubItems = ((MenuItemWithSubmenu)item).SubItems });
            else if (item is MenuItemWithAction)
                SubItems.Add(new MenuItemWithAction(item.Text, ((MenuItemWithAction)item).Action, this, item.Enabled));
            else
                throw new NotImplementedException();
        }

        public void SetSubmenu(params MenuItem[] submenu)
        {
            foreach (var item in submenu)
                AddSubmenu(item);
        }

        public static MenuItemWithSubmenu CreateRoot(IEnumerable<MenuItem> menuItems = null)
        {
            var root = new MenuItemWithSubmenu() { SubItems = menuItems?.ToList() };
            return root;
        }
    }

    public class MenuItemWithAction : MenuItem
    {
        public Action Action { get; private set; }

        public MenuItemWithAction(string text, Action action, MenuItemWithSubmenu parent = null, bool enabled = true)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Не указан текст пункта меню", nameof(text));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            Text = text;
            Action = action;
            Enabled = enabled;
            Parent = parent ?? CUI.MenuRoot;
        }
    }

    public enum MenuInteraction
    {
        Left,
        Right,
        Up,
        Down,
        Enter,
        Esc
    }

    public static class CUI
    {
        private static List<string> _Log = new List<string>();
        private static int logOffset;
        private static int selectedMenuItem;
        public static bool Exited { get; private set; }
        private static MenuItemWithSubmenu CurrentMenu;
        private static MenuItemWithSubmenu _MenuRoot;
        public static MenuItemWithSubmenu MenuRoot
        {
            get
            {
                if (_MenuRoot == null)
                    _MenuRoot = MenuItemWithSubmenu.CreateRoot();
                return _MenuRoot;
            }
            private set
            {
                _MenuRoot = value;
            }
        }

        public static bool AutoRedrawLog = true;
        public static bool AutoScrollLog = true;

        static CUI()
        {
            CurrentMenu = MenuRoot;
            Console.CursorVisible = false;
        }

        public static void RedrawAll()
        {
            RedrawMenu();
            Console.SetCursorPosition(0, 3);
            Console.Write(new string('=', Console.BufferWidth));
            RedrawLog();
        }

        public static void RedrawLog()
        {
            int count = Console.WindowHeight - 6;
            var log = _Log.Skip(logOffset).Take(count).Select(s => "  " + s.Substring(0, Math.Min(s.Length, Console.BufferWidth - 4)));
            Console.SetCursorPosition(0, 5);
            foreach (var str in log)
            {
                Console.Write(new string(' ', Console.BufferWidth - 1));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.WriteLine(str);
            }
            Console.Write(new string(' ', Console.BufferWidth - 1));
        }

        public static void Log(string log)
        {
            _Log.Add(DateTime.Now.ToShortTimeString() + "  " + log);
            if (AutoRedrawLog)
                RedrawLog();

            if (AutoScrollLog && _Log.Count >= Console.WindowHeight - 6)
                logOffset++;
        }

        public static void RedrawMenu()
        {
            var menu = CurrentMenu.SubItems.Select(m => m.Text).ToArray();
            Console.SetCursorPosition(2, 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(2, 1);
            for (int i = 0; i < menu.Length; i++)
            {
                if (selectedMenuItem == i)
                {
                    Console.ForegroundColor = CurrentMenu.SubItems[i].Enabled ? ConsoleColor.White : ConsoleColor.Gray;
                    Console.BackgroundColor = CurrentMenu.SubItems[i].Enabled ? ConsoleColor.Red : ConsoleColor.DarkRed;
                }
                else
                {
                    Console.ForegroundColor = CurrentMenu.SubItems[i].Enabled ? ConsoleColor.Red : ConsoleColor.DarkRed;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.Write(menu[i].First());

                if (selectedMenuItem == i)
                {
                    Console.ForegroundColor = CurrentMenu.SubItems[i].Enabled ? ConsoleColor.White : ConsoleColor.Gray;
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                }
                else
                {
                    Console.ForegroundColor = CurrentMenu.SubItems[i].Enabled ? ConsoleColor.Gray : ConsoleColor.DarkGray;
                    Console.BackgroundColor = ConsoleColor.Black;
                }

                Console.Write(menu[i].Substring(1));

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Gray;
                if (i != menu.Length - 1)
                    Console.Write("   ");
            }
        }

        public static string InputString(string Text)
        {
            Console.SetCursorPosition(2, 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(2, 1);
            Console.Write(Text + ": ");
            var str = Console.ReadLine();
            RedrawAll();
            return str;
        }

        public static void ProcessInteraction(MenuInteraction Int)
        {
            switch (Int)
            {
                case MenuInteraction.Left:
                    if (--selectedMenuItem == -1)
                        selectedMenuItem = CurrentMenu.SubItems.Count - 1;
                    RedrawMenu();
                    break;
                case MenuInteraction.Right:
                    if (++selectedMenuItem == CurrentMenu.SubItems.Count)
                        selectedMenuItem = 0;
                    RedrawMenu();
                    break;
                case MenuInteraction.Up:
                    if (logOffset > 0)
                        logOffset--;
                    RedrawLog();
                    break;
                case MenuInteraction.Down:
                    logOffset++;
                    RedrawLog();
                    break;
                case MenuInteraction.Enter:
                    if (CurrentMenu.SubItems[selectedMenuItem] is MenuItemWithSubmenu)
                    {
                        CurrentMenu = CurrentMenu.SubItems[selectedMenuItem] as MenuItemWithSubmenu;
                        selectedMenuItem = 0;
                        RedrawMenu();
                    }
                    else
                    {
                        if (CurrentMenu.SubItems[selectedMenuItem].Enabled)
                            (CurrentMenu.SubItems[selectedMenuItem] as MenuItemWithAction).Action.Invoke();
                    }
                    break;
                case MenuInteraction.Esc:
                    if (CurrentMenu == MenuRoot)
                    {
                        Exited = true;
                        return;
                    }
                    
                    selectedMenuItem = CurrentMenu.Parent.SubItems.ToList().IndexOf(CurrentMenu);
                    CurrentMenu = CurrentMenu.Parent;
                    RedrawMenu();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static void CreateMenu(IEnumerable<MenuItem> Menu)
        {
            if (Menu == null)
                throw new ArgumentNullException(nameof(Menu));

            MenuRoot = MenuItemWithSubmenu.CreateRoot(Menu);
            CurrentMenu = MenuRoot;
        }

        internal static void Exit()
        {
            Exited = true;
        }
    }
}
