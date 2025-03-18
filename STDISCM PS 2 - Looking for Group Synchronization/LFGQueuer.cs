using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDISCM_PS_2___Looking_for_Group_Synchronization
{
    internal class LFGQueuer
    {
        // Default Inputs
        private const int DEFAULT_MAX_INSTANCES = 4;
        private const int DEFAULT_TANKS_PER_INSTANCE = 8;
        private const int DEFAULT_HEALERS_PER_INSTANCE = 8;
        private const int DEFAULT_DPS_PER_INSTANCE = 24;
        private const int DEFAULT_MIN_FINISH_TIME = 10;
        private const int DEFAULT_MAX_FINISH_TIME = 30;

        // Inputs
        public static int maxInstances;
        public static int tanksPerInstance;
        public static int healersPerInstance;
        public static int DPSPerInstance;
        public static int minFinishTime;
        public static int maxFinishTime;

        public static bool isRunning = true;

        // Variables
        public static List<PartyInstance> instances = new List<PartyInstance>();
        public static List<Player> playerQueue = new List<Player>();

        // Get inputs from config.txt
        public static void GetConfig()
        {
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
                        if (!int.TryParse(parts[1].Trim(), out int numInstances) || numInstances < 1)
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
                        if (!int.TryParse(parts[1].Trim(), out int numTanks) || numTanks < 1)
                        {
                            Console.WriteLine($"Error: Invalid Number of Tanks. Setting Number of Tanks to {DEFAULT_TANKS_PER_INSTANCE}.");
                            tanksPerInstance = DEFAULT_TANKS_PER_INSTANCE;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            tanksPerInstance = numTanks;
                        }
                        break;
                    case "NUMBER_OF_HEALER_PLAYERS (H)":
                        if (!int.TryParse(parts[1].Trim(), out int numHealers) || numHealers < 1)
                        {
                            Console.WriteLine($"Error: Invalid Number of Healers. Setting Number of Healers to {DEFAULT_HEALERS_PER_INSTANCE}.");
                            healersPerInstance = DEFAULT_HEALERS_PER_INSTANCE;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            healersPerInstance = numHealers;
                        }
                        break;
                    case "NUMBER_OF_DPS_PLAYERS (D)":
                        if (!int.TryParse(parts[1].Trim(), out int numDPS) || numDPS < 1)
                        {
                            Console.WriteLine($"Error: Invalid Number of DPS. Setting Number of DPS to {DEFAULT_DPS_PER_INSTANCE}.");
                            DPSPerInstance = DEFAULT_DPS_PER_INSTANCE;
                            hasErrorWarning = true;
                        }
                        else
                        {
                            DPSPerInstance = numDPS;
                        }
                        break;
                    case "MIN_FINISH_TIME_FOR_INSTANCE (T1)":
                        if (!int.TryParse(parts[1].Trim(), out int minTime) || minTime < 1)
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
                        if (!int.TryParse(parts[1].Trim(), out int maxTime) || maxTime < 1)
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

            // If there is an error, print a line
            if (hasErrorWarning)
            {
                Console.WriteLine();
            }

            // Print Configurations
            Console.WriteLine($"Max Number of Instances (n) : {maxInstances}");
            Console.WriteLine($"Number of Tanks per Instance (t) : {tanksPerInstance}");
            Console.WriteLine($"Number of Healers per Instance (h) : {healersPerInstance}");
            Console.WriteLine($"Number of DPS per Instance (d) : {DPSPerInstance}");
            Console.WriteLine($"Min Finish Time for Instance (t1) : {minFinishTime}");
            Console.WriteLine($"Max Finish Time for Instance (t2) : {maxFinishTime}");

            Console.WriteLine();
        }

        // Initialize
        public static void Initialize()
        {
            // Initialize party instances
            for (int i = 0; i < maxInstances; i++)
            {
                instances.Add(new PartyInstance(i + 1));
            }

            // Initialize player queue
            List<Player> temp = new List<Player>();
            for (int i = 0; i < tanksPerInstance; i++)
            {
                temp.Add(new Player(i, Player.PlayerClass.TANK));
            }
            for (int i = 0; i < healersPerInstance; i++)
            {
                temp.Add(new Player(i, Player.PlayerClass.HEALER));
            }
            for (int i = 0; i < DPSPerInstance; i++)
            {
                temp.Add(new Player(i, Player.PlayerClass.DPS));
            }

            // Shuffle player queue
            while (temp.Any())
            {
                int randomIndex = Random.Shared.Next(temp.Count);
                playerQueue.Add(temp[randomIndex]);
                temp.RemoveAt(randomIndex);
            }
        }

        // Run
        public static void Run()
        {
            int prioritizedParty = 0;

            while (isRunning)
            {

            }
        }

        // Start
        public static void Start()
        {
            foreach (PartyInstance instance in instances)
            {
                instance.Start();
            }

            Run();
        }
    }
}
