using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.IO;
using System;
using System.Globalization;
using TMPro;

public class SecondExperiment : MonoBehaviour
{
    private bool isSecondObject;
    private int trialIndex, heavier, MAXNUMTRIALS = 20, NUMPOSITIONS = 20;
    public int participantID;
    public GameObject handle, yesButton, noButton, nextButton, yesCylinder, noCylinder, nextCylinder, capsule;
    private string answersPath, fileName, answerLine, inputObjects;
    private List<Vector3> handlePositions;
    private Vector3 capsulePosition;
    private float heavy, light, first, second;
    private Rigidbody rigidbody;
    public TextMeshPro timer, question;
    private float cronometer;
    private int [,] positionIndex; //stores the position indexis of of the handle in the object
    private bool firstTrial;
    private string input;
    private Vector3 yesButtonPos, noButtonPos, nextButtonPos;

    private GameObject canvasChato;
    void Start()
    {
        //TODO: adjust this positions when the object is defined
        //init the handle positions

        handlePositions = new List<Vector3>(5);
        handlePositions.Add(new Vector3(0.0f, -1.0f, 0.00f)); //extreme right
        handlePositions.Add(new Vector3(0.0f, -0.5f, 0.00f)); //moderate right
        handlePositions.Add(new Vector3(0.0f, 0.0f, 0.00f)); //center 
        handlePositions.Add(new Vector3(0.0f, 0.5f, 0.00f)); //moderate left
        handlePositions.Add(new Vector3(0.0f, 1.0f, 0.00f)); //extreme left

        yesButtonPos = new Vector3(-0.3085f, 0.0334f, -1.051f);
        noButtonPos = new Vector3(-0.3085f, 0.0334f, -1.051f);
        nextButtonPos = new Vector3(-0.3085f, 0.0334f, -1.051f);//nextCylinder.transform.localPosition;


        positionIndex = new int[NUMPOSITIONS, 2];

        readPositionsIndex();
        readObjectsMass();
        shuffleArray();

        //change this when the weights are known
       
        whichIsFirst();
       
        capsulePosition = new Vector3(0.0035f, 1.1751f, -0.02f);
        handle.transform.localPosition = handlePositions[positionIndex[trialIndex, 0]];



        answersPath = @Application.dataPath + "/Answers/Exp2/";
        fileName = answersPath + participantID + "-" + "-exp2-answers.csv";

        trialIndex = 0;
        //canFinishScene = false;
        //capsule = Instantiate(capsulePrefab, capsulePosition, Quaternion.identity) as GameObject;
        //capsule = GameObject.Find("Exos_cylinder");
        rigidbody = capsule.GetComponent<Rigidbody>();
        rigidbody.mass = first;

       // yesButton.SetActive(false);
       // noButton.SetActive(false);
        cronometer = 0.0f;
        question.text = "";
        firstTrial = true;
        isSecondObject = false;
        canvasChato = GameObject.Find("[DebugWindow]");
        canvasChato.SetActive(false);

    }


    void LateUpdate()
    {
        cronometer += Time.deltaTime;


        if (firstTrial && cronometer <= 40.0f)
        {
            timer.text = cronometer.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        }
        else if (firstTrial && cronometer > 40.0f)
        {
            cronometer = 0.0f;
            firstTrial = false;
            timer.text = "Start!";

        }
        else if (!firstTrial)
        {
            timer.text = "";
        }

    }


    public void OnCustomButtonPress()
    {
        if (!isSecondObject)
        {
            Debug.Log("Next button pressed");
            answerLine = participantID.ToString() + "," + trialIndex + "," + heavy.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "," + 
                                                                             light.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "," +
                                                                             positionIndex[trialIndex, 0].ToString() + "," +
                                                                             positionIndex[trialIndex, 1].ToString(); 

            // add the mass of the rigidbody
                                                                            //change the mass and reset position
            rigidbody.mass = second;
            capsule.transform.position = capsulePosition;
            handle.transform.localPosition = handlePositions[positionIndex[trialIndex, 1]];

            //display yes and no buttons
            // yesButton.SetActive(true);
            //noButton.SetActive(true);


            //hide next object buttom
            // nextButton.SetActive(false);
            question.text = "Is the previous object heavier than this one?";
            //nextCylinder.transform.localPosition = nextButtonPos;
            isSecondObject = true;
        }
    }

