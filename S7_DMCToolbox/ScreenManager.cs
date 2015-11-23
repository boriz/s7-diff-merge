using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using S7_DMCToolbox.Base.UserInterface;
using S7_DMCToolbox.Screens;

namespace S7_DMCToolbox
{
    public static class ScreenManager
    {
        public static IEnumerable<UserControl> Screens { get; private set; }

        public static IViewModel DefaultScreenViewModel
        {
            get
            {
                return Screens.Select(p => p.DataContext).Cast<IViewModel>().First();
            }
        }

        static ScreenManager()
        {
            // TODO: Add all screens
            Screens = new List<UserControl>
            {
                new TemplateScreen(),
                new ChartScreen(),
            };

            if (Screens.Any(s => s.DataContext != null && !(s.DataContext is IViewModel)))
            {
                throw new System.ApplicationException("Screen data context can only be of type IViewModel");
            }
        }
    }
}