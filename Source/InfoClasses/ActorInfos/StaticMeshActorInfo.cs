using System.IO;
using System.Collections.Generic;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Component.StaticMesh;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse_Conversion.Meshes;
using Serilog;

namespace BlenderUMap
{
    internal class StaticMeshActorInfo : BaseActorInfo
    {
        public override string Type => "StaticMeshActor";

        public static StaticMeshActorInfo Get(UObject actor, bool exportMesh = true, bool readMaterials = true)
        {
            Log.Logger.Information("Attempting mesh read of actor " + actor.Name);
            var staticMeshComponent = actor.GetOrDefaultLazy<UStaticMeshComponent>("StaticMeshComponent").Value;
            if (staticMeshComponent == null)
                return null;

            var staticMesh = staticMeshComponent.GetOrDefault<UStaticMesh>("StaticMesh");
            var materials = new List<MaterialInfo>();
            if (staticMesh != null && exportMesh)
            {
                MeshMultiExporter exporter = new(staticMesh, ELodFormat.FirstLod, Config.UseUModel);
                if (exporter.TryWriteToDir(Program.ExportDirectory, out string savedFileName))
                {
                    Log.Logger.Information("Exported mesh to " + staticMesh.GetExportDir());
                }
                if (staticMesh.Materials != null && readMaterials)
                {
                    foreach (var matLazy in staticMesh.Materials)
                    {
                        if (matLazy.TryLoad(out var mat))
                            materials.Add(new MaterialInfo((UMaterialInterface)mat, true));
                        else
                            Log.Logger.Warning("Failed to read material for " + staticMesh.Name);
                    }
                }
            }
            else if (staticMesh == null) Log.Logger.Warning("Failed to find StaticMesh on " + staticMeshComponent.Name);

            StaticMeshActorInfo info = new()
            {
                Name = actor.Name,
                DirPath = staticMesh == null ? string.Empty :
                          (Config.UseUModel
                          ? Path.Combine(staticMesh.GetExportDir().FullName.Replace($"{Program.FileProvider.GameName}\\Content\\", "\\Game\\"), $"{staticMesh.Name}.pskx")
                          : Path.Combine(staticMesh.GetExportDir().FullName, $"{staticMesh.Name}_LOD0.pskx")),
                StaticMesh = staticMesh,
                Position = staticMeshComponent.GetOrDefault<FVector>("RelativeLocation") / 100,
                Rotation = staticMeshComponent.GetOrDefault<FRotator>("RelativeRotation"),
                Scale = staticMeshComponent.GetOrDefault<FVector>("RelativeScale3D"),
                Materials = materials,
                Children = null
            };

            return info;
        }

        public string DirPath;
        [Newtonsoft.Json.JsonIgnore] public UStaticMesh StaticMesh;
        public FVector Position;
        public FRotator Rotation;
        public FVector Scale;
        public List<MaterialInfo> Materials;
        public List<UObject> Children;
    }
}
