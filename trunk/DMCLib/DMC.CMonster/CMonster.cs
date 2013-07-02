using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace DMC.CMonster
{
    public partial class CMonster<ActionClassType>
    {
        // TODO:  This should be loaded from logic file.  Public for now so we can programatically generate SEA data.
        public CData CData { get; set; }

        public string State { get; private set; }
        public bool IsRunning { get; private set; }

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Queue<string> events;
        private Queue<string> actions;

        private Thread mainThread;
        private object actionObject;

        public CMonster(object actionObject)
        {
            // TODO:  the initial state should come from logic file
            this.State = "Idle";

            this.IsRunning = false;

            this.events = new Queue<string>();
            this.actions = new Queue<string>();

            this.actionObject = actionObject;

            this.mainThread = new Thread(MainThread);
            this.mainThread.Start();
        }

        public void Shutdown()
        {
            logger.Debug("CMonster {0} shutdown requested.", typeof(ActionClassType).Name);
            this.IsRunning = false;
        }

        public void FireEvent(string evnt)
        {
            logger.Debug("CMonster {0} even fired: {1}", typeof(ActionClassType).Name, evnt);
            this.events.Enqueue(evnt);
        }

        private void MainThread()
        {
            this.IsRunning = true;

            while (this.IsRunning)
            {
                if (this.actions.Count > 0)
                {
                    string action = this.actions.Dequeue();

                    MethodInfo mi = this.actionObject.GetType().GetMethods().Where(m => m.Name == action).First() as MethodInfo;

                    logger.Debug("CMonster {0} processing action: {1}", typeof(ActionClassType).Name, action);
                    mi.Invoke(this.actionObject, null);
                }
                else if (this.events.Count > 0)
                {
                    string newEvent = this.events.Dequeue();

                    //TODO:  dude, make these lines below a bit more readable...
                    CStateData stateData = (from item in this.CData where item.State == this.State select item).First() as CStateData;

                    var q = from item in stateData.EventActions where item.Event == newEvent select item;
                    if (q.Count() > 0)
                    {
                        CEventData eventData = q.First() as CEventData;

                        foreach (string a in eventData.Actions)
                        {
                            this.actions.Enqueue(a);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(40);
                }
            }
        }
    }
}
