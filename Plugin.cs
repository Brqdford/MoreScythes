using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoreScythes;
using UnityEngine;
using Wish;

[BepInPlugin("org.brqdfordsdevelopmentgroup.morescythes", "More Scythes", "1.0.5")]
public class Plugin : BaseUnityPlugin
{
	[HarmonyPatch]
	private class Patches
	{
		[HarmonyPostfix]
		[HarmonyPatch(typeof(MainMenuController), "Awake")]
		public static void MainMenuControllerAwake()
		{
			try
			{
				ItemHandler.CreateScytheItems();
			}
			catch (Exception ex)
			{
				logger.LogError((object)ex);
			}
		}
		[HarmonyPrefix]
		[HarmonyPatch(typeof(UseItem), "SetPlayer")]
		public static bool UseItemSetPlayer(ref UseItem __instance)
		{
			if (!((Component)__instance).gameObject.activeSelf)
			{
				((Component)__instance).gameObject.SetActive(true);
			}
			return true;
		}
	}

	public const string PLUGIN_GUID = "org.brqdfordsdevelopmentgroup.morescythes";

	public const string PLUGIN_NAME = "More Scythes";

	public const string PLUGIN_VERSION = "1.0.5";

	private Harmony _harmony = new Harmony("org.brqdfordsdevelopmentgroup.morescythes");

	public static ManualLogSource logger;

	private void Awake()
	{
		logger = ((Plugin)this).Logger;
		_harmony.PatchAll();
		((Plugin)this).Logger.LogInfo((object)"More Scythes plugin has loaded successfully.");
	}
}