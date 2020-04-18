using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using exiii.Unity.Rx;
using System;
using System.Linq;

namespace exiii.Unity
{
    public class TutorialTextHandler : MonoBehaviour, ITutorialText
    {
        [SerializeField]
        private List<TutorialStatu> m_TutorialTexts = new List<TutorialStatu>();

        [SerializeField]
        private float m_MoveSpeed;

        [SerializeField]
        private TextMesh m_TutorialTextMesh;

        [SerializeField, Unchangeable]
        private int m_IndexPointer;

        private void Start()
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x,
                                               transform.localScale.y, 
                                               transform.localScale.z);
            TutorialSelect(0);
        }

        private void Update()
        {
            transform.LookAt(Camera.main.transform);

            var speed = m_MoveSpeed * Time.deltaTime;

            var targetPosition = m_TutorialTexts[m_IndexPointer].Coordinate;
            if (m_TutorialTexts[m_IndexPointer].UseTransform)
            {
                targetPosition = m_TutorialTexts[m_IndexPointer].TransformCoodinate;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed);
        }

        public void TutorialStep(TutorialStepEnum step)
        {
            m_IndexPointer = Mathf.Clamp(m_IndexPointer + (int)step, 0, m_TutorialTexts.Count - 1);
            m_TutorialTextMesh.text = m_TutorialTexts[m_IndexPointer].Text;
        }

        public void TutorialSelect(int index)
        {
            m_IndexPointer = Mathf.Clamp(index, 0, m_TutorialTexts.Count - 1);
            m_TutorialTextMesh.text = m_TutorialTexts[m_IndexPointer].Text;
        }
    }

    [Serializable]
    public class TutorialStatu
    {
        [Multiline]
        public string Text;

        public Vector3 Coordinate;

        public bool UseTransform;
        //use transform
        public Transform Transform;
        public Vector3 Offset;

        public Vector3 TransformCoodinate { get { return Transform.position + Offset; } }
    }
}
