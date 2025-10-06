using HarmonyLib;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

[assembly: MelonInfo(typeof(Better_Pot_Fusion.Core), "Better Pot Fusion", "3.0.1", "dynaslash, Dakosha, JustNull & Mamoru-kun", null)]
[assembly: MelonGame("LanPiaoPiao", "PlantsVsZombiesRH")]

namespace Better_Pot_Fusion
{
    public class Core : MelonMod
    {
        private static Dictionary<PlantType, PlantType> plantMixDictionary = new Dictionary<PlantType, PlantType>
        {
            { PlantType.Cabbagepult, PlantType.CabbagePot },
            { PlantType.Cornpult, PlantType.CornPot },
            { PlantType.Garlic, PlantType.GarlicPot },
            { PlantType.Umbrellaleaf, PlantType.UmbrellaPot },
            { PlantType.Marigold, PlantType.SilverPot },
            { PlantType.Melonpult, PlantType.MelonPot },
            { PlantType.SunFlower, PlantType.SunPot },
            { PlantType.Plantern, PlantType.LanternPot },
            { PlantType.CherryBomb, PlantType.CherryPot },
            { PlantType.IceShroom, PlantType.IcePot },
            { PlantType.WallNut, PlantType.NutPot },
        };

        public override void OnInitializeMelon() => MelonLogger.Msg("Better Pot Fusion is loaded!");

        [HarmonyPatch(typeof(CreatePlant), nameof(CreatePlant.SetPlant))]
        public static class SetPlant_Patch
        {
            [HarmonyPrefix]
            public static bool SetPlant(int newColumn, int newRow, PlantType theSeedType)
            {
                if (!plantMixDictionary.ContainsKey(theSeedType))
                    return true;
                if (!Input.GetKey(KeyCode.LeftAlt))
                    return true;
                bool isSet = false;
                if ((GameAPP.Instance.gameObject.TryGetComponent(out TravelMgr travelMgr) && travelMgr.advancedUpgrades[44]) || Board.Instance.boardTag.isColumn)
                {
                    foreach (Plant plant in Board.Instance.plantArray.ToArray().Where(plant => plant != null && plant.thePlantColumn == newColumn && (plantMixDictionary.ContainsKey(plant.thePlantType) || plant.thePlantType == PlantType.GoldPot || plant.thePlantType == PlantType.Pot)))
                    {
                        PlantType targetPlantType = GetTargetPlantType(plant);
                        if (targetPlantType != 0)
                        {
                            if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, targetPlantType, null, Vector2.zero, true, true) != null)
                            {
                                if (targetPlantType == PlantType.CherryPot)
                                    Board.Instance.CreateCherryExplode(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y + 1.5f), plant.thePlantRow);
                                if (targetPlantType == PlantType.IcePot)
                                    Board.Instance.CreateFreeze(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y));
                                CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 0, 0);
                                if (Mouse.Instance.thePlantTypeOnMouse == PlantType.Melonpult)
                                {
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                }
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
                    foreach (Plant plant in Board.Instance.plantArray.ToArray().Where(plant => plant != null && plant.thePlantColumn == newColumn && plant.thePlantRow == newRow && (plantMixDictionary.ContainsKey(plant.thePlantType) || plant.thePlantType == PlantType.GoldPot || plant.thePlantType == PlantType.Pot)))
                    {
                        PlantType targetPlantType = GetTargetPlantType(plant);
                        if (targetPlantType != 0)
                        {
                            if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, targetPlantType, null, Vector2.zero, true, true) != null)
                            {
                                if (targetPlantType == PlantType.CherryPot)
                                    Board.Instance.CreateCherryExplode(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y + 1.5f), plant.thePlantRow);
                                if (targetPlantType == PlantType.IcePot)
                                    Board.Instance.CreateFreeze(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y));
                                CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 0, 0);
                                if (Mouse.Instance.thePlantTypeOnMouse == PlantType.Melonpult)
                                {
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                }
                                isSet = true;
                                foreach (Plant plant2 in Board.Instance.plantArray.ToArray().Where(plant2 => plant2 != null && plant2.thePlantColumn == newColumn && plant2.thePlantRow == Math.Abs(plant.thePlantRow - 5) && (plantMixDictionary.ContainsKey(plant.thePlantType) || plant.thePlantType == PlantType.GoldPot || plant.thePlantType == PlantType.Pot)))
                                {
                                    PlantType targetPlantType2 = GetTargetPlantType(plant2);
                                    if (targetPlantType2 != 0)
                                        if (CreatePlant.Instance.SetPlant(plant2.thePlantColumn, plant2.thePlantRow, targetPlantType2, null, Vector2.zero, true, true) != null)
                                        {
                                            if (targetPlantType2 == PlantType.CherryPot)
                                                Board.Instance.CreateCherryExplode(new Vector2(plant2.transform.localPosition.x, plant2.transform.localPosition.y + 1.5f), plant2.thePlantRow);
                                            plant2.Die(0);
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
                    foreach (Plant plant in Board.Instance.plantArray.ToArray().Where(plant => plant != null && plant.thePlantColumn == newColumn && plant.thePlantRow == newRow && (plantMixDictionary.ContainsKey(plant.thePlantType) || plant.thePlantType == PlantType.GoldPot || plant.thePlantType == PlantType.Pot)))
                    {
                        PlantType targetPlantType = GetTargetPlantType(plant);
                        if (targetPlantType != 0)
                        {
                            if (CreatePlant.Instance.SetPlant(plant.thePlantColumn, plant.thePlantRow, targetPlantType, null, Vector2.zero, true, true) != null)
                            {
                                if (targetPlantType == PlantType.CherryPot)
                                    Board.Instance.CreateCherryExplode(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y + 1.5f), plant.thePlantRow);
                                if (targetPlantType == PlantType.IcePot)
                                    Board.Instance.CreateFreeze(new Vector2(plant.transform.localPosition.x, plant.transform.localPosition.y));
                                CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 0, 0);
                                if (Mouse.Instance.thePlantTypeOnMouse == PlantType.Melonpult)
                                {
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                    CreateItem.Instance.SetCoin(plant.thePlantColumn, plant.thePlantRow, 1, 0);
                                }
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
            if (plant.thePlantType == PlantType.GoldPot)
                if (GameAPP.Instance.gameObject.TryGetComponent(out TravelMgr travelMgr))
                {
                    if (plantTypeOnMouse == PlantType.SunFlower && (travelMgr.weakUltimates.ToArray().Where(weak => weak == PlantType.SolarPot).Any() || Board.Instance.boardTag.enableTravelPlant))
                        return PlantType.SolarPot;
                    else
                        return 0;
                }
                else
                    return 0;
            return plantMixDictionary.TryGetValue(plantTypeOnMouse, out PlantType mixPlantType) ? (mixPlantType == plant.thePlantType ? 0 : mixPlantType) : 0;
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
