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
using System;

namespace WG_FittingWorkers.Patches
{
	[HarmonyPatch(typeof(CommercialAISystem), nameof(CommercialAISystem.GetFittingWorkers))]
	class CommercialPatch
	{
		[HarmonyPrefix]
		static bool GetFittingWorkers_Prefix(ref int __result, BuildingData building, BuildingPropertyData properties, int level, ServiceCompanyData serviceData)
		{
			// This result for a new building results in a company with 2/3 of the capacity
			__result = Mathf.CeilToInt(serviceData.m_MaxWorkersPerCell * (float)building.m_LotSize.x * (float)building.m_LotSize.y * (2.25f + 0.25f * (float)level) * properties.m_SpaceMultiplier);
			System.Console.WriteLine("C(" + level + ") - " + __result);
			return false; // Skip original
		}
	}
}