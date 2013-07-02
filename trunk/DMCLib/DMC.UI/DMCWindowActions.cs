using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace DMC.UI
{
    public partial class DMCWindow : Window
    {
        public void ShowAboutWindow()
        {
            this.Dispatcher.Invoke(
                new Action(
                    delegate()
                    {
                        new AboutWindow()
                            {
                                Owner = this,
                            }.ShowDialog();
                    }));
        }

        public void CloseChildWindows()
        {
            this.Dispatcher.Invoke(
                new Action(
                    delegate()
                    {
                        while (this.OwnedWindows.Count > 0)
                        {
                            this.OwnedWindows[0].Close();
                        }
                    }));
        }

        public void SetBusyTrue()
        {
        }

        public void SetBusyFalse()
        {

        }
    }
}
