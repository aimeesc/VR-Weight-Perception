using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
public class FirstExperiment : MonoBehaviour
{
    private int NUMCUBES = 4, MAXNUMTRIALS = 5, trialIndex;
    public GameObject[] cubes;
    public GameObject[] slots = new GameObject[4]; 
    public GameObject cubePrefab;
    public Rigidbody[] rigidbodies;
    private float offsetPosition;
    Vector3 cubePosition;

    private float[] answers;

    /* equivalence from slots and weight
     * 0 = heavier
     * 1 = heavy
     * 2 = light
     * 3 = lighter
     */


    void Start()
    {

        string cubeName;
        trialIndex = 0;
        cubePosition = new Vector3(0.4324f, 1.1f, -0.309f); //TODO: change these values when the scene is ready
        offsetPosition = 0.0f;
        cubes = new GameObject[NUMCUBES];
        rigidbodies = new Rigidbody[NUMCUBES];
        answers = new float[NUMCUBES];

        for (int i = 0; i < NUMCUBES; i++) {

            cubeName = "cube" + i;
            Debug.Log("Creating new prefab");

            cubes[i] = Instantiate(cubePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            cubes[i].name = cubeName;
            rigidbodies[i] = cubes[i].GetComponent<Rigidbody>();
            rigidbodies[i].mass = 4.0f; // TODO: change this to the value of mass still TBD
            cubes[i].transform.position = cubePosition;
            cubePosition.x = cubePosition.x - 0.5f;

        }


    }


    public void resetScene()
    {
        for (int i = 0; i < NUMCUBES; i++)
        {
            //reset the position
            cubes[i].transform.position = cubePosition;
            cubePosition.x = cubePosition.x - 0.5f;

            //change the mass
            rigidbodies[i] = cubes[i].GetComponent<Rigidbody>();
            rigidbodies[i].mass = 4.0f; // TODO: change this to the value of mass still TBD


        }
    }

    public void OnCustomButtonPress()
    {

        if (canFinishTrial()) { 

            if (trialIndex < MAXNUMTRIALS)
            {
                Debug.Log("We pushed our custom button!");
                //save the answers
                getAnswers();
                //reset the attributes

                //increase the index
                
                trialIndex++;
            }
            else
            {
                //end the test
                Debug.Log("end of the test");
            }

        }
        else
        {
            Debug.Log("Can't finish de scene");
        }
    }

    public void getAnswers()
    {
        string objName;
        for (int i = 0; i < NUMCUBES; i++)
        {
            objName = slots[i].GetComponent<DetectColision>().getObjectName(); // for each slot, gets the name of the cube placed on it
            answers[i] = GameObject.Find(objName).GetComponent<Rigidbody>().mass; // for each cube, gets its mass
            Debug.Log("Answers");
            Debug.Log(i + " - " + answers[i]);
        }
        
        
    }

    public bool canFinishTrial()
    {
        for(int i = 0; i < NUMCUBES; i++)
        {
            if (slots[i].GetComponent<DetectColision> ().isPlacedOnSlot() == false)
                return false;
            
        }
        return true;
    }
}
