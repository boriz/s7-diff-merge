using System.ComponentModel.Composition;
using DmcLib.Events;
using S7_DMCToolbox.Base;
using S7_DMCToolbox.Base.Modules;

namespace S7_DMCToolbox.GlobalEngine
{
    public class GlobalEngine : NotifyPropertyChanged, IGlobalEngine
    {
        private readonly ITemplateEngine TemplateEngine;
        
        [ImportingConstructor]
        public GlobalEngine(ITemplateEngine templateEngine)
        {
            this.TemplateEngine = templateEngine;
        }
        
        public void Initialize()
        {
            this.TemplateEngine.Initialize();
        }

        public void Close()
        {
            
        }
    }
}