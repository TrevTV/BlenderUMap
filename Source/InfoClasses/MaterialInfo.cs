using CUE4Parse.UE4.Assets.Exports.Material;
using CUE4Parse.UE4.Assets.Exports.Texture;
using CUE4Parse_Conversion.Textures;
using SkiaSharp;
using System.IO;
using System.Collections.Generic;

namespace BlenderUMap
{
    internal class MaterialInfo
    {
        public string Name;
        public string Path;
        public Dictionary<string, string> Textures;

        public MaterialInfo(UMaterialInstance mat, bool extractTextures)
        {
            Name = mat.Name;
            Path = mat.GetPathName();
            Textures = new Dictionary<string, string>();

            CMaterialParams parameters = new();
            mat.GetParams(parameters);

            var textures = new Dictionary<string, UUnrealMaterial>()
            {
                { "Diffuse", parameters.Diffuse },
                { "Normal", parameters.Normal },
                { "Specular", parameters.Specular },
                { "SpecPower", parameters.SpecPower },
                { "Opacity", parameters.Opacity },
                { "Emissive", parameters.Emissive },
                { "Cube", parameters.Cube },
                { "Mask", parameters.Mask }
            };

            foreach (var t in textures)
            {
                if (t.Value == null) continue;

                if (extractTextures)
                {
                    var texture = Program.FileProvider.LoadObject<UTexture2D>(t.Value.GetPathName());
                    using var data = texture.Decode()?.Encode(SKEncodedImageFormat.Png, 100);
                    var path = System.IO.Path.Combine(t.Value.GetExportDir().FullName, texture.Name + ".png");
                    File.WriteAllBytes(path, data.AsSpan().ToArray());
                    Textures.Add(t.Key, path);
                    continue;
                }

                Textures.Add(t.Key, t.Value.GetPathName());
            }
        }
    }
}