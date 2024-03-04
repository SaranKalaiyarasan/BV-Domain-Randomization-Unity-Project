using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadScript : MonoBehaviour
{

    int count = 0;
    List<string> materialPaths;

    // Start is called before the first frame update
    void Start()
    {
        Material yourMaterial = Resources.Load("Materials/Fabric_pattern_07", typeof(Material)) as Material;
        gameObject.GetComponent<Renderer>().material = yourMaterial;
        materialPaths = new List<string>
        {
            "Materials/Fabric_pattern_07",
            "Materials/Pavement",
            "Materials/Patterned_concrete_pavers"
        };
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("RoadScript called");
        if(CameraScript.swapRoad)
        {
            Debug.Log("Swapping Road");
            ApplyMaterial(count);
            count++;
            if(count>2){
                count = 0;
            }
        }
    }

    private void ApplyMaterial(int index){
        Material yourMaterial = Resources.Load(materialPaths[index], typeof(Material)) as Material;
        this.gameObject.GetComponent<Renderer>().material = yourMaterial;
    }
}