    public void OnYesButtonPress()
    {
        if (isSecondObject) { 
            //write the answers 
            answerLine = answerLine  + "," + first.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "," + cronometer.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            writeAnswer();
            trialIndex++;

            if (trialIndex < MAXNUMTRIALS)
            {
                resetScene();

            }
            else
            {
                UnityEditor.EditorApplication.isPlaying = false; 
            }
            isSecondObject = false;
        }
        //yesCylinder.transform.localPosition = yesButtonPos;

    }

    public void OnNoButtonPress()
    {

        if (isSecondObject)
        {
            //write the answers 
            answerLine = answerLine + "," + second.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "," + cronometer.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            writeAnswer();
            trialIndex++;

            if (trialIndex < MAXNUMTRIALS)
            {
                resetScene();
            }
            else
            {
                UnityEditor.EditorApplication.isPlaying = false;

            }
            isSecondObject = false;

        }
        //noCylinder.transform.localPosition = noButtonPos;

    }


    public void resetScene()
    {

        
        //hide yes and no buttom
       // yesButton.SetActive(false);
        //noButton.SetActive(false);

        //nextButton.SetActive(true);
        question.text = "";


        //reset the position of the object
        capsule.transform.position = capsulePosition;

        //chooses randomly which is going to be the first object
        whichIsFirst();
        rigidbody.mass = first;

        //gets the position of the first handle
        handle.transform.localPosition = handlePositions[positionIndex[trialIndex, 0]];

        cronometer = 0.0f;


    }

    public void readObjectsMass()
    {
        int i = 0, j = 0;
        input = File.ReadAllText(@Application.dataPath + "/Resources/objectsList2.txt");
        //read the lines
        foreach (var row in input.Split('\n'))
        {
            //that's the only line we need to read
            if (i == participantID)
            {
                j = 0;
                foreach (var col in row.Trim().Split(' '))
                {
                    if(j == 0)
                        light = float.Parse(col, CultureInfo.InvariantCulture);
                    else
                        heavy = float.Parse(col, CultureInfo.InvariantCulture);
                    j++;
                }

            }
            i++;
        }
    }

    public void readPositionsIndex()
    {
        int i = 0, j = 0;
        input = File.ReadAllText(@Application.dataPath + "/Resources/positionList.txt");

        //positionIndex
        foreach (var row in input.Split('\n'))
        {
                j = 0;
                foreach (var col in row.Trim().Split(' '))
                {
                    positionIndex[i,j] = int.Parse(col, CultureInfo.InvariantCulture);
                    j++;

                }
                Debug.Log("Position index 0: " + positionIndex[i, 0] + "Position index j: " + positionIndex[i, 1]);

            
            i++;
        }
    }
    public void writeAnswer()
    {

        Debug.Log("I'm writing the answers");
        FileStream fileStream = null;
        fileStream = File.Open(fileName, File.Exists(fileName) ? FileMode.Append : FileMode.OpenOrCreate);

        using (StreamWriter fs = new StreamWriter(fileStream))
        {
            fs.WriteLine(answerLine);
        };

        fileStream.Close();
    }

    public void whichIsFirst()
    {
        System.Random rand = new System.Random();
        int n = rand.Next(0, 2);
        if (n == 0)
        {
            first = heavy;
            second = light;
        }

        else {
            first = light;
            second = heavy;
        }
        Debug.Log("heavy: " + heavy + "light" + light);
    }





    public void shuffleArray()
    {
        int n = NUMPOSITIONS;
        System.Random rand = new System.Random();

        for (int i = 0; i < n; i++)
        {

            swap(i, i + rand.Next(n - i));

        }

        

    }

    public void swap(int a, int b)
    {
        int temp0, temp1;
        temp0 = positionIndex[a,0];
        temp1 = positionIndex[a,1];

        positionIndex[a,0] = positionIndex[b,0];
        positionIndex[a,1] = positionIndex[b,1];

        positionIndex[b,0] = temp0;
        positionIndex[b,1] = temp1;

    }

}
