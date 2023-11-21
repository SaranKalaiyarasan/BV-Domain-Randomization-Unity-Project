using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject sun;
    private Camera cam;
    private System.Random rnd;

    public static int picturesTaken = 0;
    public static int counter = 0;
    readonly public static int totalpics = 10;//50;
    readonly private Vector2 AspectRatio = new Vector2(1920, 1080);

    
    
    readonly private string shapeName = "Pink_Triangle";
    public static int shapeIndex = 37;

    public GameObject TLTarget;
    public GameObject BRTarget;
    public GameObject BLTarget;
    public GameObject TRTarget;
    public GameObject target;
    public GameObject road;
    //Add more road objects so that they can be switched to
    public static Boolean swapPage = false;

    private int prevPicTaken = -1;
    private int prevShapeIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        rnd = new System.Random();
        cam = GetComponent<Camera>();
    }


    // Update is called once per frame
    void Update()
    {
        Debug.Log("TOTAL: " + totalpics);
        //Change code so that material is changed every set  number of pictures, instead of just turning off and on. IN PROGRESS.
        if (picturesTaken >= (int)((3 * totalpics) / 4))
        {
            road.SetActive(false);
        }
        else
        {
            road.SetActive(true);
        }

        if (picturesTaken <= totalpics)
        {
            if (!swapPage)
            {
                randomizeSun();

                randomizeCamera();
                randomizeTarget();

                List<Vector2> corners = new List<Vector2>();

                Vector2 temp = cam.WorldToScreenPoint(TLTarget.transform.position);
                corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
                temp = cam.WorldToScreenPoint(BLTarget.transform.position);
                corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
                temp = cam.WorldToScreenPoint(TRTarget.transform.position);
                corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
                temp = cam.WorldToScreenPoint(BRTarget.transform.position);
                corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));

                Vector2 widthHeight = findWidthHeight(corners);

                temp = cam.WorldToScreenPoint(target.transform.position);
                Vector2 centerPos = new Vector2(temp.x, AspectRatio.y - temp.y);

                if (verifyBounds(centerPos))
                {
                    string screenShotPath;
                    string filePath;

                    //Every four pictures sent to train set
                    if (picturesTaken % 5 != 0)
                    {
                        screenShotPath = "C:\\Data\\Buckeye Vertical\\Image Classifier\\Prelim Detection Dataset\\train\\images\\" + shapeIndex + "_" + picturesTaken.ToString() + ".png";
                        filePath = "C:\\Data\\Buckeye Vertical\\Image Classifier\\Prelim Detection Dataset\\train\\labels\\" + shapeIndex + "_" + picturesTaken.ToString() + ".txt";
                    }
                    //Every fifth picture sent to validation set
                    else
                    {
                        screenShotPath = "C:\\Data\\Buckeye Vertical\\Image Classifier\\Prelim Detection Dataset\\valid\\images\\" + shapeIndex + "_" + picturesTaken.ToString() + ".png";
                        filePath = "C:\\Data\\Buckeye Vertical\\Image Classifier\\Prelim Detection Dataset\\valid\\labels\\" + shapeIndex + "_" + picturesTaken.ToString() + ".txt";
                    }

                    Debug.Log(" ");
                    Debug.Log(picturesTaken.ToString());
                    Debug.Log("Width: " + widthHeight.x);
                    Debug.Log("Height: " + widthHeight.y);
                    Debug.Log("Center Position: " + centerPos.ToString());

                    Debug.Log("The current prevPicTaken is: " + prevPicTaken);

                    //change to train
                    string textToWrite = shapeIndex + " " + normalize(centerPos.x, AspectRatio.x).ToString() + " " + normalize(centerPos.y, AspectRatio.y).ToString() + " " + normalize(widthHeight.x, AspectRatio.x) + " " + normalize(widthHeight.y, AspectRatio.y);

                    Debug.Log("WE ARE ABOUT TO TAKE A SCREENSHOT");
                    //Rough Solution
                    if (File.Exists("C:\\Data\\Buckeye Vertical\\Image Classifier\\Prelim Detection Dataset\\train\\images\\" + prevShapeIndex + "_" + prevPicTaken.ToString() + ".png") || prevPicTaken == -1|| File.Exists("C:\\Data\\Buckeye Vertical\\Prelim Detection Dataset\\valid\\images\\" + prevShapeIndex + "_" + prevPicTaken.ToString() + ".png"))
                    {
                        // Create a new StreamWriter and write the text to the file
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            writer.WriteLine(textToWrite);
                        }

                        //CHANGE TO TRAIN
                        Debug.Log("WE HAVE TAKEN A SCREENSHOT");
                        ScreenCapture.CaptureScreenshot(screenShotPath);
                        prevPicTaken = picturesTaken;
                        prevShapeIndex = shapeIndex;

                        picturesTaken++;
                        Debug.Log("Hello");
                    }
                }
            }
            else
            {
                swapPage = false;
            }
        }
        else
        {
            if (shapeIndex != 0)
            {
                shapeIndex--;
                swapPage = true;
                picturesTaken = 0;
            }
        }
    }

    public float normalize(float value, float total)
    {
        return value / total;
    }

    //Returns Vector2 (width, height)
    private Vector2 findWidthHeight(IList<Vector2> corners)
    {
        float highestX = float.MinValue;
        float highestY = float.MinValue;
        float lowestX = float.MaxValue;
        float lowestY = float.MaxValue;

        foreach (Vector2 vector in corners)
        {
            if (vector.x > highestX)
            {
                highestX = vector.x;
            }

            if (vector.y > highestY)
            {
                highestY = vector.y;
            }

            if (vector.x < lowestX)
            {
                lowestX = vector.x;
            }

            if (vector.y < lowestY)
            {
                lowestY = vector.y;
            }
        }

        return new Vector2(highestX - lowestX, highestY - lowestY);
    }

    private bool verifyBounds(Vector2 CenterPos)
    {
        return !(CenterPos.x < 0 || CenterPos.x > AspectRatio.x || CenterPos.y < 0 || CenterPos.y > AspectRatio.y);
    }

    private void randomizeSun()
    {
        float angle = rnd.Next(0, 200);
        sun.transform.localRotation = Quaternion.Euler(angle, 0, 0);
    }

    private void randomizeTarget()
    {
        float x = rnd.Next(-72, 106);
        float z = rnd.Next(-73, 60);
        target.transform.localPosition = new Vector3(x, 0.6f, z);

        float rotation_y = rnd.Next(0, 359);
        target.transform.localRotation = Quaternion.Euler(0, rotation_y, 0);
    }

    private void randomizeCamera()
    {
        float x = rnd.Next(-60, 60);
        float y = rnd.Next(100, 150);
        float z = rnd.Next(-25, 25);
        cam.transform.localPosition = new Vector3(x, y, z);

        float rotation_y = rnd.Next(0, 359);
        float rotation_x = rnd.Next(85, 95);
        cam.transform.localRotation = Quaternion.Euler(rotation_x, rotation_y, 0);
    }
}