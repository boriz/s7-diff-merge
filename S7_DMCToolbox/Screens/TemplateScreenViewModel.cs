using System.ComponentModel.Composition;
using System.Threading;
using System.Windows.Input;
using DmcLib.Wpf.Commands;
using S7_DMCToolbox.Base.Modules;

namespace S7_DMCToolbox.Screens
{
    public class TemplateScreenViewModel : BaseViewModel
    {
        private ITemplateEngine _TemplateEngine;
        [Import]
        public ITemplateEngine TemplateEngine
        {
            get
            {
                return this._TemplateEngine;
            }
            set
            {
                this.SetProperty(ref this._TemplateEngine, value);
            }
        }

        private bool _IsRunning;
        public bool IsRunning
        {
            get
            {
                return this._IsRunning;
            }
            set
            {
                this.SetProperty(ref this._IsRunning, value);
            }
        }

        private string _TestMethodStatus;
        public string TestMethodStatus
        {
            get
            {
                return this._TestMethodStatus;
            }
            set
            {
                this.SetProperty(ref this._TestMethodStatus, value);
            }
        }

        public ICommand TestButtonCommand { get; private set; }
        public ICommand StopButtonCommand { get; private set; }

        public TemplateScreenViewModel()
        {
            this.TestButtonCommand = new AsyncRelayCommand(a => this.TestButton(), p => !this.IsRunning);
            this.StopButtonCommand = new AsyncRelayCommand(a => this.Stop(), p => this.IsRunning);
        }

        private void TestButton()
        {
            this.IsRunning = true;
            this.TestMethodStatus = "Test Method Started";
            this.TemplateEngine.TestMethod();
            this.TestMethodStatus = "Test Method Finished";
            this.IsRunning = false;
            Thread.Sleep(1000);
        }

        private void Stop()
        {
            this.TestMethodStatus = "Stopping task...";
            this.TemplateEngine.Stop();
            this.IsRunning = false;
            this.TestMethodStatus = "Task stopped";
        }
    }
}