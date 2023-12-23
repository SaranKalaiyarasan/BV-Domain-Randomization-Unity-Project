using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class PayloadTargetContents
{
    public GameObject target; // The 'Paper' object
    public GameObject TLTarget; // TopLeft object
    public GameObject BRTarget; // BottomRight object
    public GameObject BLTarget; // BottomLeft object
    public GameObject TRTarget; // TopRight object
}
