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
    //shapeName= 7
    private int matColor = 7;
    private int matShape = 7;
    private int matDigit = 89;
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
            Material yourMaterial = Resources.Load("Images/Materials/" + matName, typeof(Material)) as Material;
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

    string getMaterialName()
    {
        matDigit--;
        if(matDigit == 64)
        {
            matDigit = 57;
        }
        if(matDigit == 47)
        {
            matColor--;
            matDigit = 90;
        }
        if(matColor < 0)
        {
            matShape--;
            matColor = 7;
        }
        if (matShape < 0)
        {
            Debug.Log("MATERIAL SHOULD NOT EXIST");
        }
        return (matShape.ToString()+matColor.ToString()+matDigit.ToString());
    }
}
