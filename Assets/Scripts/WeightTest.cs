using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeightTest : MonoBehaviour
{
    // Start is called before the first frame update
    public Rigidbody r;
    private float cronometer;
    public TextMeshPro t;
    void Start()
    {
        cronometer = 0;
        r = this.GetComponent<Rigidbody>();
        r.mass = r.mass + 1.0f;
        t.text = r.mass.ToString();
    }

    // Update is called once per frame
    void Update()
    {

        //this.rigidbody.mass = 5;
        cronometer = cronometer + Time.deltaTime;
        if(cronometer > 6.0f && r.mass < 40)
        {            
            r.mass = r.mass + 1.0f;
            t.text = r.mass.ToString();
            cronometer = 0;
        }

    }
}
