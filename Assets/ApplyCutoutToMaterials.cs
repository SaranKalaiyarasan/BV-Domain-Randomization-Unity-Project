using UnityEngine;
using UnityEditor;
using System.IO;

public class ApplyCutoutToMaterials : EditorWindow
{
    private string folderPath = "Assets/Resources/Images/Materials"; // Default path to search for materials.

    [MenuItem("Tools/Apply Cutout to Materials2")]
    private static void ShowWindow()
    {
        var window = GetWindow<ApplyCutoutToMaterials>();
        window.titleContent = new GUIContent("Apply Cutout to Materials");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Set Folder Path", EditorStyles.boldLabel);
        folderPath = EditorGUILayout.TextField("Materials Folder Path", folderPath);

        if (GUILayout.Button("Apply Cutout and Alpha Cutoff"))
        {
            ApplyCutoutAndAlpha(folderPath);
        }
    }

    private static void ApplyCutoutAndAlpha(string folderPath)
{
    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), folderPath);
    if (!Directory.Exists(fullPath))
    {
        Debug.LogError("Directory does not exist: " + fullPath);
        return;
    }

    string[] materialFiles = Directory.GetFiles(fullPath, "*.mat", SearchOption.AllDirectories);
    foreach (string materialFile in materialFiles)
    {
        // Correctly format the assetPath by removing the absolute path portion
        string assetPath = materialFile.Replace(Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar, "").Replace('\\', '/');
        Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
        if (material != null)
        {
            Debug.Log($"Modifying material: {material.name}");

            if (material.HasProperty("_Mode"))
            {
                material.SetFloat("_Mode", 1); // Set the rendering mode to Cutout.
                Debug.Log($"Set _Mode to Cutout on {material.name}");
            }
            else
            {
                Debug.LogWarning($"{material.name} does not have a _Mode property");
            }

            if (material.HasProperty("_Cutoff"))
            {
                material.SetFloat("_Cutoff", 0.85f); // Set the alpha cutoff value.
                Debug.Log($"Set _Cutoff to 0.8 on {material.name}");
            }
            else
            {
                Debug.LogWarning($"{material.name} does not have a _Cutoff property");
            }

            material.EnableKeyword("_ALPHATEST_ON");
            material.renderQueue = 2450;

            EditorUtility.SetDirty(material); // Mark the material as dirty to ensure the changes are saved.
        }
        else
        {
            Debug.LogWarning($"Could not load material at path: {assetPath}");
        }
    }
    AssetDatabase.SaveAssets(); // Save all changes to assets.
    Debug.Log("Applied cutout mode and alpha cutoff to all materials in folder: " + folderPath);
}

}
