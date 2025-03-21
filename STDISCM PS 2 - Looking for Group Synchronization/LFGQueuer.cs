﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        private const uint DEFAULT_MIN_FINISH_TIME = 1000;
        private const uint DEFAULT_MAX_FINISH_TIME = 5000;

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

        public static bool isRunning = true;
        private static uint numFullParties = 0;
        private static uint printNum = 0;
        private static readonly object printLock = new object();

        // Variables
        private static List<PartyInstance> partyInstances = new List<PartyInstance>();

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
                        if (!uint.TryParse(parts[1].Trim(), out uint numTanks) || numTanks > int.MaxValue || numTanks < 1)
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
                        if (!uint.TryParse(parts[1].Trim(), out uint numHealers) || numHealers > int.MaxValue || numHealers < 1)
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
                        if (!uint.TryParse(parts[1].Trim(), out uint numDPS) || numDPS > int.MaxValue || numDPS < 1)
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
                        if (!uint.TryParse(parts[1].Trim(), out uint minTime) || minTime > int.MaxValue || minTime < 1)
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
                        if (!uint.TryParse(parts[1].Trim(), out uint maxTime) || maxTime > int.MaxValue || maxTime < 1)
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

            // Get number of full parties
            numFullParties = Math.Min(
                tankPlayers / REQUIRED_TANKS_PER_INSTANCE, 
                Math.Min(
                    healerPlayers / REQUIRED_HEALERS_PER_INSTANCE, 
                    DPSPlayers / REQUIRED_DPS_PER_INSTANCE));

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
        }

        // Initialize
        public static void Initialize()
        {
            // Initialize party instances
            for (uint i = 0; i < maxInstances; i++)
            {
                partyInstances.Add(new PartyInstance(i + 1));
            }
        }

        // Run
        public static void Run()
        {
            uint prioritizedParty = 0;

            while (isRunning)
            {
                if (numFullParties > 0)
                {
                    bool result = partyInstances[(int)prioritizedParty].AddMembers();
                    if (result)
                        numFullParties--;

                    prioritizedParty++;
                    if (prioritizedParty >= maxInstances)
                        prioritizedParty = 0;
                }
                else
                {
                    // Check if all party instances are waiting
                    bool allWaiting = true;
                    foreach (PartyInstance instance in partyInstances)
                    {
                        if (instance.state != PartyInstance.PartyState.WAITING)
                        {
                            allWaiting = false;
                            break;
                        }
                    }
                    if (allWaiting && numFullParties == 0)
                    {
                        isRunning = false;
                    }
                }
                    
            }

            PrintPartyInstances();
        }

        // Start
        public static void Start()
        {
            PrintPartyInstances();

            foreach (PartyInstance instance in partyInstances)
            {
                instance.Start();
            }

            Run();
        }

        // Print Party Instances
        public static void PrintPartyInstances()
        {
            lock (printLock)
            {
                Console.WriteLine();
                Console.WriteLine($"Log {printNum++}: ");
                foreach (PartyInstance instance in partyInstances)
                {
                    Console.WriteLine($"Party Instance {instance.id} : {instance.state}");
                }
            }
        }
        public static void PrintPartyInstances(uint changedPartyInstance, PartyInstance.PartyState nextPartyState)
        {
            lock (printLock)
            {
                Console.WriteLine();
                Console.WriteLine($"Log {printNum++}: ");
                foreach (PartyInstance instance in partyInstances)
                {
                    if (instance.id == changedPartyInstance)
                    {
                        Console.WriteLine($"Party Instance {instance.id} : {instance.state} -> {nextPartyState}");
                    }
                    else
                    {
                        Console.WriteLine($"Party Instance {instance.id} : {instance.state}");
                    }
                }
            }
        }
    }
}
