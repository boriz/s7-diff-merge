using System;
using System.Collections.Generic;
using System.Linq;
using DmcLib.Logging;
using S7_DMCToolbox.Windows;

namespace S7_DMCToolbox
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
            : base(new Guid("F636C65A-97C6-4796-B835-7BE3F805D4EB"))
        {
        }

        protected override void Start()
        {
            base.Start();

            if (this.MainWindow.DataContext is MainWindowViewModel)
            {
                EventLog.LogInfo("Main Window Initializing...");
                (this.MainWindow.DataContext as MainWindowViewModel).Initialize();
                EventLog.LogInfo("Main Window Initialized.");
            }
        }

        protected override IEnumerable<object> GetPartsToCompose()
        {
            return ScreenManager.Screens.Where(s => s.DataContext != null)
                                        .Select(s => s.DataContext);
        }
    }
}
