using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

public class CameraScript : MonoBehaviour
{
    public GameObject sun;
    private Camera cam;
    private System.Random rnd;

    public static int picturesTaken = 0;
    public static int counter = 0;
    readonly public static int totalpics = 3;//50;
    readonly private Vector2 AspectRatio = new Vector2(1920, 1080);

    const string workingDirectory = "D:\\Data\\Buckeye Vertical\\Image Classifier";
    //const string workingDirectory = "U:\\Prelim Detection Dataset";

    public PayloadTargetContents[] payloadTargets;
    public GameObject road;
    public static Boolean swapPage = false;
    public static Boolean swapRoad = false;

    private int fileCount = 0;
    private int prevFileCount = 0;

    private int totalObjects = 9216;
    private int fileCountConstant = 1152;

    //CHANGE THIS!!
    private int numPayloads = 8;

    private int prevPicTaken = -1;

    private void randomizeSun()
    {
        // Random angle between 0 and 200 degrees.
        float angle = rnd.Next(0, 180);
        sun.transform.localRotation = Quaternion.Euler(angle, 0, 0);

        // Calculate intensity based on the angle.
        // Assuming maximum intensity is 2 and minimum is 0.5 for demonstration.
        float maxIntensity = 1.3f;
        float minIntensity = 1.0f;

        // Normalize angle for intensity calculation (making 90 degrees = 1, 0 and 180 degrees = 0)
        float normalizedAngle = Mathf.Abs(angle - 90) / 90.0f; // This will give 0 at 90 degrees and 1 at 0 or 180 degrees.

        // Calculate intensity (inverse relationship with the normalized angle).
        float intensity = maxIntensity - (normalizedAngle * (maxIntensity - minIntensity));

        // Set the sun's intensity.
        sun.GetComponent<Light>().intensity = intensity;
    }

    private void randomizeCamera()
    {
        float x = rnd.Next(-60, 60);
        float y = rnd.Next(150, 200);
        float z = rnd.Next(-25, 25);
        cam.transform.localPosition = new Vector3(x, y, z);

        float rotation_y = rnd.Next(0, 359);
        float rotation_x = rnd.Next(85, 95);
        cam.transform.localRotation = Quaternion.Euler(rotation_x, rotation_y, 0);
    }



    //Returns a boolean, which verifies if all objects were successfully placed on screen, and not overlapping
    private bool RandomizeAndVerifyTargets()
    {
        bool notFailed = true;
        Camera cam = Camera.main; // Assuming you're using the main camera
        List<Vector3> placedPositions = new List<Vector3>(); // To store positions of placed objects

        foreach (PayloadTargetContents targetContent in payloadTargets)
        {
            bool isPositionValid = false;
            int attempts = 0;
            int maxAttempts = 10000; // Set a maximum number of attempts to prevent infinite loops

            while (!isPositionValid && attempts < maxAttempts)
            {
                // Randomize position
                Vector3 newPosition = GetRandomPosition();
                targetContent.target.transform.localPosition = newPosition;

                // Check if in view
                Vector3 screenPos = cam.WorldToViewportPoint(targetContent.target.transform.position);
                bool inView = screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1 && screenPos.z > 0;

                // Check for overlaps
                bool isOverlapping = placedPositions.Any(pos => Vector3.Distance(newPosition, pos) < 50f);

                // Update position validity
                isPositionValid = inView && !isOverlapping;

                if (isPositionValid)
                {
                    // Randomize rotation
                    float rotation_y = rnd.Next(0, 359);
                    targetContent.target.transform.localRotation = Quaternion.Euler(0, rotation_y, 2);

                    placedPositions.Add(newPosition); // Add this position to the list
                }

                attempts++;
            }

            if (attempts >= maxAttempts)
            {
                notFailed = false;
                //Debug.LogWarning("Max attempts reached for object placement. Last position may not be ideal.");
            }
        }
        return notFailed;
    }

    private Vector3 GetRandomPosition()
    {
        float x = rnd.Next(-150, 200); //rnd.Next(-72, 106);
        float z = rnd.Next(-150, 200); //rnd.Next(-73, 60);
        return new Vector3(x, 3.0f, z);
    }


