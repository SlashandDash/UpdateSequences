using HarmonyLib;
using System.Net.Http;
using System.IO;
using System.Threading.Tasks;
using System;

using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;

using BepInEx;
using BepInEx.IL2CPP;
using Cpp2IL.Core;

using SequencedDropGameMode;


namespace UpdateSequences
{
    internal static class Patches
    {
        //   Anti Bepinex detection (Thanks o7Moon: https://github.com/o7Moon/CrabGame.AntiAntiBepinex)
        [HarmonyPatch(typeof(EffectManager), nameof(EffectManager.Method_Private_Void_GameObject_Boolean_Vector3_Quaternion_0))] // Ensures effectSeed is never set to 4200069 (if it is, modding has been detected)
        [HarmonyPatch(typeof(LobbyManager), nameof(LobbyManager.Method_Private_Void_0))] // Ensures connectedToSteam stays false (true means modding has been detected)
        // [HarmonyPatch(typeof(Deobf_MenuSnowSpeedModdingDetector), nameof(Deobf_MenuSnowSpeedModdingDetector.Method_Private_Void_0))] // Would ensure snowSpeed is never set to Vector3.zero (though it is immediately set back to Vector3.one due to an accident on Dani's part lol)
        [HarmonyPrefix]
        internal static bool PreBepinexDetection()
            => false;

        static string pluginDir = Paths.PluginPath;

        static string sequencedDropGameModeDir = Path.GetDirectoryName(typeof(CustomGameModeSequencedDrop).Assembly.Location);

        static string updateSequencesDir = Path.GetDirectoryName(typeof(UpdateSequences).Assembly.Location);
        static string sequencedDropSequencesDir = Path.Combine(sequencedDropGameModeDir, "SequencedDropSequences");

        static string seqRepoSlashOrgDir = Path.Combine(updateSequencesDir, "SequencedDropSequences-master");
        static string seqRepoSlashDir = Path.Combine(sequencedDropSequencesDir, "SequencedDropSequences-master");

        static bool[] isDiff = { true, true, true, true, true };

        static string[] diffString = { Path.Combine(seqRepoSlashDir, "Difficulty", "Easy"),
            Path.Combine(seqRepoSlashDir, "Difficulty", "Normal"),
            Path.Combine(seqRepoSlashDir, "Difficulty", "Hard"),
            Path.Combine(seqRepoSlashDir, "Difficulty", "Harder"),
            Path.Combine(seqRepoSlashDir, "Difficulty", "Insane")};


        static string[] diffOrgString = { Path.Combine(seqRepoSlashOrgDir, "Difficulty", "Easy"),
            Path.Combine(seqRepoSlashOrgDir, "Difficulty", "Normal"),
            Path.Combine(seqRepoSlashOrgDir, "Difficulty", "Hard"),
            Path.Combine(seqRepoSlashOrgDir, "Difficulty", "Harder"),
            Path.Combine(seqRepoSlashOrgDir, "Difficulty", "Insane")};


        


