using System.ComponentModel;

namespace S7_DMCToolbox.Base.UserInterface
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void Initialize();
        void Deinitialize();
    }
}