    //Returns a string comprised of all payload object's class, normalized x and y value, and normalized width and height
    private string GenerateNormalizedDataString()
    {
        string dataString = "";

        foreach (PayloadTargetContents payload in payloadTargets)
        {
            int materialClass;
            Renderer targetRenderer = payload.target.GetComponent<Renderer>();
            if (targetRenderer != null && targetRenderer.material != null)
            {
                string materialName = targetRenderer.material.name;
                materialClass = GetMaterialClass(materialName);
                dataString += materialClass + " ";
            }
            Vector2 centerPos = GetCenterPosition(payload.target);
            Vector2 widthHeight = GetWidthHeight(payload);

            dataString += normalize(centerPos.x, AspectRatio.x).ToString() + " " +
                        normalize(centerPos.y, AspectRatio.y).ToString() + " " +
                        normalize(widthHeight.x, AspectRatio.x).ToString() + " " +
                        normalize(widthHeight.y, AspectRatio.y).ToString() + "\n";
        }

        return dataString;
    }

    //Gets material class give a 4 digit string
    int GetMaterialClass(string materialName)
    {
        if (!string.IsNullOrEmpty(materialName) && materialName.Length >= 2)
        {
            int shape = int.Parse(materialName.Substring(0, 1));
            int color = int.Parse(materialName.Substring(1, 1));
            return shape * 8 + color;  // Compute class index
        }
        return -1;  // Return -1 or an appropriate value for invalid cases
    }

