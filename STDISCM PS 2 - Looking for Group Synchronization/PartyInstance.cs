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
            EMPTY,
            ACTIVE,
        }

        // Variables
        public uint id;
        public PartyState state = PartyState.EMPTY;
        public uint partiesServed = 0;
        public uint totalTimeServing = 0;

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
            if (this.state == PartyState.EMPTY)
            {
                LFGQueuer.PrintPartyInstances(id, PartyState.ACTIVE);
                this.state = PartyState.ACTIVE;
                this.partiesServed++;
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
                    case PartyState.EMPTY:
                        break;

                    case PartyState.ACTIVE:
                        int sleepTime = Random.Shared.Next((int)LFGQueuer.minFinishTime, (int)LFGQueuer.maxFinishTime + 1);
                        Thread.Sleep(sleepTime * 1000);
                        this.totalTimeServing += (uint)sleepTime;

                        LFGQueuer.PrintPartyInstances(id, PartyState.EMPTY, (uint)sleepTime);
                        this.state = PartyState.EMPTY;
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
