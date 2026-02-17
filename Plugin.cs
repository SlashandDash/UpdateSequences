using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Globalization;

namespace UpdateSequences
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("lammas123.SequencedDropGameMode", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class UpdateSequences : BasePlugin
    {
        internal static UpdateSequences Instance { get; private set; }

        public override void Load()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            Instance = this;

            Harmony harmony = new(MyPluginInfo.PLUGIN_NAME);
            harmony.PatchAll(typeof(Patches));

            Log.LogInfo($"Initialized [{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION}]");

        }
    }
}
