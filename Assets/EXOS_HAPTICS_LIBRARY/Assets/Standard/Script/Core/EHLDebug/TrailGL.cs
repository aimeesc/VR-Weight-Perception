using UnityEngine;
using System.Collections;
using exiii.Collections;

namespace exiii.Unity
{
	public class TrailGL : MonoBehaviour 
	{
        #region Inspector

        [SerializeField, UnchangeableInPlaying]
        private int m_CountFrame = 90;

        [SerializeField]
        private Color m_Color = Color.white;

        #endregion

        private LimitedList<Vector3> m_Positions = new LimitedList<Vector3>();

        public LimitedList<Vector3> Positions { get { return m_Positions; } }

        private void Start()
        {
            m_Positions.MaxItemCount = m_CountFrame;
        }

        private void Update()
        {
            Positions.Add(transform.position);

            LineDrawerGL.DrawOneStroke(m_Positions, m_Color);
        }
    }
}
