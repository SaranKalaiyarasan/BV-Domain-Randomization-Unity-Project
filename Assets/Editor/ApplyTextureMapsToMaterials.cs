using UnityEngine;
using UnityEditor;
using System.IO;

public class ApplyTextureMapsToMaterials : EditorWindow
{
    private string materialsFolderPath = "Assets/Resources/Images/Materials"; // Path to the materials
    private string texturesFolderPath = "Assets/Resources/Materials"; // Path to the textures

    [MenuItem("Tools/Apply Texture Maps to Materials")]
    private static void ShowWindow()
    {
        var window = GetWindow<ApplyTextureMapsToMaterials>();
        window.titleContent = new GUIContent("Apply Maps to Mats");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Materials Folder Path", EditorStyles.boldLabel);
        materialsFolderPath = EditorGUILayout.TextField("Materials Folder", materialsFolderPath);

        GUILayout.Label("Textures Folder Path", EditorStyles.boldLabel);
        texturesFolderPath = EditorGUILayout.TextField("Textures Folder", texturesFolderPath);

        if (GUILayout.Button("Apply Maps"))
        {
            ApplyMaps(materialsFolderPath, texturesFolderPath);
        }
    }

    private static void ApplyMaps(string materialsPath, string texturesPath)
    {
        string[] materialFiles = Directory.GetFiles(materialsPath, "*.mat", SearchOption.AllDirectories);

        foreach (string materialFile in materialFiles)
        {
            Material material = AssetDatabase.LoadAssetAtPath<Material>(materialFile);

            if (material != null)
            {
                ApplyTexture(material, "_OcclusionMap", "paper_0025_ao_4k.png", texturesPath);
                ApplyTexture(material, "_ParallaxMap", "paper_0025_height_4k.png", texturesPath);
                ApplyTexture(material, "_BumpMap", "paper_0025_normal_directx_4k.png", texturesPath);
                ApplyTexture(material, "_MetallicGlossMap", "paper_0025_roughness_4k.png", texturesPath);

                EditorUtility.SetDirty(material);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Applied texture maps to all materials in folder: " + materialsPath);
    }

    private static void ApplyTexture(Material material, string propertyName, string textureName, string texturesPath)
    {
        string texturePath = Path.Combine(texturesPath, textureName);
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

        if (texture != null)
        {
            material.SetTexture(propertyName, texture);
            Debug.Log($"Applied {textureName} to {material.name}");
        }
        else
        {
            Debug.LogWarning($"Could not load texture at path: {texturePath}");
        }
    }
}
