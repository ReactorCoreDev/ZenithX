using System;
using System.Collections.Generic;

namespace ZenithX
{
    public interface CreateList
    {
        string label { get; }
    }

    public struct ToggleInfo : CreateList
    {
        public string label { get; }
        public Func<bool> getState;
        public Action<bool> setState;

        public ToggleInfo(string label, Func<bool> getState, Action<bool> setState)
        {
            this.label = label;
            this.getState = getState;
            this.setState = setState;
        }
    }

    public class ButtonInfo : CreateList
    {
        public string label { get; }
        public Action action;

        public ButtonInfo(string label, Action action)
        {
            this.label = label;
            this.action = action;
        }
    }

    public struct SubmenuInfo
    {
        public string name;
        public bool isExpanded;
        public List<CreateList> items;
        public List<ToggleInfo> toggles;
        public List<ButtonInfo> buttons;

        public SubmenuInfo(string name, bool isExpanded, List<CreateList> items)
        {
            this.name = name;
            this.isExpanded = isExpanded;
            this.items = items;

            this.toggles = new List<ToggleInfo>();
            this.buttons = new List<ButtonInfo>();

            foreach (var item in items)
            {
                if (item is ToggleInfo toggle)
                    toggles.Add(toggle);
                else if (item is ButtonInfo button)
                    buttons.Add(button);
            }
        }
    }

    public struct GroupInfo
    {
        public string name;
        public bool isExpanded;
        public List<CreateList> items;
        public List<ToggleInfo> toggles;
        public List<ButtonInfo> buttons;
        public List<SubmenuInfo> submenus;

        public GroupInfo(string name, bool isExpanded, List<CreateList> items, List<SubmenuInfo> submenus)
        {
            this.name = name;
            this.isExpanded = isExpanded;
            this.items = items;
            this.submenus = submenus;

            this.toggles = new List<ToggleInfo>();
            this.buttons = new List<ButtonInfo>();

            foreach (var item in items)
            {
                if (item is ToggleInfo toggle)
                    toggles.Add(toggle);
                else if (item is ButtonInfo button)
                    buttons.Add(button);
            }
        }
    }
}