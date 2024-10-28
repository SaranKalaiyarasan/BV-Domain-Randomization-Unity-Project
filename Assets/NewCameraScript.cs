using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NewCameraScript : MonoBehaviour
{
    private Camera cam;
    private int screenshotCounter = 0;
    private int frameCounter = 0;
    public float moveSpeedForward = 20f;
    public float moveDistance = 100;
    private bool movingForward = true;

    const string workingDirectory = "D:\\Data\\Buckeye Vertical\\Filtering";
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        MoveCameraForward();
        ScreenshotCapture();
    }
   private void MoveCameraForward()
    {
        float step = moveSpeedForward * Time.deltaTime;
        if (movingForward)
        {
            transform.position += new Vector3(0, 0, step);
            if (transform.position.z >= moveDistance)
            {
                movingForward = false;
            }
        }
        else
        {
            transform.position += new Vector3(0, 0, -step);
            if (transform.position.z <= -moveDistance)
            {
                movingForward = true;
            }
        }
    }
    private void ScreenshotCapture()
    {
        frameCounter++;
        if (frameCounter % 5 == 0)
        {
            string screenshotPath = workingDirectory + $"\\{screenshotCounter}.png";
            ScreenCapture.CaptureScreenshot(screenshotPath);
            screenshotCounter++;
            Debug.Log($"Screenshot saved: {screenshotPath}");
        }
    }
}