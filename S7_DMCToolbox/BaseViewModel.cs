using DmcLib.Events;
using S7_DMCToolbox.Base.UserInterface;

namespace S7_DMCToolbox
{
    public abstract class BaseViewModel : NotifyPropertyChanged, IViewModel
    {       
        public virtual void Initialize()
        {
            
        }

        public virtual void Deinitialize()
        {
            
        }
    }
}