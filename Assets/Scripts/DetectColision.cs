using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectColision : MonoBehaviour
{
    // Start is called before the first frame update

    private string objName;
    private bool isInSlot;


    void Start()
    {
        objName = " ";
        isInSlot = false;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.name == "cube0" || col.name == "cube1" || col.name == "cube2" || col.name == "cube3")
        {
            objName = col.name;
            isInSlot = true;
        }
    }

    //when the cube is removed from one of the slots
    private void OnTriggerExit(Collider col)
    {
        if (col.name == "cube0" || col.name == "cube1" || col.name == "cube2" || col.name == "cube3")
        {
            objName = "null";
            isInSlot = false;
            
        }

    }


    public bool isPlacedOnSlot()
    {
        Debug.Log("isPlacedOnSlot collision was called");
        return isInSlot;

    }

    public string getObjectName()
    {
        return objName;
    }
}
