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
    internal class MeshActorInfo : BaseActorInfo
    {
        public static MeshActorInfo Get(UObject actor)
        {
            Log.Logger.Information("Attempting mesh read of actor " + actor.Name);
            var staticMeshComponent = actor.GetOrDefaultLazy<UStaticMeshComponent>("StaticMeshComponent").Value;
            if (staticMeshComponent == null) return null;

            var staticMesh = staticMeshComponent.GetOrDefault<UStaticMesh>("StaticMesh");
            var materials = new List<MaterialInfo>();
            if (staticMesh != null)
            {
                MeshExporter exporter = new(staticMesh, ELodFormat.FirstLod, false);
                if (exporter.TryWriteToDir(Program.CurrentDirectory, out string savedFileName))
                {
                    Log.Logger.Information("Exported mesh to " + staticMesh.GetExportDir());
                    Log.Logger.Information(savedFileName);
                }
                if (staticMesh.Materials != null)
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
            else Log.Logger.Warning("Failed to find StaticMesh on " + staticMeshComponent.Name);

            MeshActorInfo info = new()
            {
                Name = actor.Name,
                DirPath = staticMesh == null ? string.Empty : Path.Combine(staticMesh?.GetExportDir().FullName, $"{staticMesh.Name}_LOD0.pskx"),
                Position = staticMeshComponent.GetOrDefault<FVector>("RelativeLocation"),
                Rotation = staticMeshComponent.GetOrDefault<FRotator>("RelativeRotation"),
                Scale = staticMeshComponent.GetOrDefault<FVector>("RelativeScale3D"),
                Materials = materials,
                Children = null
            };

            return info;
        }

        public string DirPath;
        public FVector Position;
        public FRotator Rotation;
        public FVector Scale;
        public List<MaterialInfo> Materials;
        public List<UObject> Children;
    }
}
