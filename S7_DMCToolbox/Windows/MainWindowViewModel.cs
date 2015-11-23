using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DmcLib.Wpf.Commands;
using DmcLib.Wpf.ViewModels;
using S7_DMCToolbox.Base;
using S7_DMCToolbox.Base.UserInterface;

namespace S7_DMCToolbox.Windows
{
    [Export]
    public class MainWindowViewModel : DmcViewModel
    {
        private IGlobalEngine _GlobalEngine;
        [Import]
        public IGlobalEngine GlobalEngine
        {
            get
            {
                return this._GlobalEngine;
            }
            set
            {
                this.SetProperty(ref this._GlobalEngine, value);
            }
        }


		private String _Status = "test";
		public String Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this.SetProperty(ref this._Status, value);
			}
		}

        private IViewModel _ActiveScreen;
        public IViewModel ActiveScreen
        {
            get
            {
                return this._ActiveScreen;
            }
            protected set
            {
                this.SetProperty(ref this._ActiveScreen, value);
            }
        }

        public ICommand NavigateToScreenCommand { get; set; }
        public ICommand ToggleEventLogVisibiliyCommand { get; set; }

        public MainWindowViewModel()
        {
            this.NavigateToScreenCommand = new RelayCommand(this.NavigateToScreen);
            this.ToggleEventLogVisibiliyCommand = new RelayCommand(this.ShowDebugWindow);    
        }
        
        public void Initialize()
        {
            this.GlobalEngine.Initialize();

            this.ActiveScreen = ScreenManager.DefaultScreenViewModel;
        }

        private void NavigateToScreen(object param)
        {
            if (param is Type)
            {
                var vmType = param as Type;

                var screen = ScreenManager.Screens.Where(s => s.DataContext is IViewModel)
                              .Select(s => s.DataContext)
                              .Cast<IViewModel>()
                              .FirstOrDefault(vm => vm.GetType() == vmType);

                if (screen != null)
                {
                    this.ActiveScreen = screen;
                }   
            }
        }

        private void ShowDebugWindow(object param)
        {
            if (Application.Current.Windows.OfType<DebugWindow>().Any())
            {
                Application.Current.Windows.OfType<DebugWindow>().First().Activate();
            }
            else
            {
                new DebugWindow().Show();
            }
        }
    }
}