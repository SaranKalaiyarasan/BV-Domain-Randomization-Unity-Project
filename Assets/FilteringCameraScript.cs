using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class FilteringCameraScript : MonoBehaviour
{
    private Camera cam;
    public GameObject road;

    private Vector3 roadSize;
    // Start is called before the first frame update
    void Start()
    {
        roadSize = road.GetComponent<Renderer>().bounds.size;
        cam = GetComponent<Camera>();
        cam.transform.localPosition = new Vector3(0,250);
    }

    private void moveCamera()
    {
        // cam.transform.position.X += 1;
    }

    // Update is called once per frame
    void Update()
    {
        moveCamera();
    }
}
