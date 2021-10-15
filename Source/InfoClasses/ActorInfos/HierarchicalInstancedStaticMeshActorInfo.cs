using System.IO;
using System.Collections.Generic;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.StaticMesh;
using CUE4Parse.UE4.Objects.Core.Math;
using CUE4Parse_Conversion.Meshes;
using Serilog;
using System.Linq;
using Newtonsoft.Json.Linq;
using CUE4Parse.UE4.Assets.Objects;

namespace BlenderUMap
{
    internal class HierarchicalInstancedStaticMeshActorInfo : BaseActorInfo
    {
        public override string Type => "HierarchicalInstancedStaticMeshActor";

        public static HierarchicalInstancedStaticMeshActorInfo Get(UObject actor, bool exportMesh = true, bool readMaterials = true)
        {
            Log.Logger.Information("Attempting instanced mesh read of actor " + actor.Name);
            var staticMeshComponent = actor.GetOrDefaultLazy<UHierarchicalInstancedStaticMeshComponent>("RootComponent").Value;
            if (staticMeshComponent == null)
                return null;

            var staticMesh = staticMeshComponent.GetOrDefault<UStaticMesh>("StaticMesh");
            var materials = new List<MaterialInfo>();
            if (staticMesh != null && exportMesh)
            {
                MeshMultiExporter exporter = new(staticMesh, ELodFormat.FirstLod, Config.UseUModel);
                if (exporter.TryWriteToDir(Program.CurrentDirectory, out string savedFileName))
                {
                    Log.Logger.Information("Exported mesh to " + staticMesh.GetExportDir());
                }
                if (staticMesh.Materials != null && readMaterials)
                {
                    foreach (var matLazy in staticMesh.Materials)
                    {
                        if (matLazy.TryLoad<UMaterialInstance>(out var mat))
                            materials.Add(new MaterialInfo(mat, true));
                        else
                            Log.Logger.Warning("Failed to read material for " + staticMesh.Name);
                    }
                }
            }
            else if (staticMesh == null) Log.Logger.Warning("Failed to find StaticMesh on " + staticMeshComponent.Name);

            string dirPath =
                staticMesh == null ? string.Empty :
                (Config.UseUModel
                ? Path.Combine(staticMesh.GetExportDir().FullName.Replace($"{Program.FileProvider.GameName}\\Content\\", "\\Game\\"), $"{staticMesh.Name}.pskx")
                : Path.Combine(staticMesh.GetExportDir().FullName, $"{staticMesh.Name}_LOD0.pskx"));

            List<StaticMeshActorInfo> staticMeshActors = new();

            var instanceComponents = ((ArrayProperty)actor.Properties.SingleOrDefault(p => p.Name.Text == "InstanceComponents").Tag).Value.Properties;
            for (int i = 0; i < instanceComponents.Count; i++)
            {
                ObjectProperty obj = (ObjectProperty)instanceComponents[i];
                var hism = obj.Value.Load<UHierarchicalInstancedStaticMeshComponent>();


                FVector position = default;
                var bounds = staticMeshComponent.Properties.SingleOrDefault(p => p.Name.Text == "BuiltInstanceBounds")?.Tag;
                if (bounds != null)
                {
                    JObject jObj = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(bounds, Newtonsoft.Json.Formatting.Indented));
                    position = new()
                    {
                        X = (float)jObj["Max"]["X"],
                        Y = (float)jObj["Max"]["Y"],
                        Z = (float)jObj["Max"]["Z"]
                    };
                }

                staticMeshActors.Add(new()
                {
                    Name = actor.Name + "_" + i,
                    DirPath = string.Empty,
                    StaticMesh = staticMesh,
                    Position = position,
                    Rotation = staticMeshComponent.GetOrDefault<FRotator>("RelativeRotation"),
                    Scale = staticMeshComponent.GetOrDefault<FVector>("RelativeScale3D") / 10,
                    Materials = null,
                    Children = null
                });
            }

            HierarchicalInstancedStaticMeshActorInfo hInfo = new()
            {
                Name = actor.Name,
                DirPath = dirPath,
                Materials = materials,
                InstancedMeshes = staticMeshActors
            };

            return hInfo;
        }

        public string DirPath;
        public List<MaterialInfo> Materials;
        public List<StaticMeshActorInfo> InstancedMeshes;
    }
}