        [HarmonyPatch(typeof(Chatbox), nameof(Chatbox.SendMessage))]
        [HarmonyPrefix]
        static void OnSendMessagePre(string param_1) {
            
            if (param_1 == ".fetch") {

                Task.Run(() => CompareHash());
            }
            if (param_1 == ".enable easy")
            {
                DiffChecker();

                if (isDiff[0]) {
                    Chatbox.Instance.ForceMessage("Easy already enabled");
                    return;
                }

                CopyDirectory(diffOrgString[0], diffString[0], true);

                Chatbox.Instance.ForceMessage("Easy enabled");

            }
            if (param_1 == ".enable normal")
            {
                DiffChecker();

                if (isDiff[1])
                {
                    Chatbox.Instance.ForceMessage("Normal already enabled");
                    return;
                }

                CopyDirectory(diffOrgString[1], diffString[1], true);

                Chatbox.Instance.ForceMessage("Normal enabled");

            }
            if (param_1 == ".enable hard")
            {
                DiffChecker();

                if (isDiff[2])
                {
                    Chatbox.Instance.ForceMessage("Hard already enabled");
                    return;
                }

                CopyDirectory(diffOrgString[2], diffString[2], true);

                Chatbox.Instance.ForceMessage("Hard enabled");

            }
            if (param_1 == ".enable harder")
            {
                DiffChecker();

                if (isDiff[3])
                {
                    Chatbox.Instance.ForceMessage("Harder already enabled");
                    return;
                }

                CopyDirectory(diffOrgString[3], diffString[3], true);

                Chatbox.Instance.ForceMessage("Harder enabled");

            }
            if (param_1 == ".enable insane")
            {
                DiffChecker();

                if (isDiff[4])
                {
                    Chatbox.Instance.ForceMessage("Insane already enabled");
                    return;
                }

                CopyDirectory(diffOrgString[4], diffString[4], true);

                Chatbox.Instance.ForceMessage("Insane enabled");

            }
            if (param_1 == ".enable all")
            {
                DiffChecker();

                for (int i = 0; i < 5; i++)
                {
                    if (!isDiff[i])
                        CopyDirectory(diffOrgString[i], diffString[i], true);
                }

                Chatbox.Instance.ForceMessage("All Diff enabled");

            }
            if (param_1 == ".disable easy")
            {
                DiffChecker();

                if (!isDiff[0])
                {
                    Chatbox.Instance.ForceMessage("Easy already disabled");
                    return;
                }

                Directory.Delete(diffString[0], recursive: true);

                Chatbox.Instance.ForceMessage("Easy disabled");


            }
            if (param_1 == ".disable normal")
            {
                DiffChecker();

                if (!isDiff[1])
                {
                    Chatbox.Instance.ForceMessage("Normal already disabled");
                    return;
                }

                Directory.Delete(diffString[1], recursive: true);

                Chatbox.Instance.ForceMessage("Normal disabled");

            }
            if (param_1 == ".disable hard")
            {
                DiffChecker();

                if (!isDiff[2])
                {
                    Chatbox.Instance.ForceMessage("Hard already disabled");
                    return;
                }

                Directory.Delete(diffString[2], recursive: true);

                Chatbox.Instance.ForceMessage("Hard disabled");

            }
            if (param_1 == ".disable harder")
            {
                DiffChecker();

                if (!isDiff[3])
                {
                    Chatbox.Instance.ForceMessage("Harder already disabled");
                    return;
                }

                Directory.Delete(diffString[3], recursive: true);

                Chatbox.Instance.ForceMessage("Harder disabled");

            }
            if (param_1 == ".disable insane")
            {
                DiffChecker();

                if (!isDiff[4])
                {
                    Chatbox.Instance.ForceMessage("Insane already disabled");
                    return;
                }

                Directory.Delete(diffString[4], recursive: true);

                Chatbox.Instance.ForceMessage("Insane disabled");

            }
            if (param_1 == ".only easy")
            {
                DiffChecker();


                for (int i = 0; i < 5; i++)
                {
                    if (isDiff[i] && i != 0)
                        Directory.Delete(diffString[i], recursive: true);
                }

                DiffChecker();

                if (!isDiff[0])
                    CopyDirectory(diffOrgString[0], diffString[0], true);

                Chatbox.Instance.ForceMessage("Only enabled Easy");

            }
            if (param_1 == ".only normal")
            {
                DiffChecker();


                for (int i = 0; i < 5; i++)
                {
                    if (isDiff[i] && i != 1)
                        Directory.Delete(diffString[i], recursive: true);
                }

                DiffChecker();

                if (!isDiff[1])
                    CopyDirectory(diffOrgString[1], diffString[1], true);

                Chatbox.Instance.ForceMessage("Only enabled Normal");

            }
            if (param_1 == ".only hard")
            {
                DiffChecker();


                for (int i = 0; i < 5; i++)
                {
                    if (isDiff[i] && i != 2)
                        Directory.Delete(diffString[i], recursive: true);
                }

                DiffChecker();

                if (!isDiff[2])
                    CopyDirectory(diffOrgString[2], diffString[2], true);

                Chatbox.Instance.ForceMessage("Only enabled Hard");

            }
            if (param_1 == ".only harder")
            {
                DiffChecker();


                for (int i = 0; i < 5; i++)
                {
                    if (isDiff[i] && i != 3)
                        Directory.Delete(diffString[i], recursive: true);
                }

                DiffChecker();

                if (!isDiff[3])
                    CopyDirectory(diffOrgString[3], diffString[3], true);

                Chatbox.Instance.ForceMessage("Only enabled Harder");

            }
            if (param_1 == ".only insane")
            {
                DiffChecker();


                for (int i = 0; i < 5; i++)
                {
                    if (isDiff[i] && i != 4)
                        Directory.Delete(diffString[i], recursive: true);
                }

                DiffChecker();

                if (!isDiff[4])
                    CopyDirectory(diffOrgString[4], diffString[4], true);

                Chatbox.Instance.ForceMessage("Only enabled Insane");

            }
            if (param_1 == ".disable all")
            {
                DiffChecker();

                for (int i = 0; i < 5; i++) {
                    if (isDiff[i])
                        Directory.Delete(diffString[i], recursive: true);
                }

                Chatbox.Instance.ForceMessage("All Diff disabled");

            }
            if (param_1 == ".list")
            {
                string[] diffName = { "Easy", "Normal", "Hard", "Harder", "Insane" };

                DiffChecker();

                for (int i = 0; i < 5; i++) {

                    Chatbox.Instance.ForceMessage(diffName[i] + " = " + isDiff[i].ToString());
                }
            }
            if (param_1 == ".help")
            {
                Chatbox.Instance.ForceMessage("-----------------------------------------------");
                Chatbox.Instance.ForceMessage(".fetch | downloads/updates seqs");
                Chatbox.Instance.ForceMessage(".list | lists all enabled/disabled difficulties");
                Chatbox.Instance.ForceMessage(".help | shows all available commands");
                Chatbox.Instance.ForceMessage(".enable [DIFFICULTY]");
                Chatbox.Instance.ForceMessage(".disable [DIFFICULTY]");
                Chatbox.Instance.ForceMessage(".only [DIFFICULTY]");
                Chatbox.Instance.ForceMessage("[DIFFICULTY] = easy, normal, hard, harder, insane");
            }
        }

