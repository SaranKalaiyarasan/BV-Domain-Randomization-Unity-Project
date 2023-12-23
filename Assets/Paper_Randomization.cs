using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper_Randomization : MonoBehaviour
{
    float transparency = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Request a random material name at the start
        string currentMaterialName = getMaterialName();
        if (!string.IsNullOrEmpty(currentMaterialName))
        {
            Material yourMaterial = Resources.Load("Images/Materials/" + currentMaterialName, typeof(Material)) as Material;
            gameObject.GetComponent<Renderer>().material = yourMaterial;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CameraScript.picturesTaken % 10 == 0)
        {
            ChangeAlpha(gameObject.GetComponent<Renderer>().material, ((CameraScript.totalpics / 2.0f) + (float)(CameraScript.totalpics - CameraScript.picturesTaken)) / (float)CameraScript.totalpics);
        }
        if (CameraScript.swapPage)
        {
            string matName = getMaterialName();
            //Debug.Log("SWITCHING MATERIALS");
            Material yourMaterial = Resources.Load("Images/Materials/" + matName, typeof(Material)) as Material;
            this.gameObject.GetComponent<Renderer>().material = yourMaterial;
        }
    }

    void ChangeAlpha(Material mat, float alphaVal)
    {
        //Debug.Log(alphaVal);
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
        mat.SetColor("_Color", newColor);

    }

    public int GetMaterialClass()
    {
        string materialName = getMaterialName();
        if (!string.IsNullOrEmpty(materialName))
        {
            int shape = int.Parse(materialName.Substring(0, 1));
            int color = int.Parse(materialName.Substring(1, 1));
            return shape * 8 + color;  // Compute class index
        }
        return -1;  // Return -1 or an appropriate value for invalid cases
    }

    string getMaterialName()
    {
        return SharedMaterialsManager.Instance.GetRandomMaterial();
    }
}
