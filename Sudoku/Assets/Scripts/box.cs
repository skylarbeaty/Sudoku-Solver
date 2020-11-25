using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class box : MonoBehaviour
{
    //this class only exists to pass data in Unity on multiple of the same box prefab
    //represents a 3x3 with indices in phone order
    // 0 1 2
    // 3 4 5
    // 6 7 8
    public Text[] cells;
}
