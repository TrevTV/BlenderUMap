using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.Engine;
using CUE4Parse.UE4.Versions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;

namespace BlenderUMap
{
    internal class Program
    {
        public static DefaultFileProvider FileProvider;
        public static DirectoryInfo CurrentDirectory;

        private static void Main()
        {
            CurrentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

            string timestamp = DateTime.Now.ToString("HH-mm-ss.ff");
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File($"Logs/BlenderUMap-{timestamp}.log",
                    rollingInterval: RollingInterval.Infinite,
                    outputTemplate: "[{Timestamp:HH:mm:ss.ff}] [{Level:u3}] {Message}{NewLine}{Exception}")
                .CreateLogger();

            if (!Config.Init())
                return;

            string configValidation = Config.CheckConfigValid();
            if (configValidation != null)
            {
                Log.Logger.Error("[Config] " + configValidation);
                Log.Logger.Information("Press any key to exit...");
                Console.ReadKey();
                return;
            }

            FileProvider = new DefaultFileProvider(Config.GameDirectory, SearchOption.AllDirectories, false, new VersionContainer(Config.GameVersion));
            FileProvider.Initialize();
            foreach (AESKeyInfo aes in Config.EncryptionKeys)
                FileProvider.SubmitKey(aes.FGuid, aes.FAes);
            FileProvider.LoadLocalization();
            BeginExport();

            Log.Logger.Information("Finished export, press any key to exit...");
            Console.ReadKey();
        }

        private static void BeginExport()
        {
            UWorld obj = FileProvider.LoadObject<UWorld>(Config.ExportPackage);
            if (obj == null)
            {
                Log.Logger.Error("Couldn't load object at " + Config.ExportPackage + ", quitting export...");
                return;
            }

            ULevel persistentLevel = obj.PersistentLevel.Load<ULevel>();

            var actors = new List<BaseActorInfo>();
            foreach (var actorLazy in persistentLevel.Actors)
            {
                UObject actor = null;
                actorLazy?.TryLoad(out actor);
                if (actor == null) continue;

                BaseActorInfo info = null;
                info ??= MeshActorInfo.Get(actor);

                if (info != null)
                {
                    actors.Add(info);
                    Log.Logger.Information("Saved object " + info.Name);
                }
                else
                    Log.Logger.Warning($"Failed to retrieve info from {actor.Name}, this object won't be included in the export.");
            }

            File.WriteAllText("_processed.json", JsonConvert.SerializeObject(actors, Formatting.Indented));
        }
    }
}