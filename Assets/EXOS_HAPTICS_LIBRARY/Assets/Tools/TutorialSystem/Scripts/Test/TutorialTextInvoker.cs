using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{

    public class TutorialTextInvoker : MonoBehaviour
    {
        [SerializeField]
        private GameObject textField;

        private ITutorialText textObservers;

        [SerializeField]
        private float timer;

        [SerializeField]
        private float invokeTime;

        private void Start()
        {
            textObservers = textField.GetComponent<ITutorialText>();
            Debug.Log(textObservers);
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (invokeTime < timer)
            {
                textObservers.TutorialStep(TutorialStepEnum.Next);
                timer = 0;
            }
        }
    }
}