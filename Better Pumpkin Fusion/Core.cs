using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(Better_Pumpkin_Fusion.Core), "Better Pumpkin Fusion", "231.0.0", "dynaslash, Dakosha, JustNull & Mamoru-kun", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]

namespace Better_Pumpkin_Fusion
{
    public class Core : MelonMod
    {
        private static Dictionary<PlantType, PlantType> plantMixDictionary = new Dictionary<PlantType, PlantType>
        {
            { PlantType.Plantern, PlantType.LanternPumpkin },
            { PlantType.Cactus, PlantType.CactusPumpkin },
            { PlantType.Blover, PlantType.BlowerPumpkin },
            { PlantType.StarFruit, PlantType.StarPumpkin },
            { PlantType.Magnetshroom, PlantType.MagnetPumpkin },
            { PlantType.CherryBomb, PlantType.CherryPumpkin },
            { PlantType.PotatoMine, PlantType.PotatoPumpkin },
            { PlantType.ScaredyShroom, PlantType.ScaredyPumpkin },
            { PlantType.Squash, PlantType.SquashPumpkin },
            { PlantType.HypnoShroom, PlantType.HypnoPumpkin },
            { PlantType.TorchWood, PlantType.TorchPumpkin },
        };

        public override void OnInitializeMelon() => MelonLogger.Msg("Better Pumpkin Fusion is loaded!");

