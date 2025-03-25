using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDISCM_PS_2___Looking_for_Group_Synchronization
{
    internal class DungeonInstance
    {
        public enum DungeonState
        {
            EMPTY,
            ACTIVE,
        }

        // Variables
        public uint id;
        public DungeonState state = DungeonState.EMPTY;
        public uint partiesServed = 0;
        public uint totalTimeServing = 0;

        private Thread thread;

        // Constructor
        public DungeonInstance(uint id)
        {
            this.id = id;
            this.thread = new Thread(Run);
        }

        // Add party members
        public bool AddMembers()
        {
            if (this.state == DungeonState.EMPTY)
            {
                LFGQueuer.PrintDungeonInstances(id, DungeonState.ACTIVE);
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
                DungeonState origState = this.state;

                switch (this.state)
                {
                    case DungeonState.EMPTY:
                        break;

                    case DungeonState.ACTIVE:
                        int sleepTime = Random.Shared.Next((int)LFGQueuer.minFinishTime, (int)LFGQueuer.maxFinishTime + 1);
                        Thread.Sleep(sleepTime * 1000);
                        this.totalTimeServing += (uint)sleepTime;

                        LFGQueuer.PrintDungeonInstances(id, DungeonState.EMPTY, (uint)sleepTime);
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
