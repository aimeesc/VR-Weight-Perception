using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.IO;
using System;
using System.Globalization;
using TMPro;

public class FirstExperiment : MonoBehaviour
{
    private int NUMCUBES = 4, MAXNUMTRIALS = 15, trialIndex;
    public GameObject[] cubes;
    public GameObject[] slots = new GameObject[4];
    public GameObject cubePrefab;
    public Rigidbody[] rigidbodies;
    private float offsetPosition, cronometer;
    //Vector3 cubePosition;
    private string answersPath, fileName, answerLine, inputObjects;
    private float[] answers, tempWeights;
    private float[,] orderedWeights, weights;
    public int participantID;
    private List<Vector3> cubePositions = new List<Vector3>(4);
    private int[] positionIndex;
    public TextMeshPro timer;
    private bool firstTrial;

    /* equivalence from slots and weight
     * 0 = heavier
     * 1 = heavy
     * 2 = light
     * 3 = lighter
     */


    void Start()
    {

        answersPath = @Application.dataPath + "/Answers/Exp1/";
        fileName = answersPath + participantID + "-" + "-exp1-answers.csv";
        int i = 0, j = 0, k = 0;
        string cubeName;
        trialIndex = 0;
        positionIndex = new int[4] { 0, 1, 2, 3 };
        cubePositions.Add(Vector3.zero);
        cubePositions.Add(Vector3.zero);
        cubePositions.Add(Vector3.zero);
        cubePositions.Add(Vector3.zero);


        shuffleArray();
        setCubePositions();
        
        offsetPosition = 0.0f;
        cubes = new GameObject[NUMCUBES];
        rigidbodies = new Rigidbody[NUMCUBES];
        answers = new float[NUMCUBES];

        orderedWeights = new float[15, NUMCUBES];
        weights = new float[15, NUMCUBES]; 
        tempWeights = new float[NUMCUBES];
        inputObjects = File.ReadAllText(@Application.dataPath + "/Resources/objectsList.txt");

        /* READ FORCES FROM THE OBJECT LIST*/
        foreach (var row in inputObjects.Split('\n'))
        {
            if (k >= (participantID * 15) && k < (participantID+1) * 15) // there needs to be some calculation to find out which line of the file is going to be used3
            {
               // Debug.Log("Im k: " + k);
                j = 0;

                //for each line, read the numbers on it into one position of the array
                foreach (var col in row.Trim().Split(' '))
                {
                    tempWeights[j] = float.Parse(col, CultureInfo.InvariantCulture);
                    j++;
                }

                //copy the values into the main matrix that contain the weights 
                for (j = 0; j < NUMCUBES; j++)
                {
                    weights[i, j] = tempWeights[j];
                }

                //sort and reverse the values in the temporary array
                Array.Sort(tempWeights); 
                Array.Reverse(tempWeights);

                //copy the values into the matrix that
                for (j = 0; j<NUMCUBES; j++)
                {
                    orderedWeights[i, j] = tempWeights[j];
                }
                i++;

            }
            k++;
        }



        for (i = 0; i < NUMCUBES; i++) {

            cubeName = "cube" + i;
            cubes[i] = Instantiate(cubePrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
            cubes[i].name = cubeName;
            rigidbodies[i] = cubes[i].GetComponent<Rigidbody>();
            rigidbodies[i].mass = weights[trialIndex,i]; // TODO: change this to the value of mass still TBD
            cubes[i].transform.position = new Vector3(cubePositions[i].x, cubePositions[i].y, cubePositions[i].z);

        }

        answerLine = participantID.ToString() + "," + trialIndex + ",";
        writeOrderedVector(); //order the forces for the answer file
        cronometer = 0.0f;
        firstTrial = true;
    }



    void LateUpdate()
    {
        cronometer += Time.deltaTime;

       
        if(firstTrial && cronometer <= 40.0f)
        {
            timer.text = cronometer.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }else if(firstTrial && cronometer > 40.0f)
        {
            cronometer = 0.0f;
            firstTrial = false;
            timer.text = "Start!";

        }else if(!firstTrial && cronometer > 15.0f)
        {
            timer.text = "";
        }

    }

    public void resetScene()
    {
        shuffleArray();
        setCubePositions();

        for (int i = 0; i < NUMCUBES; i++)
        {
            //reset the position
            cubes[i].transform.position = new Vector3(cubePositions[i].x, cubePositions[i].y, cubePositions[i].z);

            //change the mass
            rigidbodies[i] = cubes[i].GetComponent<Rigidbody>();
            rigidbodies[i].mass = weights[trialIndex, i]; // TODO: change this to the value of mass still TBD
            
        }

        answerLine = participantID.ToString() + "," + trialIndex + ",";
        writeOrderedVector();
    }

    public void OnCustomButtonPress()
    {

        if (canFinishTrial()) {
            trialIndex++;

            getAnswers();
            writeAnswer();

            if (trialIndex < MAXNUMTRIALS)
            {
                //save the answers
              
                //reset the attributes
                resetScene();
                cronometer = 0.0f;

            }
            else
            {
                //end the test
                UnityEditor.EditorApplication.isPlaying = false; //TODO: change to app quit
            }

        }
        else
        {
           // Debug.Log("Can't finish de scene");
        }
    }

    public void getAnswers()
    {
        string objName;
        for (int i = 0; i < NUMCUBES; i++)
        {
            objName = slots[i].GetComponent<DetectColision>().getObjectName(); // for each slot, gets the name of the cube placed on it
            answers[i] = GameObject.Find(objName).GetComponent<Rigidbody>().mass; // for each cube, gets its mass
           // Debug.Log("Answers");
          //  Debug.Log(i + " - " + answers[i]);
            answerLine = answerLine + answers[i].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";

        }
        answerLine = answerLine + cronometer.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

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



    public void writeAnswer()
    {

       // Debug.Log("I'm writing the answers");
        FileStream fileStream = null;
        fileStream = File.Open(fileName, File.Exists(fileName) ? FileMode.Append : FileMode.OpenOrCreate);

        using (StreamWriter fs = new StreamWriter(fileStream))
        {
            fs.WriteLine(answerLine);
        };

        fileStream.Close();
    }


    public void writeOrderedVector()
    {

        for (int i = 0; i < NUMCUBES; i++)
            answerLine = answerLine + orderedWeights[trialIndex, i].ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + ",";
    }


    public void shuffleArray()
    {
        int n = NUMCUBES;
        System.Random rand = new System.Random();

        for (int i = 0; i < n; i++)
        {

            swap(i, i + rand.Next(n-i));

        }

    }

    public void swap(int a, int b)
    {
        int temp;
        temp = positionIndex[a];
        positionIndex[a] = positionIndex[b];
        positionIndex[b] = temp;
    }

    public void setCubePositions()
    {


        for(int i = 0; i < NUMCUBES; i++)
        {
            cubePositions[positionIndex[i]] = new Vector3(0.590f - (0.3f)*i, 2.1f, -0.309f); //TODO: change these values when the scene is ready
        }

        
    }
}
