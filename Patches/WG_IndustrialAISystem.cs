using HarmonyLib;
using UnityEngine;
using Game.Prefabs;
using Game.Companies;
using Game.Simulation;
using WG_FittingWorkers.Systems;
using static Game.Buildings.PropertyUtils;
using Unity.Entities;
using Unity.Mathematics;
using BepInEx;
using Unity.Collections;
using Unity.Jobs;
using Game.Economy;

namespace WG_FittingWorkers.Patches
{
	[HarmonyPatch(typeof(IndustrialAISystem), nameof(IndustrialAISystem.GetFittingWorkers))]
	class IndustrialPatch
	{	
		// Could expand these to include different types of industry combinations
		const Resource OFFICE_INDUSTRY = Resource.Software | Resource.Telecom | Resource.Financial | Resource.Media;

		[HarmonyPrefix]
		static bool GetFittingWorkers_Prefix(ref int __result, BuildingData building, BuildingPropertyData properties, int level, IndustrialProcessData processData)
		{
			float baseMultiplier = 2.25f;
			float spaceMultiplier = properties.m_SpaceMultiplier;

			// properties.m_AllowedManufactured gives a bit array of flags of what the building can be used for
			// m_AllowedManufactured represents what is allowed in the building and can span multiple industries
			if (properties.m_AllowedManufactured == OFFICE_INDUSTRY) {
				baseMultiplier = 2.75f; // Offices are slightly more dense than industry... at least in game
				// Accounting for the taller buildings. CS2's 'height' and boosting it for offices which are taller
				// TODO - If we can change the space multipler when loading the prefab (if it actually works this way), then remove it
				spaceMultiplier = math.pow(spaceMultiplier, 1.4f);
			}

			// This result for a new building results in a company with 2/3 of the capacity. The rest of the capacity will be filled as the company grows larger
			__result = Mathf.CeilToInt(processData.m_MaxWorkersPerCell * (float)building.m_LotSize.x * (float)building.m_LotSize.y * (baseMultiplier + 0.25f * (float)level) * spaceMultiplier);
			//System.Console.WriteLine("I(" + level + ") - " + __result);
			return false; // Skip original
		}
	}
}