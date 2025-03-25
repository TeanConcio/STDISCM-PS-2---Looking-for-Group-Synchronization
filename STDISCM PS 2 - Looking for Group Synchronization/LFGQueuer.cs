using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace STDISCM_PS_2___Looking_for_Group_Synchronization
{
    internal class LFGQueuer
    {
        // Default Inputs
        private const uint DEFAULT_MAX_INSTANCES = 4;
        private const uint DEFAULT_TANKS_PER_INSTANCE = 8;
        private const uint DEFAULT_HEALERS_PER_INSTANCE = 8;
        private const uint DEFAULT_DPS_PER_INSTANCE = 24;
        private const uint DEFAULT_MIN_FINISH_TIME = 1;
        private const uint DEFAULT_MAX_FINISH_TIME = 5;

        // Members per Instance

        private const uint REQUIRED_TANKS_PER_INSTANCE = 1;
        private const uint REQUIRED_HEALERS_PER_INSTANCE = 1;
        private const uint REQUIRED_DPS_PER_INSTANCE = 3;

        // Inputs
        public static uint maxInstances;
        public static uint tankPlayers;
        public static uint healerPlayers;
        public static uint DPSPlayers;
        public static uint minFinishTime;
        public static uint maxFinishTime;
        
        // Variables
        private static List<DungeonInstance> dungeonInstances = new List<DungeonInstance>();
        public static bool isRunning = true;
        private static uint numFullParties = 0;
        private static uint waitingParties = 0;
        private static uint finishedParties = 0;
        private static uint logNum = 0;
        private static readonly QueuedLock printLock = new QueuedLock();

        // Get inputs from config.txt
        public static void GetConfig()
        {
            Console.WriteLine("Getting Configurations from config.txt");
            Console.WriteLine();

            bool hasErrorWarning = false;

            string[] lines = System.IO.File.ReadAllLines("config.txt");
            foreach (string line in lines)
            {
                // Skip empty lines or comments (#)
                if (line.Trim() == "" || line.Trim().StartsWith("#"))
                {
                    continue;
                }

                // Split line by "="
                string[] parts = line.Split("=");

                switch (parts[0].Trim().ToUpper())
                {
                    case "MAX_NUMBER_OF_INSTANCES (N)":
                        if (!uint.TryParse(parts[1].Trim(), out uint numInstances) || numInstances > int.MaxValue || numInstances < 1)
                        {
                            Console.WriteLine($"Error: Invalid Number of Instances. Setting Number of Instances to {DEFAULT_MAX_INSTANCES}.");
                            maxInstances = DEFAULT_MAX_INSTANCES;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            maxInstances = numInstances;
                        }
                        break;
                    case "NUMBER_OF_TANK_PLAYERS (T)":
                        if (!uint.TryParse(parts[1].Trim(), out uint numTanks) || numTanks > int.MaxValue)
                        {
                            Console.WriteLine($"Error: Invalid Number of Tanks. Setting Number of Tanks to {DEFAULT_TANKS_PER_INSTANCE}.");
                            tankPlayers = DEFAULT_TANKS_PER_INSTANCE;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            tankPlayers = numTanks;
                        }
                        break;
                    case "NUMBER_OF_HEALER_PLAYERS (H)":
                        if (!uint.TryParse(parts[1].Trim(), out uint numHealers) || numHealers > int.MaxValue)
                        {
                            Console.WriteLine($"Error: Invalid Number of Healers. Setting Number of Healers to {DEFAULT_HEALERS_PER_INSTANCE}.");
                            healerPlayers = DEFAULT_HEALERS_PER_INSTANCE;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            healerPlayers = numHealers;
                        }
                        break;
                    case "NUMBER_OF_DPS_PLAYERS (D)":
                        if (!uint.TryParse(parts[1].Trim(), out uint numDPS) || numDPS > int.MaxValue)
                        {
                            Console.WriteLine($"Error: Invalid Number of DPS. Setting Number of DPS to {DEFAULT_DPS_PER_INSTANCE}.");
                            DPSPlayers = DEFAULT_DPS_PER_INSTANCE;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            DPSPlayers = numDPS;
                        }
                        break;
                    case "MIN_FINISH_TIME_FOR_INSTANCE (T1)":
                        if (!uint.TryParse(parts[1].Trim(), out uint minTime) || minTime > int.MaxValue)
                        {
                            Console.WriteLine($"Error: Invalid Minimum Finish Time. Setting Minimum Finish Time to {DEFAULT_MIN_FINISH_TIME}.");
                            minFinishTime = DEFAULT_MIN_FINISH_TIME;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            minFinishTime = minTime;
                        }
                        break;
                    case "MAX_FINISH_TIME_FOR_INSTANCE (T2)":
                        if (!uint.TryParse(parts[1].Trim(), out uint maxTime) || maxTime > int.MaxValue)
                        {
                            Console.WriteLine($"Error: Invalid Maximum Finish Time. Setting Maximum Finish Time to {DEFAULT_MAX_FINISH_TIME}.");
                            maxFinishTime = DEFAULT_MAX_FINISH_TIME;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            maxFinishTime = maxTime;
                        }
                        break;
                }
            }

            // Check min finish time is less than max finish time
            if (minFinishTime > maxFinishTime)
            {
                Console.WriteLine($"Error: Minimum Finish Time is greater than Maximum Finish Time. Setting Minimum Finish Time to {DEFAULT_MIN_FINISH_TIME} and Maximum Finish Time to {DEFAULT_MAX_FINISH_TIME}.");
                minFinishTime = DEFAULT_MIN_FINISH_TIME;
                maxFinishTime = DEFAULT_MAX_FINISH_TIME;
                hasErrorWarning = true;
            }

            // Get number of full parties
            numFullParties = Math.Min(
                tankPlayers / REQUIRED_TANKS_PER_INSTANCE, 
                Math.Min(
                    healerPlayers / REQUIRED_HEALERS_PER_INSTANCE, 
                    DPSPlayers / REQUIRED_DPS_PER_INSTANCE));

            waitingParties = numFullParties;

            // If there is an error, print a line
            if (hasErrorWarning)
            {
                Console.WriteLine();
            }

            // Print Configurations
            Console.WriteLine($"Max Number of Instances (n) : {maxInstances}");
            Console.WriteLine($"Number of Tanks per Instance (t) : {tankPlayers}");
            Console.WriteLine($"Number of Healers per Instance (h) : {healerPlayers}");
            Console.WriteLine($"Number of DPS per Instance (d) : {DPSPlayers}");
            Console.WriteLine($"Min Finish Time for Instance (t1) : {minFinishTime}");
            Console.WriteLine($"Max Finish Time for Instance (t2) : {maxFinishTime}");
            Console.WriteLine();
            Console.WriteLine($"Number of Full Parties : {numFullParties}");

            Console.WriteLine();
            Console.WriteLine();
        }

        // Initialize
        public static void Initialize()
        {
            // Initialize dungeon instances
            for (uint i = 0; i < maxInstances; i++)
            {
                dungeonInstances.Add(new DungeonInstance(i + 1));
            }
        }

        // Start
        public static void Start()
        {
            Console.WriteLine("Starting LFG Queuer");

            PrintDungeonInstances();

            foreach (DungeonInstance instance in dungeonInstances)
            {
                instance.Start();
            }

            Run();
        }

        // Run
        public static void Run()
        {
            uint prioritizedDungeon = 0;
            uint partiesLeft = numFullParties;
            while (isRunning)
            {
                if (partiesLeft > 0)
                {
                    bool result = dungeonInstances[(int)prioritizedDungeon].AddMembers();
                    if (result)
                        partiesLeft--;

                    prioritizedDungeon++;
                    if (prioritizedDungeon >= maxInstances)
                        prioritizedDungeon = 0;
                }
                else
                {
                    // Check if all dungeon instances are waiting
                    bool allWaiting = true;
                    foreach (DungeonInstance instance in dungeonInstances)
                    {
                        if (instance.state != DungeonInstance.DungeonState.EMPTY)
                        {
                            allWaiting = false;
                            break;
                        }
                    }
                    if (allWaiting && partiesLeft == 0)
                    {
                        isRunning = false;
                    }
                }  
            }

            // Join threads
            foreach (DungeonInstance instance in dungeonInstances)
            {
                instance.Join();
            }

            PrintDungeonInstances();
            Console.WriteLine();
            Console.WriteLine();

            PrintSummary();
        }

        // Print Dungeon Instances
        public static async void PrintDungeonInstances()
        {
            using (printLock.Lock())
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine($"Log {logNum++}: ");

                foreach (DungeonInstance instance in dungeonInstances)
                {
                    sb.AppendLine($"\tDungeon Instance {instance.id} : {instance.state}");
                }

                sb.AppendLine($"\tWaiting Parties: {waitingParties}");
                sb.AppendLine($"\tFinished Parties: {finishedParties}");

                Console.WriteLine(sb.ToString());
            }
        }
        public static async void PrintDungeonInstances(
            uint changedDungeonInstance, 
            DungeonInstance.DungeonState nextDungeonState, 
            uint runTime = 0)
        {
            using (printLock.Lock())
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine();
                sb.AppendLine($"Log {logNum++}: ");

                foreach (DungeonInstance instance in dungeonInstances)
                {
                    if (instance.id == changedDungeonInstance)
                    {
                        string runTimeString = "";

                        if (nextDungeonState == DungeonInstance.DungeonState.EMPTY)
                        {
                            runTimeString = $" (Runtime: {runTime} seconds)";
                            LFGQueuer.finishedParties++;
                        }
                        else if (nextDungeonState == DungeonInstance.DungeonState.ACTIVE)
                        {
                            LFGQueuer.waitingParties--;
                        }

                        sb.AppendLine($"\tDungeon Instance {instance.id} : {instance.state} -> {nextDungeonState}" + runTimeString);
                        instance.state = nextDungeonState;
                    }
                    else
                    {
                        sb.AppendLine($"\tDungeon Instance {instance.id} : {instance.state}");
                    }
                }

                sb.AppendLine($"\tWaiting Parties: {waitingParties}");
                sb.AppendLine($"\tFinished Parties: {finishedParties}");

                Console.WriteLine(sb.ToString());
            }
        }

        // Print Summary
        public static void PrintSummary()
        {
            Console.WriteLine("Summary: ");
            Console.WriteLine();

            // Print dungeon instances summary
            foreach (DungeonInstance instance in dungeonInstances)
            {

                Console.WriteLine($"Dungeon Instance {instance.id} : {instance.partiesServed} parties served; {instance.totalTimeServing} seconds total serving time");
            }

            Console.WriteLine();

            // Leftover players
            uint leftoverTanks = tankPlayers - numFullParties * REQUIRED_TANKS_PER_INSTANCE;
            uint leftoverHealers = healerPlayers - numFullParties * REQUIRED_HEALERS_PER_INSTANCE;
            uint leftoverDPS = DPSPlayers - numFullParties * REQUIRED_DPS_PER_INSTANCE;

            Console.WriteLine($"Leftover Players: {leftoverTanks} Tanks, {leftoverHealers} Healers, {leftoverDPS} DPS");
        }
    }
}
