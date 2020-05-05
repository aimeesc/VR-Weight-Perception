using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using System.IO;
using System;
using System.Globalization;
public class SecondExperiment : MonoBehaviour
{
    private bool isSecondObject;
    private int trialIndex, heavier, MAXNUMTRIALS = 5;
    public int participantID;
    public GameObject capsulePrefab, handle, yesButton, noButtom, nextButton;
    private GameObject capsule;
    private string answersPath, fileName, answerLine, inputObjects;
    private List<Vector3> handlePositions;
    private Vector3 capsulePosition;
    private float heavy, light, first, second;
    private Rigidbody rigidbody;
    void Start()
    {
        //init the handle positions
        handlePositions = new List<Vector3>(3);
        handlePositions[0] = new Vector3(0.0f, 1.038f, 0.0f); //extreme right
        handlePositions[1] = new Vector3(0.0f, 1.038f, 0.0f);
        handlePositions[2] = new Vector3(0.0f, 1.038f, 0.0f);
        handlePositions[3] = new Vector3(0.0f, 1.038f, 0.0f);
        handlePositions[4] = new Vector3(0.0f, 1.038f, 0.0f);

        //change this when the weights are known
        heavy = 4.0f;
        light = 2.0f;
        whoIsFirst();
        capsulePosition = new Vector3(0.0035f, 1.175f, 0.02f);
        answersPath = @Application.dataPath + "/Answers/Exp2/";
        fileName = answersPath + participantID + "-" + "-exp2-answers.csv";

        trialIndex = 0;
        //canFinishScene = false;
        capsule = Instantiate(capsulePrefab, capsulePosition, Quaternion.identity) as GameObject;
        rigidbody = capsule.GetComponent<Rigidbody>();
        rigidbody.mass = first;

        //SetActive(false);

    }

   


    public void OnCustomButtonPress()
    {
        answerLine = participantID.ToString() + "," + trialIndex + ","; // add the mass of the rigidbody
        rigidbody.mass = second;
        capsule.transform.position = capsulePosition;


        //display yes and no buttons
        //hide next object buttom


    }

    public void OnYesButtonPress()
    {

        //write the answers 
        answerLine = answerLine + first.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
        writeAnswer();

        if (trialIndex < MAXNUMTRIALS)
        {
            resetScene();

        }
        else
        {
            UnityEditor.EditorApplication.isPlaying = false; 
        }

    }

    public void OnNoButtonPress()
    {
        //write the answers 
        answerLine = answerLine + second.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);

        writeAnswer();
        if (trialIndex < MAXNUMTRIALS)
        {
            resetScene();
        }
        else
        {
            UnityEditor.EditorApplication.isPlaying = false;

        }
    }


    public void resetScene()
    {
        //hide yes and no buttom

        //reset flag

        //reset the position of the object

        //random the new position for the handle


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

    public void whoIsFirst()
    {
        System.Random rand = new System.Random();
        int n = rand.Next(0, 2);
        if (n == 0)
        {
            first = heavy;
            second = light;
        }

        else
            first = light;
            second = heavy;
    }



}