        static async Task DownloadSeqs()
        {
            Chatbox.Instance.ForceMessage("Downloading...");

            string zipPath = Path.Combine(updateSequencesDir, "temp.zip");

            try
            {
                // Delete old folder completely
                if (Directory.Exists(seqRepoSlashOrgDir))
                    Directory.Delete(seqRepoSlashOrgDir, true);

                if (Directory.Exists(seqRepoSlashDir))
                    Directory.Delete(seqRepoSlashDir, true);

                await DownloadAndExtractZipAsync(
                    "https://github.com/SlashandDash/SequencedDropSequences/archive/refs/heads/master.zip",
                    zipPath,
                    updateSequencesDir);

                File.Delete(zipPath);

                CopyDirectory(seqRepoSlashOrgDir, seqRepoSlashDir, true);


                Chatbox.Instance.ForceMessage("Done.");
            }
            catch (Exception ex)
            {
                Chatbox.Instance.ForceMessage("Error: " + ex.Message);
            }

        }


        static async Task DownloadHashCommit()
        {
            string filePath = Path.Combine(updateSequencesDir, "commit_hash.txt");

            if (File.Exists(filePath))
                File.Delete(filePath);

            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "CSharpApp");

            string json = await client.GetStringAsync(
                "https://api.github.com/repos/SlashandDash/SequencedDropSequences/commits"
            );

            // Extract the first "sha" manually
            int shaIndex = json.IndexOf("\"sha\":\"") + 7;
            int shaEnd = json.IndexOf("\"", shaIndex);
            string sha = json.Substring(shaIndex, shaEnd - shaIndex);

            await File.WriteAllTextAsync(filePath, sha);

            Console.WriteLine($"Latest commit hash saved to {filePath}");
        }

        
        static void CompareHash() 
        {
            string oldHash = Path.Combine(updateSequencesDir, "commit_hash_old.txt");
            string newHash = Path.Combine(updateSequencesDir, "commit_hash.txt");

            DownloadHashCommit().GetAwaiter().GetResult();

            Chatbox.Instance.ForceMessage("Downloading Hash Commit");

            if (!File.Exists(oldHash)) {
                File.Move(newHash, oldHash);
                DownloadSeqs().GetAwaiter().GetResult();
                return;
            }


            if (AreFilesEqual(oldHash, newHash))
            {
                File.Delete(newHash);
                Chatbox.Instance.ForceMessage("Up to date");
            }
            else 
            {

                File.Delete(oldHash);
                File.Move(newHash, oldHash);

                DownloadSeqs().GetAwaiter().GetResult();

                for (int i = 0; i < 5; i++){
                    if (!isDiff[i])
                    {
                        Directory.Delete(diffString[i], true);
                    }    
                }
            }

        }



        // Function from Gibson Mod Template

        public static async Task DownloadAndExtractZipAsync(string URL, string destinationFolderPath, string extractFolderPath)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(URL);
                response.EnsureSuccessStatusCode();

                using (FileStream fileStream = new FileStream(destinationFolderPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    // Copy the content from the response message to the file stream
                    await response.Content.CopyToAsync(fileStream);
                }
            }

            // Ensure the extract path exists
            Directory.CreateDirectory(extractFolderPath);
            ZipFile.ExtractToDirectory(destinationFolderPath, extractFolderPath, true);
        }

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            var dir = new DirectoryInfo(sourceDir);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();

            Directory.CreateDirectory(destinationDir);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                file.CopyTo(targetFilePath);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }

        static bool AreFilesEqual(string path1, string path2)
        {
            if (!File.Exists(path1) || !File.Exists(path2))
                return false;

            string text1 = File.ReadAllText(path1);
            string text2 = File.ReadAllText(path2);

            return text1 == text2;
        }


        static void DiffChecker() {

            for (int i = 0; i < 5; i++) {
                if (Directory.Exists(diffString[i])){

                    isDiff[i] = true;
                }
                else {
                    isDiff[i] = false;
                }
            }
        }
    }
}
