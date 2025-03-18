using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDISCM_PS_2___Looking_for_Group_Synchronization
{
    internal class PartyInstance
    {
        public enum PartyState
        {
            WAITING,
            RUNNING,
        }

        // Variables
        public uint id;
        public PartyState state = PartyState.WAITING;
        private Thread thread;

        // Constructor
        public PartyInstance(uint id)
        {
            this.id = id;
            this.thread = new Thread(Run);
        }

        // Add party members
        public bool AddMembers()
        {
            if (this.state == PartyState.WAITING)
            {
                LFGQueuer.PrintPartyInstances(id, PartyState.RUNNING);
                this.state = PartyState.RUNNING;
                return true;
            }

            return false;
        }

        // Run
        public virtual void Run()
        {
            while (LFGQueuer.isRunning)
            {
                PartyState origState = this.state;

                switch (this.state)
                {
                    case PartyState.WAITING:
                        break;

                    case PartyState.RUNNING:
                        Thread.Sleep(Random.Shared.Next((int)LFGQueuer.minFinishTime, (int)LFGQueuer.maxFinishTime + 1));
                        LFGQueuer.PrintPartyInstances(id, PartyState.WAITING);
                        this.state = PartyState.WAITING;
                        break;
                }
            }
        }

        // Start
        public void Start()
        {
            thread.Start();
        }

        // Join
        public void Join()
        {
            thread.Join();
        }
    }
}
