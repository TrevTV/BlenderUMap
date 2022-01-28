using System.IO;
using System.Linq;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse_Conversion.Meshes;
using System.Diagnostics;

namespace BlenderUMap
{
    internal class MeshMultiExporter
    {
        private UStaticMesh StaticMesh;
        private ELodFormat LodFormat;
        private bool UseUModel;

        public MeshMultiExporter(UStaticMesh originalMesh, ELodFormat exportLod = ELodFormat.FirstLod, bool useUModel = false)
        {
            StaticMesh = originalMesh;
            LodFormat = exportLod;
            UseUModel = useUModel;
        }

        public bool TryWriteToDir(DirectoryInfo directory, out string savedFileName)
        {
            if (UseUModel)
            {
                string launchOptions =
                    $"-path=\"{Config.GameDirectory}\" " +
                    $"-log=log.txt " + 
                    $"-notex " +
                    $"-game=ue4.{Config.GameVersion.ToString().Split('_').Last()} " +
                    $"-out=\"{Program.ExportDirectory.FullName}\" " +
                    $"-export \"{StaticMesh.Outer.GetPathName().Replace($"{Program.FileProvider.GameName}/Content/", "/Game/")}.uasset\" " +
                    $"\"{StaticMesh.GetPathName().Split('.').Last()}\"";
                File.WriteAllText(Path.Combine(Program.CurrentDirectory.FullName, "UModel", "cmd.txt"), launchOptions);
                Process umodel = new();
                umodel.StartInfo.FileName = Path.Combine(Program.CurrentDirectory.FullName, "UModel", "umodel_64.exe");
                umodel.StartInfo.WorkingDirectory = Path.Combine(Program.CurrentDirectory.FullName, "UModel");
                umodel.StartInfo.Arguments = "@cmd.txt";
                umodel.Start();
                umodel.WaitForExit();

                savedFileName = "UModel Export";
                return umodel.ExitCode == 0;
            }
            else
            {
                MeshExporter exporter = new(StaticMesh, LodFormat, false);
                return exporter.TryWriteToDir(directory, out savedFileName);
            }
        }
    }
}
