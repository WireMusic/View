using System;
using System.Collections.Generic;

namespace Stage.UIModule
{
    public class Menubar
    {
        public List<Menu> Menus;

        public Menubar()
        {
            Menus = new List<Menu>();
        }

        internal unsafe void Render()
        {
            if (_imgui.BeginMenuBar())
            {
                foreach (var menu in Menus)
                {
                    if (_imgui.BeginMenu(menu.Name, true))
                    {
                        foreach (var item in menu.Items)
                        {
                            item.Render();
                        }

                        _imgui.EndMenu();
                    }
                }

                _imgui.EndMenuBar();
            }
        }

        public void AddMenu(Menu menu)
        {
            foreach (Menu m in Menus)
            {
                if (m.Name == menu.Name)
                    return;
            }

            Menus.Add(menu);
        }

        public void RemoveMenu(string menuName)
        {
            Menu toRemove = null;

            foreach(Menu m in Menus)
            {
                if (m.Name == menuName)
                    toRemove = m;
            }

            if (toRemove != null)
                Menus.Remove(toRemove);
        }
    }

    public abstract class MenuPart
    {
        internal abstract string Name { get; set; }

        internal abstract void Render();
    }

    public class Menu : MenuPart
    {
        internal override string Name { get; set; }

        public List<MenuPart> Items;

        public Menu(string name)
        {
            Name = name;
            Items = new List<MenuPart>();
        }

        public void AddItem(MenuPart item)
        {
            foreach (MenuPart m in Items)
            {
                if (m.Name == item.Name)
                    return;
            }

            Items.Add(item);
        }

        public void RemoveMenu(string menuName)
        {
            MenuItem toRemove = null;

            foreach (MenuItem m in Items)
            {
                if (m.Name == menuName)
                    toRemove = m;
            }

            if (toRemove != null)
                Items.Remove(toRemove);
        }

        internal override unsafe void Render()
        {
            if (_imgui.BeginMenu(Name, true))
            {
                foreach (MenuPart m in Items)
                {
                    m.Render();
                }

                _imgui.EndMenu();
            }
        }
    }

    public class MenuItem : MenuPart
    {
        internal override string Name { get; set; }
        public string Shortcut { get; private set; }

        public Action Callback = null;

        public MenuItem(string name, string shortcut = "")
        {
            Name = name;
            Shortcut = shortcut;
        }

        public void SetCallback(Action callback)
        {
            Callback = callback;
        }

        public void Invoke()
        {
            if (Callback != null)
                Callback();
        }

        internal override unsafe void Render()
        {
            if (_imgui.MenuItem_Bool(Name, Shortcut, false, true))
            {
                Invoke();
            }
        }
    }
}
