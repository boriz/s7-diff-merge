using System.Threading;
using DmcLib.Events;
using S7_DMCToolbox.Base.Modules;
using Template.Engine.Drivers;

namespace Template.Engine
{
    public class TemplateEngine : NotifyPropertyChanged, ITemplateEngine
    {
        private ITemplateEngineDriver templateDriver;
        private bool running;

        public TemplateEngine()
        {
            this.templateDriver = new SimulatorDriver();
        }
        
        public void Initialize()
        {
            
        }

        public void Close()
        {
            this.Stop();
        }

        public void TestMethod()
        {
            this.running = true;
            for (var i = 0; i < 30; i++)
            {
                if (!this.running)
                    return;

                Thread.Sleep(100);
            }
            this.running = false;
        }

        public void Stop()
        {
            this.running = false;
        }

        public void ChangeMode(TemplateEngineMode engineMode)
        {
            this.templateDriver.Deinitialize();
            this.templateDriver = this.GetDriver(engineMode);
            this.templateDriver.Initialize();
        }

        private ITemplateEngineDriver GetDriver(TemplateEngineMode engineMode)
        {
            switch (engineMode)
            {
                case TemplateEngineMode.Template:
                    return new TemplateDriver();
                default:
                case TemplateEngineMode.Simulator:
                    return new SimulatorDriver();
            }
        }
    }
}