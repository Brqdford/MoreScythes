using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Wish;

namespace MoreScythes;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class MainPlugin : BaseUnityPlugin
{
    private const string PluginGuid = "org.brqdfordsdevelopmentgroup.morescythes";
    private const string PluginName = "More Scythes";
    private const string PluginVersion = "1.0.9";
    public static ManualLogSource Log { get; private set; }

    private void Awake()
    {
        Log = Logger;
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), PluginGuid);
        Logger.LogInfo("More Scythes plugin has loaded successfully.");
    }


}