    private Vector2 GetCenterPosition(GameObject target)
    {
        Vector2 temp = cam.WorldToScreenPoint(target.transform.position);
        Vector2 centerPos = new Vector2(temp.x, AspectRatio.y - temp.y);
        return centerPos;
    }
    //Returns width and height of a payload object as a Vector2 Object
    private Vector2 GetWidthHeight(PayloadTargetContents payload)
    {
        List<Vector2> corners = new List<Vector2>();
        Vector2 temp = cam.WorldToScreenPoint(payload.TLTarget.transform.position);
        corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
        temp = cam.WorldToScreenPoint(payload.BLTarget.transform.position);
        corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
        temp = cam.WorldToScreenPoint(payload.TRTarget.transform.position);
        corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
        temp = cam.WorldToScreenPoint(payload.BRTarget.transform.position);
        corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));

        Vector2 widthHeight = findWidthHeight(corners);
        return widthHeight;
    }   
    //Normalize method
    public float normalize(float value, float total)
    {
        return value / total;
    }

    //Returns Vector2 (width, height) given 4 coordinates
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

    // Start is called before the first frame update
    void Start()
    {
        rnd = new System.Random();
        cam = GetComponent<Camera>();
    }

    //Update is called once per frame
    // void Update()
    // {
    //     swapPage = false;
    //     if(counts % 5 == 0){
    //     }
    //     if(counts % 30 == 0){
    //         swapPage = true;
    //         randomizeCamera();
    //         randomizeSun();
    //         RandomizeAndVerifyTargets();
    //     }

    //     counts++;  
    // }
    
    void Update()
    {

        if (picturesTaken <= totalpics){
            if(!swapPage)
            {
                randomizeSun();
                randomizeCamera();
                if(RandomizeAndVerifyTargets()){
                    string textToWrite = GenerateNormalizedDataString();
                    string screenShotPath;
                    string filePath;
                    //Every four pictures sent to train set
                    if (picturesTaken % 5 != 0)
                    {
                        screenShotPath = workingDirectory + "\\train\\images\\" + (fileCount+fileCountConstant) + "_" + picturesTaken.ToString() + ".png";
                        filePath = workingDirectory + "\\train\\labels\\" + (fileCount+fileCountConstant) + "_" + picturesTaken.ToString() + ".txt";
                    }
                    //Every fifth picture sent to validation set
                    else
                    {
                        screenShotPath = workingDirectory + "\\valid\\images\\" + (fileCount+fileCountConstant) + "_" + picturesTaken.ToString() + ".png";
                        filePath = workingDirectory + "\\valid\\labels\\" + (fileCount+fileCountConstant) + "_" + picturesTaken.ToString() + ".txt";
                    }

                    if(File.Exists(workingDirectory + "\\train\\images\\" + (prevFileCount + fileCountConstant) + "_" + prevPicTaken.ToString() + ".png") ||
                    prevPicTaken == -1|| File.Exists(workingDirectory + "\\valid\\images\\" + (prevFileCount + fileCountConstant) + "_" + prevPicTaken.ToString() + ".png"))
                    {
                        // Create a new StreamWriter and write the text to the file
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            writer.WriteLine(textToWrite);
                        }

                        //CHANGE TO TRAIN
                        ScreenCapture.CaptureScreenshot(screenShotPath);
                        prevPicTaken = picturesTaken;
                        prevFileCount = fileCount;
                        
                        picturesTaken++;
                    }
                }
            }
            else
            {
                swapRoad = false;
                Debug.Log(swapRoad);
                swapPage = false;
            }
        }
        else
        {
            //If the current fileCount is less than the 0-indexed final number of iterations
            if(fileCount < (totalObjects/numPayloads)-1)
            {
                fileCount++;
                swapRoad = true;
                Debug.Log(swapRoad);
                swapPage = true;
                picturesTaken = 0;
            }
            else
            {
                Debug.Log("Execution Finished");
                return;
            }
        }
    }

    
    // private void PlaceObjectsInGrid()
    // {
    //     Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, cam.nearClipPlane);
    //     Vector3 worldCenter = cam.ScreenToWorldPoint(screenCenter);

    //     float gridVerticalSize = Screen.height / gridRows;
    //     float gridHorizontalSize = Screen.width / gridColumns;
    //     Debug.Log("gridVerticalSize: " + gridVerticalSize);

    //     // Increase the depth
    //     float depth = 100; // Adjust this value to place objects further away

    //     for (int i = 0; i < payloadTargets.Length; i++)
    //     {
    //         int row = i / gridColumns;
    //         int column = i % gridColumns;
    //         Debug.Log(row);
    //         Debug.Log(column);

    //         Vector3 screenCellPosition = new Vector3(
    //             (column + 0.5f) * gridHorizontalSize,
    //             (row + 0.5f) * gridVerticalSize,
    //             cam.nearClipPlane
    //         );

    //         Vector3 worldCellPosition = cam.ScreenToWorldPoint(screenCellPosition);

    //         // Adjust the depth
    //         worldCellPosition += cam.transform.forward * depth;

    //         PlaceObjectInCell(payloadTargets[i].target, worldCellPosition);
    //     }
    // }

    // private void PlaceObjectInCell(GameObject obj, Vector3 cellPosition)
    // {
    //     // Define a margin as a percentage of cell size
    //     float marginPercentage = 0.1f; // For example, 20% margin

    //     // Calculate actual margin values based on cell size and margin percentage
    //     float marginX = cellWidth * marginPercentage;
    //     float marginZ = cellHeight * marginPercentage;

    //     // Adjust randomization range to exclude margins
    //     float randomizedX = UnityEngine.Random.Range(-cellWidth / 2 + marginX, cellWidth / 2 - marginX);
    //     float randomizedZ = UnityEngine.Random.Range(-cellHeight / 2 + marginZ, cellHeight / 2 - marginZ);

    //     obj.transform.position = cellPosition + new Vector3(randomizedX, 0.6f, randomizedZ);
    //     Debug.Log(cellPosition);
    //     // Optionally, randomize rotation or other properties
    // }

  // private void randomizeTargetsPosition(){
    //     foreach (var targetObj in payloadTargets)
    //     {
    //         bool isInView;
    //         do
    //         {
    //             randomizeTarget(targetObj);
    //             isInView = verifyBounds(targetObj.target);
    //         }
    //         while (!isInView);
    //     }
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     //Change code so that material is changed every set  number of pictures, instead of just turning off and on. IN PROGRESS.
    //     if (picturesTaken >= (int)((3 * totalpics) / 4))
    //     {
    //         road.SetActive(false);
    //     }
    //     else
    //     {
    //         road.SetActive(true);
    //     }

    //     if (picturesTaken <= totalpics)
    //     {
    //         if (!swapPage)
    //         {
                // randomizeSun();

                // randomizeCamera();
                // randomizeTarget();

                // List<Vector2> corners = new List<Vector2>();

                // Vector2 temp = cam.WorldToScreenPoint(TLTarget.transform.position);
                // corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
                // temp = cam.WorldToScreenPoint(BLTarget.transform.position);
                // corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
                // temp = cam.WorldToScreenPoint(TRTarget.transform.position);
                // corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));
                // temp = cam.WorldToScreenPoint(BRTarget.transform.position);
                // corners.Add(new Vector2(temp.x, AspectRatio.y - temp.y));

                // Vector2 widthHeight = findWidthHeight(corners);

                // temp = cam.WorldToScreenPoint(target.transform.position);
                // Vector2 centerPos = new Vector2(temp.x, AspectRatio.y - temp.y);

                // if (verifyBounds(centerPos))
    //             {
    //                 string screenShotPath;
    //                 string filePath;

                    // //Every four pictures sent to train set
                    // if (picturesTaken % 5 != 0)
                    // {
                    //     screenShotPath = workingDirectory + "\\train\\images\\" + shapeIndex + "_" + picturesTaken.ToString() + ".png";
                    //     filePath = workingDirectory + "\\train\\labels\\" + shapeIndex + "_" + picturesTaken.ToString() + ".txt";
                    // }
                    // //Every fifth picture sent to validation set
                    // else
                    // {
                    //     screenShotPath = workingDirectory + "\\valid\\images\\" + shapeIndex + "_" + picturesTaken.ToString() + ".png";
                    //     filePath = workingDirectory + "\\valid\\labels\\" + shapeIndex + "_" + picturesTaken.ToString() + ".txt";
                    // }

    //                 //change to train
                    // string textToWrite = (int)(shapeIndex/36) + " " + normalize(centerPos.x, AspectRatio.x).ToString() + " " + normalize(centerPos.y, AspectRatio.y).ToString() + " " + normalize(widthHeight.x, AspectRatio.x) + " " + normalize(widthHeight.y, AspectRatio.y);

    //                 //Rough Solution
    //                 if (File.Exists(workingDirectory + "\\train\\images\\" + prevShapeIndex + "_" + prevPicTaken.ToString() + ".png") ||
    //                  prevPicTaken == -1|| File.Exists(workingDirectory + "\\valid\\images\\" + prevShapeIndex + "_" + prevPicTaken.ToString() + ".png"))
    //                 {
                        // // Create a new StreamWriter and write the text to the file
                        // using (StreamWriter writer = new StreamWriter(filePath))
                        // {
                        //     writer.WriteLine(textToWrite);
                        // }

                        // //CHANGE TO TRAIN
                        // ScreenCapture.CaptureScreenshot(screenShotPath);
                        // prevPicTaken = picturesTaken;
    //                     prevShapeIndex = shapeIndex;

    //                     picturesTaken++;
    //                 }
    //             }
    //         }
    //         else
    //         {
    //             swapPage = false;
    //         }
    //     }
    //     else
    //     {
    //         if (shapeIndex > endIndex)
    //         {
    //             shapeIndex--;
    //             swapPage = true;
    //             picturesTaken = 0;
    //         }else{
    //             return;
    //         }
    //     }
    // }





    // private bool verifyBounds(Vector2 CenterPos)
    // {
    //     return !(CenterPos.x < 0 || CenterPos.x > AspectRatio.x || CenterPos.y < 0 || CenterPos.y > AspectRatio.y);
    // }

    

    // private void randomizeTarget()
    // {
    //     float x = rnd.Next(-72, 106);
    //     float z = rnd.Next(-73, 60);
    //     target.transform.localPosition = new Vector3(x, 0.6f, z);

    //     float rotation_y = rnd.Next(0, 359);
    //     target.transform.localRotation = Quaternion.Euler(0, rotation_y, 0);
    // }

    
}