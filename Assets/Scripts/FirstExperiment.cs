using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstExperiment : MonoBehaviour
{


    public GameObject[] cubes;
    public GameObject cubePrefab;
    public Rigidbody[] rigidbodies;
    private float offsetPosition;
    private int NUMCUBES = 4;

    // Start is called before the first frame update
    void Start()
    {

        Vector3 cubePosition = new Vector3(0.4324f, 1.1f, -0.309f); //TODO: change these values when the scene is readys
        offsetPosition = 0.0f;
        cubes = new GameObject[NUMCUBES];
        rigidbodies = new Rigidbody[NUMCUBES];

        for (int i = 0; i < 4; i++) { 
            Debug.Log("Creating new prefab");

            cubes[i] = Instantiate(cubePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            rigidbodies[i] = cubes[i].GetComponent<Rigidbody>();
            rigidbodies[i].mass = 4.0f; // TODO: change this to the value of mass still TBD
            cubes[i].transform.position = cubePosition;
            cubePosition.x = cubePosition.x - 0.5f;
        }


        // cubeA = Instantiate(cubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
      //  Rb = dude.GetComponent<RigidBody>();
        // cubeA.Rigidbody.mass = 3.0;
    }

    // Update is called once per frame
    void Update()
    {
        // Instantiate at position (0, 0, 0) and zero rotation.

    }
}
