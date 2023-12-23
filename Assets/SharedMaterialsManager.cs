using System.Collections.Generic;
using UnityEngine;

public class SharedMaterialsManager : MonoBehaviour
{
    public static SharedMaterialsManager Instance { get; private set; }

    private List<string> availableMaterials;
    private System.Random rnd = new System.Random();

    private int count;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeMaterials();
    }

    private void InitializeMaterials()
    {
        // Initialize your materials here. Example:
        availableMaterials = GenerateMaterialNames();
    }

    public string GetRandomMaterial()
    {
        if (availableMaterials.Count > 0)
        {
            count++;
            int index = rnd.Next(availableMaterials.Count);
            string selectedMaterial = availableMaterials[index];
            //Debug.Log(selectedMaterial);
            availableMaterials.RemoveAt(index);  // Remove the used material
            Debug.Log(count);
            return selectedMaterial;
        }
        else
        {
            Debug.LogWarning("No more available materials.");
            return null; // Or handle this situation as you see fit
        }
    }
    //Generates all material names
    public static List<string> GenerateMaterialNames()
    {
        List<int> shapes = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
        List<int> colors = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
        List<int> alphanumerics = new List<int>();
        for (int i = 48; i <= 57; i++) alphanumerics.Add(i); // ASCII codes for 0-9
        for (int i = 65; i <= 90; i++) alphanumerics.Add(i); // ASCII codes for A-Z

        List<string> firstShapeMaterials = new List<string>();

        foreach (int shape in shapes)
        {
            foreach (int color in colors)
            {
                foreach (int code in alphanumerics)
                {
                    string materialName = shape.ToString() + color.ToString() + code.ToString();
                    firstShapeMaterials.Add(materialName);
                }
            }
        }

        return firstShapeMaterials;
    }
}
