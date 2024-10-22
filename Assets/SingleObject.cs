using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleObject : MonoBehaviour
{
    public GameObject target;
    private int counter = 0;
    private int counter2 = 0;
    // Start is called before the first frame update
    void Start()
    {
        target.active = false;
    }

    // Update is called once per frame
    void Update()
    {
        counter += 1;
        if(counter%100 == 0){
            target.active = true;
            counter2 = counter + 5;
        }
        if(counter == counter2){
            target.active = false;
        }
    }
}
