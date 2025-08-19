global using BepInEx;
global using BepInEx.IL2CPP;
global using HarmonyLib;
using System;
using UnhollowerRuntimeLib;
using UnityEngine;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace UpdateSequences
{
    //[BepInPlugin("751a4f5b-8152-4e32-8a3f-6c8f571d43f6", "ModTemplate", "1.0.0")]
    [BepInPlugin("751a4f5b-8152-4e32-8a3f-6c8f571d43f6", "UpdateSequences", "1.0.0")]
    public class Plugin : BasePlugin
    {
        static string trueCommitHash_path = System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\true_CommitHash.txt";
        static string tempCommitHash_path = System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\temp_CommitHash.txt";
        static string configFile = System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\config\\SeqRepoUpdate_config.txt";
        static string seqDirectoryMaster = System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\SequencedDropSequences-master";
        static string seqDirectory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\SequencedDropSequences";

        static string[] seqDirectoryDifficulty = {
            System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\SequencedDropSequences\\Difficulty\\Easy",
            System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\SequencedDropSequences\\Difficulty\\Normal",
            System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\SequencedDropSequences\\Difficulty\\Hard",
            System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\SequencedDropSequences\\Difficulty\\Harder",
            System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\SequencedDropSequences\\Difficulty\\Insane"
        };

        static bool[] seqBoolDifficulty = { true, true, true, true, true };

        public override async void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
            Log.LogInfo("Mod created by Slash and Dash");
            ClassInjector.RegisterTypeInIl2Cpp<MyClass>();


            if (!Directory.Exists(seqDirectory))
            {
                Directory.CreateDirectory(seqDirectory);
            }

            if (!File.Exists(trueCommitHash_path))
            {
                using (StreamWriter writer = new StreamWriter(trueCommitHash_path))
                {
                    writer.WriteLine("Nothing");
                }
            }

            string orgTextHash = File.ReadAllText(trueCommitHash_path);

            await getCommitHash("https://api.github.com/repos/SlashandDash/SequencedDropSequences/commits/master", tempCommitHash_path);
            string newTextHash = File.ReadAllText(tempCommitHash_path);

            if (orgTextHash != newTextHash)
            {
                File.Delete(trueCommitHash_path);
                await getCommitHash("https://api.github.com/repos/SlashandDash/SequencedDropSequences/commits/master", trueCommitHash_path);

                if (File.Exists(seqDirectory + "\\README.md"))
                {
                    File.Delete(seqDirectory + "\\README.md");
                    foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                    {
                        File.Delete(file);
                    }
                }

                if (Directory.Exists(seqDirectory + "\\Difficulty"))
                {
                    Directory.Delete(seqDirectory + "\\Difficulty", true);
                }

                await downloadTs("https://github.com/SlashandDash/SequencedDropSequences/archive/refs/heads/master.zip", System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\a.zip", System.IO.Directory.GetParent(Application.dataPath).ToString(), seqDirectory);

                createConfigFile();
                CopyFiles();
            }

            File.Delete(tempCommitHash_path);
        }


        public class MyClass : MonoBehaviour
        {
            string seqDirectory = System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\BepInEx\\plugins\\SequencedDropSequences\\";
            void Update()
            {
            }
        }

        public static async Task getCommitHash(string URL, string destinationFolderPath)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                HttpResponseMessage response = await client.GetAsync(URL);
                response.EnsureSuccessStatusCode();
                using (FileStream fileStream = new FileStream(destinationFolderPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
                string content = await File.ReadAllTextAsync(destinationFolderPath);
                MatchCollection matches = Regex.Matches(content, "\"([^\"]*)\"");

                if (matches.Count >= 2)
                {
                    string secondQuotedString = matches[1].Groups[1].Value;

                    await File.WriteAllTextAsync(destinationFolderPath, secondQuotedString);
                }
            }
        }

        public static async Task downloadTs(string URL, string destinationFolderPath, string extractFolderPath, string seqDirectory)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "C# App");
                HttpResponseMessage response = await client.GetAsync(URL);
                response.EnsureSuccessStatusCode();
                using (FileStream fileStream = new FileStream(destinationFolderPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }
            }

            Directory.CreateDirectory(extractFolderPath);
            ZipFile.ExtractToDirectory(destinationFolderPath, extractFolderPath, true);


            string[] entries = Directory.GetFileSystemEntries(seqDirectoryMaster);

            foreach (string entry in entries)
            {
                string entryName = Path.GetFileName(entry);
                string destEntry = Path.Combine(seqDirectory, entryName);

                if (Directory.Exists(entry))
                {
                    Directory.Move(entry, destEntry);
                }
                else
                {
                    File.Move(entry, destEntry);
                }
            }

            File.Delete(destinationFolderPath);
            Directory.Delete(seqDirectoryMaster, true);
        }

        public static void CopyFiles()
        {
            for (int i = 0; i < 5; i++)
            {
                if (seqBoolDifficulty[i] == true)
                {
                    if (Directory.Exists(seqDirectoryDifficulty[i]))
                    {
                        string[] files = Directory.GetFiles(seqDirectoryDifficulty[i]);
                        foreach (string file in files)
                        {
                            string fileName = Path.GetFileName(file);
                            string destFile = Path.Combine(seqDirectory, fileName);
                            File.Copy(file, destFile, true);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Directory does not exist: {seqDirectoryDifficulty[i]}");
                    }
                }
            }
        }

        public static void createConfigFile()
        {
            if (!File.Exists(configFile))
            {
                using (StreamWriter writer = new StreamWriter(configFile))
                {
                    writer.WriteLine("Easy = true");
                    writer.WriteLine("Normal = true");
                    writer.WriteLine("Hard = true");
                    writer.WriteLine("Harder = true");
                    writer.WriteLine("Insane = true");
                }
            }

            string[] lines = File.ReadAllLines(configFile);

            string easyLine = lines.Length > 0 ? lines[0] : null;
            string normalLine = lines.Length > 1 ? lines[1] : null;
            string hardLine = lines.Length > 2 ? lines[2] : null;
            string harderLine = lines.Length > 3 ? lines[3] : null;
            string insaneLine = lines.Length > 4 ? lines[4] : null;


            seqBoolDifficulty[0] = easyLine != "Easy = false";
            seqBoolDifficulty[1] = normalLine != "Normal = false";
            seqBoolDifficulty[2] = hardLine != "Hard = false";
            seqBoolDifficulty[3] = harderLine != "Harder = false";
            seqBoolDifficulty[4] = insaneLine != "Insane = false";

        }

        static void lineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }



        [HarmonyPatch(typeof(GameUiChatBox), nameof(GameUiChatBox.SendMessage))]
        [HarmonyPrefix]
        static async void OnSendMessagePre(string param_1)
        {
            if (param_1 == "seq fetch")
            {
                GameUiChatBox.Instance.ForceMessage("Fetching Sequenced Repository ...");

                if (!Directory.Exists(seqDirectory))
                {
                    Directory.CreateDirectory(seqDirectory);
                }


                if (!File.Exists(trueCommitHash_path))
                {
                    using (StreamWriter writer = new StreamWriter(trueCommitHash_path))
                    {
                        writer.WriteLine("Nothing");
                    }
                }

                string orgTextHash = File.ReadAllText(trueCommitHash_path);

                await getCommitHash("https://api.github.com/repos/SlashandDash/SequencedDropSequences/commits/master", tempCommitHash_path);
                string newTextHash = File.ReadAllText(tempCommitHash_path);

                if (orgTextHash != newTextHash)
                {
                    GameUiChatBox.Instance.ForceMessage("New Seq file detected in the repository, updating ...");

                    File.Delete(trueCommitHash_path);
                    await getCommitHash("https://api.github.com/repos/SlashandDash/SequencedDropSequences/commits/master", trueCommitHash_path);

                    if (File.Exists(seqDirectory + "\\README.md"))
                    {
                        File.Delete(seqDirectory + "\\README.md");

                        foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                        {
                            File.Delete(file);
                        }
                    }

                    if (Directory.Exists(seqDirectory + "\\Difficulty"))
                    {
                        Directory.Delete(seqDirectory + "\\Difficulty", true);
                    }


                    await downloadTs("https://github.com/SlashandDash/SequencedDropSequences/archive/refs/heads/master.zip", System.IO.Directory.GetParent(Application.dataPath).ToString() + "\\a.zip", System.IO.Directory.GetParent(Application.dataPath).ToString(), seqDirectory);

                    createConfigFile();
                    CopyFiles();

                    GameUiChatBox.Instance.ForceMessage("Update completed!");
                }
                else
                {
                    GameUiChatBox.Instance.ForceMessage("Repository is already up to date!");
                }

                File.Delete(tempCommitHash_path);
            }

            if (param_1 == "seq disable easy")
            {
                if (seqBoolDifficulty[0] == false)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Easy seq is already disalbe!");
                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Disabling easy Seqs ...");

                lineChanger("Easy = false", configFile, 1);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq enable easy")
            {
                if (seqBoolDifficulty[0] == true)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Easy seq is already enabled!");
                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Enabling easy Seqs ...");

                lineChanger("Easy = true", configFile, 1);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq disable normal")
            {
                if (seqBoolDifficulty[1] == false)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Normal seq is already disabled!");
                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Disabling normal Seqs ...");

                lineChanger("Normal = false", configFile, 2);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq enable normal")
            {


                if (seqBoolDifficulty[1] == true)
                {

                    GameUiChatBox.Instance.ForceMessage("Error: Normal seq is already enabled!");

                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Enabling normal Seqs ...");

                lineChanger("Normal = true", configFile, 2);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq disable hard")
            {
                if (seqBoolDifficulty[2] == false)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Hard seq is already disabled!");
                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Disabling hard Seqs ...");

                lineChanger("Hard = false", configFile, 3);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq enable hard")
            {
                if (seqBoolDifficulty[2] == true)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Hard seq is already enabled!");
                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Enabling hard Seqs ...");

                lineChanger("Hard = true", configFile, 3);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq disable harder")
            {
                if (seqBoolDifficulty[3] == false)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Harder seq is already disabled!");

                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Disabling harder Seqs ...");

                lineChanger("Harder = false", configFile, 4);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq enable harder")
            {
                if (seqBoolDifficulty[3] == true)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Harder seq is already enabled!");

                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Enabling harder Seqs ...");

                lineChanger("Harder = true", configFile, 4);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq disable insane")
            {
                if (seqBoolDifficulty[4] == false)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Insane seq is already disabled!");

                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Disabling insane Seqs ...");

                lineChanger("Insane = false", configFile, 5);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq enable insane")
            {
                if (seqBoolDifficulty[4] == true)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: Insane seq is already enabled!");

                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Enabling insane Seqs ...");

                lineChanger("Insane = true", configFile, 5);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();

            }

            if (param_1 == "seq list")
            {
                GameUiChatBox.Instance.ForceMessage("Easy = " + seqBoolDifficulty[0].ToString());
                GameUiChatBox.Instance.ForceMessage("Normal = " + seqBoolDifficulty[1].ToString());
                GameUiChatBox.Instance.ForceMessage("Hard = " + seqBoolDifficulty[2].ToString());
                GameUiChatBox.Instance.ForceMessage("Harder = " + seqBoolDifficulty[3].ToString());
                GameUiChatBox.Instance.ForceMessage("Insane = " + seqBoolDifficulty[4].ToString());
            }

            if (param_1 == "seq enable all")
            {

                if (seqBoolDifficulty[0] == true && seqBoolDifficulty[1] == true && seqBoolDifficulty[2] == true && seqBoolDifficulty[3] == true && seqBoolDifficulty[4] == true)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: All seq are already enabled!");

                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Enabling all Seqs ...");

                lineChanger("Easy = true", configFile, 1);
                lineChanger("Normal = true", configFile, 2);
                lineChanger("Hard = true", configFile, 3);
                lineChanger("Harder = true", configFile, 4);
                lineChanger("Insane = true", configFile, 5);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();
            }

            if (param_1 == "seq disable all")
            {

                if (seqBoolDifficulty[0] == false && seqBoolDifficulty[1] == false && seqBoolDifficulty[2] == false && seqBoolDifficulty[3] == false && seqBoolDifficulty[4] == false)
                {
                    GameUiChatBox.Instance.ForceMessage("Error: All seq are already disabled!");

                    return;
                }

                GameUiChatBox.Instance.ForceMessage("Disabling all Seqs ...");

                lineChanger("Easy = false", configFile, 1);
                lineChanger("Normal = false", configFile, 2);
                lineChanger("Hard = false", configFile, 3);
                lineChanger("Harder = false", configFile, 4);
                lineChanger("Insane = false", configFile, 5);
                createConfigFile();

                foreach (string file in Directory.GetFiles(seqDirectory, "[*] *.txt"))
                {
                    File.Delete(file);
                }

                CopyFiles();
            }


            if (param_1 == "seq help")
            {
                GameUiChatBox.Instance.ForceMessage("seq help");
                GameUiChatBox.Instance.ForceMessage("seq fetch");
                GameUiChatBox.Instance.ForceMessage("seq list");
                GameUiChatBox.Instance.ForceMessage("seq enable/disable [DIFFICULTY]");
                GameUiChatBox.Instance.ForceMessage("seq enable/disable all");
                GameUiChatBox.Instance.ForceMessage("[DIFFICULTY] = easy, normal, hard, harder, insane");
            }

        }

        [HarmonyPatch(typeof(GameUi), nameof(GameUi.Awake))]
        [HarmonyPostfix]
        public static void UIAwakePatch(GameUi __instance)
        {
            GameObject pluginObj = new();
            pluginObj.AddComponent<MyClass>();
            pluginObj.transform.SetParent(__instance.transform);
        }

        //Antibepinex Bypass (dont touch, it just let you join public lobby with mods)
        [HarmonyPatch(typeof(EffectManager), "Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0")]
        [HarmonyPatch(typeof(LobbyManager), "Method_Private_Void_0")]
        [HarmonyPatch(typeof(MonoBehaviourPublicVesnUnique), "Method_Private_Void_0")]
        [HarmonyPatch(typeof(MenuUiCreateLobbySettings), "Method_Public_Void_PDM_2")]
        [HarmonyPatch(typeof(MonoBehaviourPublicTeplUnique), "Method_Private_Void_PDM_32")]
        [HarmonyPrefix]
        public static bool Prefix(System.Reflection.MethodBase __originalMethod)
        {
            return false;
        }
    }
}

