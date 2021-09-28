using System;
using System.IO;
using Serilog;
using Newtonsoft.Json;
using CUE4Parse.UE4.Versions;

namespace BlenderUMap
{
    internal class Config
    {
        public static string GameDirectory => instance?.gameDir;
        public static EGame GameVersion => (EGame)Enum.Parse(typeof(EGame), instance?.gameVersion);
        public static AESKeyInfo[] EncryptionKeys => instance?.encryptionKeys;
        public static string ExportPackage => instance?.exportPackage;

        private static string configPath;
        private static Config instance;

        public static bool Init()
        {
            configPath = "config.json";
            if (!File.Exists(configPath))
            {
                instance = new Config();
                File.WriteAllText(configPath, JsonConvert.SerializeObject(instance, Formatting.Indented));
                Log.Logger.Information("A new config file has been generated, please configure it and try again.");
                Log.Logger.Information("Press any key to exit...");
                Console.ReadKey();
                return false;
            }

            instance = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configPath));
            return true;
        }

        public static string CheckConfigValid()
        {
            if (!Directory.Exists(instance.gameDir))
                return "GameDirectory not found, please put the correct directory and try again.";
            if (!Enum.IsDefined(typeof(EGame), instance.gameVersion))
                return "UEVersion is invalid, please choose a proper version and try again.";

            return null;
        }

        [JsonProperty("GameDirectory")]
        private readonly string gameDir = "C:\\Program Files\\Epic Games\\Fortnite\\FortniteGame\\Content\\Paks";
        [JsonProperty("UEVersion")]
        private readonly string gameVersion = "GAME_UE4_LATEST";
        [JsonProperty("EncryptionKeys")]
        private readonly AESKeyInfo[] encryptionKeys = new AESKeyInfo[] { new() };
        [JsonProperty("ExportPackage")]
        private readonly string exportPackage = "/Game/Athena/Apollo/Maps/Buildings/3x3/Apollo_3x3_BoatRental";
    }
}
