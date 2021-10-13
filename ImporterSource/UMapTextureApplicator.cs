using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class UMapTextureApplicator : EditorWindow
{
    private string umapPath;
    private string textureImportDir = "Assets/UMapTextures/";
    private GameObject rootObject;

    private readonly Dictionary<string, string> textureShaderMapping =
        new Dictionary<string, string>()
            {
                { "Diffuse", "_MainTex" },
                { "Normal", "_BumpMap" },
                { "Specular", "" },
                { "SpecPower", "" },
                { "Opacity", "" },
                { "Emissive", "_EmissionMap" },
                { "Cube", "" },
                { "Mask", "" }
            };

    [MenuItem("Tools/UMap Texture Applicator")]
    public static void OpenWindow()
        => GetWindow<UMapTextureApplicator>("Texture Applicator");

    private void OnGUI()
    {
        GUILayout.Label("UMap Texture Applicator", EditorStyles.centeredGreyMiniLabel);
        GUILayout.Space(20);

        umapPath = EditorGUILayout.TextField("BlenderUMap Path", umapPath);
        textureImportDir = EditorGUILayout.TextField("Texture Import Path", textureImportDir);
        rootObject = (GameObject)EditorGUILayout.ObjectField("Root Object", rootObject, typeof(GameObject), true);

        if (GUILayout.Button("Begin Import"))
            ImportTextures();
    }

    private void ImportTextures()
    {
        // create import dir
        if (!Directory.Exists(textureImportDir))
            Directory.CreateDirectory(textureImportDir);

        // deal with unity's asset system and jankily import all png textures as Texture2Ds
        List<Texture2D> allTextures = new List<Texture2D>();
        string[] texturePaths = Directory.GetFiles(umapPath, "*.png", SearchOption.AllDirectories);
        foreach (string texture in texturePaths)
        {
            if (!File.Exists(Path.Combine(textureImportDir, Path.GetFileName(texture))))
                FileUtil.CopyFileOrDirectory(texture, Path.Combine(textureImportDir, Path.GetFileName(texture)));
        }
        AssetDatabase.Refresh();
        foreach (string texture in texturePaths)
            allTextures.Add((Texture2D)AssetDatabase.LoadAssetAtPath(Path.Combine(textureImportDir, Path.GetFileName(texture)), typeof(Texture2D)));

        // actually do the texture application stuff
        string json = File.ReadAllText(Path.Combine(umapPath, "_processed.json"));
        JArray mainArray = JArray.Parse(json);
        foreach (var obj in mainArray)
        {
            MeshRenderer rend = rootObject.transform.Find(obj["Name"].ToString())?.GetComponent<MeshRenderer>();
            if (rend == null)
            {
                Debug.LogWarning("Failed to find object " + obj["Name"] + ", textures will not be applied");
                continue;
            }
            
            foreach (var mat in obj["Materials"])
            {
                Material realMat = rend.sharedMaterials.SingleOrDefault(m => m.name == mat["Name"].ToString());
                if (realMat == null)
                {
                    Debug.LogWarning("Failed to find material " + mat["Name"] + ", textures will not be applied");
                    continue;
                }

                foreach (var mapping in textureShaderMapping)
                {
                    if (mapping.Value != string.Empty && mat["Textures"][mapping.Key] != null)
                    {
                        realMat.SetTexture(mapping.Value, allTextures.SingleOrDefault(t => t.name == Path.GetFileNameWithoutExtension(mat["Textures"][mapping.Key].ToString())));
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
    }
}