        [HarmonyPatch(typeof(CreatePlant), nameof(CreatePlant.SetPlant))]
        public static class SetPlant_Patch
        {
            [HarmonyPrefix]
            public static bool SetPlant(int newColumn, int newRow, PlantType theSeedType)
            {
                if (!plantMixDictionary.ContainsKey(theSeedType) && theSeedType != PlantType.MagnetBlover)
                    return true;
                if (!Input.GetKey(KeyCode.LeftShift))
                    return true;
                bool isSet = false;
                if ((GameAPP.Instance.gameObject.TryGetComponent(out TravelMgr travelMgr) && travelMgr.advancedUpgrades[44]) || Board.Instance.boardTag.isColumn)
                {
                    foreach (Plant plant in Board.Instance.plantArray.ToArray().Where(plant => plant != null && plant.thePlantColumn == newColumn
                    && (plantMixDictionary.ContainsKey(plant.thePlantType) || plant.thePlantType == PlantType.MagnetPumpkin || plant.thePlantType == PlantType.SuperPumpkin
                    || plant.thePlantType == PlantType.CactusPumpkin || plant.thePlantType == PlantType.BlowerPumpkin || plant.thePlantType == PlantType.UltimatePumpkin
                    || plant.thePlantType == PlantType.CherryUltimatePumpkin || plant.thePlantType == PlantType.Pumpkin)))
                    {
                        PlantType targetPlantType = GetTargetPlantType(plant);
                        if (targetPlantType != 0)
                        {
                            if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, targetPlantType, null, Vector2.zero, true, true) != null)
                            {
                                if (targetPlantType == PlantType.CherryPumpkin || targetPlantType == PlantType.CherryUltimatePumpkin)
                                    Board.Instance.CreateCherryExplode(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y + .5f), plant.thePlantRow);
                                isSet = true;
                                plant.Die(0);
                            }
                        }
                    }
                    if (isSet)
                        UpdateSunAndCooldowns();
                }
                else if (Board.Instance.boardTag.isMirror)
                {
                    foreach (Plant plant in Board.Instance.plantArray.ToArray().Where(plant => plant != null && plant.thePlantColumn == newColumn && plant.thePlantRow == newRow
                    && (plantMixDictionary.ContainsKey(plant.thePlantType) || plant.thePlantType == PlantType.MagnetPumpkin || plant.thePlantType == PlantType.SuperPumpkin
                    || plant.thePlantType == PlantType.CactusPumpkin || plant.thePlantType == PlantType.BlowerPumpkin || plant.thePlantType == PlantType.UltimatePumpkin
                    || plant.thePlantType == PlantType.CherryUltimatePumpkin || plant.thePlantType == PlantType.Pumpkin)))
                    {
                        PlantType targetPlantType = GetTargetPlantType(plant);
                        if (targetPlantType != 0)
                        {
                            if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, targetPlantType, null, Vector2.zero, true, true) != null)
                            {
                                if (targetPlantType == PlantType.CherryPumpkin || targetPlantType == PlantType.CherryUltimatePumpkin)
                                    Board.Instance.CreateCherryExplode(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y + .5f), plant.thePlantRow);
                                isSet = true;
                                foreach (Plant plant2 in Board.Instance.plantArray.ToArray().Where(plant2 => plant2 != null && plant2.thePlantColumn == newColumn && plant2.thePlantRow == Math.Abs(plant.thePlantRow - 5)
                                && (plantMixDictionary.ContainsKey(plant2.thePlantType) || plant2.thePlantType == PlantType.MagnetPumpkin || plant2.thePlantType == PlantType.SuperPumpkin
                                || plant2.thePlantType == PlantType.CactusPumpkin || plant2.thePlantType == PlantType.BlowerPumpkin || plant2.thePlantType == PlantType.UltimatePumpkin
                                || plant2.thePlantType == PlantType.CherryUltimatePumpkin || plant2.thePlantType == PlantType.Pumpkin)))
                                {
                                    PlantType targetPlantType2 = GetTargetPlantType(plant2);
                                    if (targetPlantType2 != 0)
                                    {
                                        if (CreatePlant.Instance.SetPlant(plant2.thePlantColumn, plant2.thePlantRow, targetPlantType2, null, Vector2.zero, true, true) != null)
                                        {
                                            if (targetPlantType2 == PlantType.CherryPumpkin || targetPlantType2 == PlantType.CherryUltimatePumpkin)
                                                Board.Instance.CreateCherryExplode(new Vector2(plant2.transform.localPosition.x, plant2.transform.localPosition.y + .5f), plant2.thePlantRow);
                                            plant2.Die(0);
                                        }
                                    }
                                }
                                plant.Die(0);
                            }
                        }
                    }
                    if (isSet)
                        UpdateSunAndCooldowns();
                }
                else
                {
                    foreach (Plant plant in Board.Instance.plantArray.ToArray().Where(plant => plant != null && plant.thePlantColumn == newColumn && plant.thePlantRow == newRow
                    && (plantMixDictionary.ContainsKey(plant.thePlantType) || plant.thePlantType == PlantType.MagnetPumpkin || plant.thePlantType == PlantType.SuperPumpkin
                    || plant.thePlantType == PlantType.CactusPumpkin || plant.thePlantType == PlantType.BlowerPumpkin || plant.thePlantType == PlantType.UltimatePumpkin
                    || plant.thePlantType == PlantType.CherryUltimatePumpkin || plant.thePlantType == PlantType.Pumpkin)))
                    {
                        PlantType targetPlantType = GetTargetPlantType(plant);
                        if (targetPlantType != 0)
                        {
                            if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, targetPlantType, null, Vector2.zero, true, true) != null)
                            {
                                if (targetPlantType == PlantType.CherryPumpkin || targetPlantType == PlantType.CherryUltimatePumpkin)
                                    Board.Instance.CreateCherryExplode(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y + .5f), plant.thePlantRow);
                                isSet = true;
                                plant.Die(0);
                            }
                        }
                    }
                    if (isSet)
                        UpdateSunAndCooldowns();
                }
                return !isSet;
            }
        }

        private static PlantType GetTargetPlantType(Plant plant)
        {
            PlantType plantTypeOnMouse = Mouse.Instance.thePlantTypeOnMouse;
            if (plantMixDictionary.ContainsKey(plant.thePlantType) && plantMixDictionary.ContainsKey(plantTypeOnMouse))
                return 0;
            if (plant.thePlantType == PlantType.MagnetPumpkin || plant.thePlantType == PlantType.SuperPumpkin || plant.thePlantType == PlantType.UltimatePumpkin || plant.thePlantType == PlantType.CherryUltimatePumpkin)
            {
                if (GameAPP.Instance.gameObject.TryGetComponent(out TravelMgr travelMgr))
                {
                    if ((travelMgr.unlockPlant[12] || Board.Instance.boardTag.enableAllTravelPlant) && (plant.thePlantType == PlantType.UltimatePumpkin || plant.thePlantType == PlantType.SuperPumpkin || plant.thePlantType == PlantType.CherryUltimatePumpkin))
                    {
                        if (plant.thePlantType == PlantType.SuperPumpkin && plantTypeOnMouse == PlantType.MagnetBlover)
                            return PlantType.UltimatePumpkin;
                        else if (plant.thePlantType == PlantType.UltimatePumpkin && plantTypeOnMouse == PlantType.CherryBomb)
                            return PlantType.CherryUltimatePumpkin;
                        else if (plant.thePlantType == PlantType.CherryUltimatePumpkin && plantTypeOnMouse == PlantType.Cactus)
                            return PlantType.UltimatePumpkin;
                    }
                    else if ((travelMgr.weakUltimates.ToArray().Where(weak => weak == PlantType.IFVPumpkin).Any() || Board.Instance.boardTag.enableTravelPlant) && plant.thePlantType == PlantType.MagnetPumpkin)
                    {
                        if (plantTypeOnMouse == PlantType.Magnetshroom)
                            return PlantType.IFVPumpkin;
                    }
                    else return 0;
                }
            }
            if (plant.thePlantType == PlantType.CactusPumpkin && plantTypeOnMouse == PlantType.Blover)
                return PlantType.SuperPumpkin;
            else if (plant.thePlantType == PlantType.BlowerPumpkin && plantTypeOnMouse == PlantType.Cactus)
                return PlantType.SuperPumpkin;
            if (plant.thePlantType == PlantType.Pumpkin) return plantMixDictionary.TryGetValue(plantTypeOnMouse, out PlantType mixPlantType) ? mixPlantType : 0;
            return 0;
        }

        private static void UpdateSunAndCooldowns()
        {
            if (Mouse.Instance.thePlantOnGlove == null)
            {
                if (Board.Instance.boardTag.isConvey)
                {
                    Mouse.Instance.theCardOnMouse.Die();
                    Mouse.Instance.theCardOnMouse.PutDown();
                    Mouse.Instance.ClearItemOnMouse(true);
                }
                else
                {
                    Board.Instance.theSun -= Mouse.Instance.theCardOnMouse.theSeedCost;
                    Mouse.Instance.theCardOnMouse.CD = 0f;
                    Mouse.Instance.theCardOnMouse.PutDown();
                    UnityEngine.Object.Destroy(Mouse.Instance.theItemOnMouse);
                    Mouse.Instance.ClearItemOnMouse(false);
                }
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
