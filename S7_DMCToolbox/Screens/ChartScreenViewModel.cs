using System;
using System.ComponentModel.Composition;
using System.Threading;

namespace S7_DMCToolbox.Screens
{
    public class ChartScreenViewModel : BaseViewModel
    {
        private double _Value;
        public double Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                SetProperty(ref this._Value, value);
            }
        }

        private Random rand;
        private Timer timer;

        public ChartScreenViewModel()
        {
            this.rand = new Random();
            this.timer = new Timer(TimerTick, null, 0, 1000);
        }

        private void TimerTick(object state)
        {
            this.Value = this.rand.NextDouble();
        }
    }
}