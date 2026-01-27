using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JouralAppWeb.Services
{
    public class Theme
    {
        public string CurrentTheme { get; private set; } = "light";

        public bool IsDarkMode => CurrentTheme == "dark";

        public event Action? OnThemeChanged;

        public void SetTheme(string theme)
        {
            CurrentTheme = theme;
            OnThemeChanged?.Invoke();
        }

        public void Reset()
        {
            SetTheme("light");
        }
    }
}
