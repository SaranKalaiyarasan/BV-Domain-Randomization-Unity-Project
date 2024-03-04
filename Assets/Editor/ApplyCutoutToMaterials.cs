using UnityEngine;
using UnityEditor;
using System.IO;

public class ApplyCutoutToMaterials : EditorWindow
{
    private string folderPath = "Assets/Resources/Images/Materials"; // Default path to search for materials.

    [MenuItem("Tools/Apply Cutout to Materials")]
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
            string assetPath = "Assets" + materialFile.Replace(Directory.GetCurrentDirectory(), "").Replace('\\', '/');
            Material material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material != null)
            {
                material.SetFloat("_Mode", 1); // Set the rendering mode to Cutout.
                material.SetFloat("_Cutoff", 0.8f); // Set the alpha cutoff value.
                material.EnableKeyword("_ALPHATEST_ON");
                material.renderQueue = 2450;
                EditorUtility.SetDirty(material); // Mark the material as dirty to ensure the changes are saved.
            }
        }
        AssetDatabase.SaveAssets(); // Save all changes to assets.
        Debug.Log("Applied cutout mode and alpha cutoff to all materials in folder: " + folderPath);
    }
}
