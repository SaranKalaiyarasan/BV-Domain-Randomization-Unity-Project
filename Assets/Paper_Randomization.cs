using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper_Randomization : MonoBehaviour
{
    float transparency = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        
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
            Material yourMaterial = Resources.Load("Images/Materials/" + CameraScript.shapeIndex, typeof(Material)) as Material;
            this.gameObject.GetComponent<Renderer>().material = yourMaterial;
        }
    }

    void ChangeAlpha(Material mat, float alphaVal)
    {
        Debug.Log(alphaVal);
        Color oldColor = mat.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, alphaVal);
        mat.SetColor("_Color", newColor);

    }
}
