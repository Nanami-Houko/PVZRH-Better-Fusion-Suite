﻿using Il2Cpp;
using MelonLoader;
using UnityEngine;
using System.Collections.Generic;

[assembly: MelonInfo(typeof(Better_Pot_Fusion.Core), "Better Pot Fusion", "231.0.0", "dynaslash, JustNull & Mamoru-kun", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]

namespace Better_Pot_Fusion
{
    public class Core : MelonMod
    {
        private Dictionary<int, int> plantMixDictionary = new Dictionary<int, int>
        {
            { 26, 1112 }, // Cabbage
            { 28, 1114 }, // Kernel
            { 29, 1130 }, // Garlic
            { 30, 1133 }, // Umbrella
            { 31, 1136 }, // Marigold
            { 32, 1125 }, // Melon
            { 1, 1184 }, // Sunflower
            { 20, 1195 }, // Plantern
        };

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Better Pot Fusion is loaded!");
        }

        public override void OnUpdate()
        {
            if (Board.Instance != null && Mouse.Instance.theItemOnMouse != null && Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftAlt))
            {
                TryFusion();
            }
        }

        private void TryFusion()
        {
            for (int i = 0; i < Board.Instance.plantArray.Count; i++)
            {
                var plant = Board.Instance.plantArray[i];
                if (plant != null && plant.thePlantColumn == Mouse.Instance.theMouseColumn && plant.thePlantRow == Mouse.Instance.theMouseRow)
                {
                    // Check if the plant is a flower pot (ID 27)
                    if (IsFlowerPot(plant))
                    {
                        PerformFusion(plant);
                        break;
                    }
                    else
                    {
                        int plantTypeOnMouse = (int)Mouse.Instance.thePlantTypeOnMouse;
                        int targetPlantType = GetTargetPlantType(plant);

                        if (targetPlantType != 0)
                        {
                            if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)targetPlantType, null, Vector2.zero, true, true) != null)
                            {
                                // Return sun to the player
                                int sunReward = 25;
                                if (plantTypeOnMouse == 32) // Melon
                                    sunReward += 150;

                                Board.Instance.theSun += sunReward;

                                UpdateSunAndCooldowns();
                                plant.Die(0);
                            }
                        }
                    }
                }
            }
        }

        // Function to check if the plant is a flower pot (ID 27)
        private bool IsFlowerPot(Plant plant)
        {
            return (int)plant.thePlantType == 27; // Flower pot type ID
        }

        // Perform fusion for the flower pot (ID 27) or other valid plants
        private void PerformFusion(Plant plant)
        {
            int plantTypeOnMouse = (int)Mouse.Instance.thePlantTypeOnMouse;
            int targetPlantType = GetTargetPlantType(plant);

            if (targetPlantType != 0)
            {
                if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)targetPlantType, null, Vector2.zero, true, true) != null)
                {
                    // Return sun to the player
                    int sunReward = 25;
                    if (plantTypeOnMouse == 32) // Melon
                        sunReward += 150;

                    Board.Instance.theSun += sunReward;

                    UpdateSunAndCooldowns();
                    plant.Die(0); // Kill the original plant after fusion
                }
            }
        }

        private int GetTargetPlantType(Plant plant)
        {
            int plantTypeOnMouse = (int)Mouse.Instance.thePlantTypeOnMouse;

            if ((int)plant.thePlantType == 27)
            {
                return GetMixData(plantTypeOnMouse);
            }
            else if ((int)plant.thePlantType == 1137 && plantTypeOnMouse == 1)
                return 936;

            return 0;
        }

        private int GetMixData(int plantTypeOnMouse)
        {
            return plantMixDictionary.TryGetValue(plantTypeOnMouse, out int mixPlantType) ? mixPlantType : 0;
        }

        private void UpdateSunAndCooldowns()
        {
            if (Mouse.Instance.thePlantOnGlove == null)
            {
                Board.Instance.theSun -= Mouse.Instance.theCardOnMouse.theSeedCost;
                Mouse.Instance.theCardOnMouse.CD = 0f;
                Mouse.Instance.theCardOnMouse.PutDown();
                UnityEngine.Object.Destroy(Mouse.Instance.theItemOnMouse);
                Mouse.Instance.ClearItemOnMouse(false);
            }
            else
            {
                Mouse.Instance.thePlantOnGlove.GetComponent<Plant>().Die(0);
                Mouse.Instance.thePlantOnGlove = null;
                GameObject.Find("Glove").GetComponent<Glove>().CD = 0f;
                UnityEngine.Object.Destroy(Mouse.Instance.theItemOnMouse);
                Mouse.Instance.ClearItemOnMouse(true);
            }
        }
    }
}
