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
			if (properties.m_AllowedManufactured == OFFICE_INDUSTRY) {
				// m_AllowedManufactured represents what is allowed in the building and spans multiple industries
				baseMultiplier = 3.75f;
				if (spaceMultiplier > 1.1f) {
					// Accounting for the taller buildings. CS2's 'height'
					spaceMultiplier *= 1.5f;
                }
			}

			// This result for a new building results in a company with 2/3 of the capacity
			__result = Mathf.CeilToInt(processData.m_MaxWorkersPerCell * (float)building.m_LotSize.x * (float)building.m_LotSize.y * (baseMultiplier + 0.25f * (float)level) * spaceMultiplier);
			System.Console.WriteLine("I(" + level + ") - " + __result);
			return false; // Skip original
		}
	}
}