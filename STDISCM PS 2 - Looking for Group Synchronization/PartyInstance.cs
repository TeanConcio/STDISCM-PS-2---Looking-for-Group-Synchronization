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
            FORMING,
            READY,
            RUNNING,
            FINISHED
        }

        // Constants
        public const int REQUIRED_TANKS = 1;
        public const int REQUIRED_HEALERS = 1;
        public const int REQUIRED_DPS = 3;

        // Variables
        public int id;
        public int numTanks = 0;
        public int numHealers = 0;
        public int numDPS = 0;

        public PartyState state = PartyState.FORMING;
        public List<Player> players = new List<Player>();
        private Thread thread;

        // Constructor
        public PartyInstance(int id)
        {
            this.id = id;
        }

        // Needed roles
        public int[] NeededRoles()
        {
            int[] requiredRoles = new int[] { REQUIRED_TANKS - numTanks, REQUIRED_HEALERS - numHealers, REQUIRED_DPS - numDPS };

            return requiredRoles;
        }

        // Add player to party
        public void AddPlayer(Player player)
        {
            players.Add(player);
            switch (player.playerClass)
            {
                case Player.PlayerClass.TANK:
                    numTanks++;
                    break;
                case Player.PlayerClass.HEALER:
                    numHealers++;
                    break;
                case Player.PlayerClass.DPS:
                    numDPS++;
                    break;
            }
        }

        // Run
        public virtual void Run()
        {
            while (LFGQueuer.isRunning)
            {
                switch (this.state)
                {
                    case PartyState.FORMING:
                        if (this.numTanks == REQUIRED_TANKS ||
                            this.numHealers == REQUIRED_HEALERS ||
                            this.numDPS == REQUIRED_DPS)
                            {
                                this.state = PartyState.READY;
                            }
                        break;

                    case PartyState.READY:
                        this.state = PartyState.RUNNING;
                        break;

                    case PartyState.RUNNING:
                        Thread.Sleep(Random.Shared.Next(LFGQueuer.minFinishTime, LFGQueuer.maxFinishTime + 1));
                        this.state = PartyState.FINISHED;
                        break;

                    case PartyState.FINISHED:
                        this.numTanks = 0;
                        this.numHealers = 0;
                        this.numDPS = 0;
                        this.state = PartyState.FORMING;
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
