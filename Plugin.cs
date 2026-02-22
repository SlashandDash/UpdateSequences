using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Globalization;

using BepInEx.Configuration;


namespace UpdateSequences
{
    //[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency("lammas123.SequencedDropGameMode", BepInDependency.DependencyFlags.SoftDependency)]
    public sealed class UpdateSequences : BasePlugin
    {
        internal static UpdateSequences Instance { get; private set; }



        public ConfigEntry<bool> BooleanLockDiff;

        public ConfigEntry<bool> BooleanEasy;
        public ConfigEntry<bool> BooleanNormal;
        public ConfigEntry<bool> BooleanHard;
        public ConfigEntry<bool> BooleanHarder;
        public ConfigEntry<bool> BooleanInsane;

        public ConfigEntry<bool> BooleanAutoFetch;


        public override void Load()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.CurrentUICulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            Instance = this;

            BooleanAutoFetch = Config.Bind(
                "Settings",
                "Auto_fetch",
                true,
                "Updates/Downloads sequences everytime you create a lobby."
            );

            BooleanLockDiff = Config.Bind(
                "Settings",
                "Difficulty_config",
                false,
                "Define Dificulty in the config. (Reset the Diff to the config every restart)"
            );

            BooleanEasy = Config.Bind(
                "Settings",
                "Easy",
                true,
                "Enable or disable Easy difficulty."
            );

            BooleanNormal = Config.Bind(
                "Settings",
                "Normal",
                true,
                "Enable or disable Normal difficulty."
            );

            BooleanHard = Config.Bind(
                "Settings",
                "Hard",
                true,
                "Enable or disable Hard difficulty."
            );
            
            BooleanHarder = Config.Bind(
                "Settings",
                "Harder",
                true,
                "Enable or disable Harder difficulty."
            );
            
            BooleanInsane = Config.Bind(
                "Settings",
                "Insane",
                true,
                "Enable or disable Insane difficulty."
            );

            Patches.DiffCheckerSupreme();


            Harmony harmony = new(MyPluginInfo.PLUGIN_NAME);
            harmony.PatchAll(typeof(Patches));

            Log.LogInfo($"Initialized [{MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION}]");

        }
    }
}
