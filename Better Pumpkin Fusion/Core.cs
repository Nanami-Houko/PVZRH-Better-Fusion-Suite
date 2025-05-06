using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(Better_Pot_Fusion.Core), "Better Pumpkin Fusion", "231.0.0", "dynaslash, JustNull & Mamoru-kun", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]

namespace Better_Pot_Fusion
{
    public class Core : MelonMod
    {
        private Dictionary<int, int> plantMixDictionary = new Dictionary<int, int>
        {
            { 20, 1087 }, // Plantern
            { 21, 1088 }, // Cactus
            { 22, 1091 }, // Blover
            { 23, 1089 }, // Starfruit
            { 25, 1092 }, // Magnet-shroom
            { 2, 1164 }, // Cherry Bomb
            { 4, 1190 }, // Potato Mine
            { 9, 1200 }, // Scaredy-shroom
            { 13, 1202 }, // Squash
            { 8, 1205 }, // Hypno-shroom
        };

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Better Pot Fusion is loaded!");
        }

        public override void OnUpdate()
        {
            if (Board.Instance != null && Mouse.Instance.theItemOnMouse != null && Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
            {
                TryFusion();
            }
        }

        private void TryFusion()
        {
            // Loop through the plant array and check for matching plant types in the same tile
            foreach (var plant in Board.Instance.plantArray)
            {
                if (plant != null && plant.thePlantColumn == Mouse.Instance.theMouseColumn && plant.thePlantRow == Mouse.Instance.theMouseRow)
                {
                    // Check if the plant on the mouse is a pumpkin
                    if (IsPumpkin(plant))
                    {
                        PerformFusion(plant);
                        break;
                    }
                    else
                    {
                        int targetPlantType = GetTargetPlantType(plant);
                        if (targetPlantType != 0)
                        {
                            PerformFusion(plant);
                            break;
                        }
                    }
                }
            }
        }

        // Function to check if the plant is a pumpkin
        private bool IsPumpkin(Plant plant)
        {
            return (int)plant.thePlantType == 24; // Pumpkin type ID
        }

        // Handles the fusion process and updating sun and cooldowns
        private void PerformFusion(Plant plant)
        {
            int plantTypeOnMouse = (int)Mouse.Instance.thePlantTypeOnMouse;

            int targetPlantType = GetTargetPlantType(plant);

            if (targetPlantType != 0)
            {
                if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, (PlantType)targetPlantType, null, Vector2.zero, true, true) != null)
                {
                    // Return sun to the player after fusion
                    UpdateSunAndCooldowns();
                    plant.Die(0); // Kill the original plant after fusion
                }
            }
        }

        // Get the target plant type based on the current plant and the type on mouse
        private int GetTargetPlantType(Plant plant)
        {
            int plantTypeOnMouse = (int)Mouse.Instance.thePlantTypeOnMouse;

            if ((int)plant.thePlantType == 24)
                return GetMixData(plantTypeOnMouse);
            else if ((int)plant.thePlantType == 1110 && plantTypeOnMouse == 1102)
                return 911;
            else if ((int)plant.thePlantType == 1088 && plantTypeOnMouse == 22)
                return 1110;
            else if ((int)plant.thePlantType == 1091 && plantTypeOnMouse == 21)
                return 1110;
            else if ((int)plant.thePlantType == 911 && plantTypeOnMouse == 2)
                return 922;
            else if ((int)plant.thePlantType == 922 && plantTypeOnMouse == 21)
                return 911;
            else if ((int)plant.thePlantType == 1092 && plantTypeOnMouse == 25)
                return 935;

            return 0;
        }

        // Retrieve the fusion plant type from the dictionary
        private int GetMixData(int plantTypeOnMouse)
        {
            return plantMixDictionary.TryGetValue(plantTypeOnMouse, out int mixPlantType) ? mixPlantType : 0;
        }

        // Handle sun deduction and reset of cooldowns
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
