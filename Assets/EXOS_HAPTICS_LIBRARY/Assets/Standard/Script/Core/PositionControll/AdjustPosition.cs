using UnityEngine;
using UnityEngine.SceneManagement;

namespace exiii.Unity
{
    /// <summary>
    /// Move the position of the attached object when the key specified by Inspector is pressed
    /// </summary>
    public class AdjustPosition : MonoBehaviour
    {
        public enum EDirectionType
        {
            Origin,
            Camera,
            Marker,
        }

        public enum EPositionResetType
        {
            None,
            Reset,
            Load,
        }

        #region Inspector

        [SerializeField]
        private Transform m_Camera;

        [SerializeField]
        private Transform m_PositionMarker;

        [SerializeField]
        private EDirectionType m_DirectionType;

        [SerializeField]
        private bool m_AdjustHight = false;

        [SerializeField]
        private float m_HeightOffset;

        [SerializeField]
        private float m_Amplifer = 3;

        [SerializeField]
        private float MoveDistance = 0.1f;

        [SerializeField]
        private float RotationAngle = 10.0f;

        [SerializeField]
        private EPositionResetType m_AutoPositionOnStart = EPositionResetType.None;

        [SerializeField]
        private bool m_SaveToEachScene = false;

        #endregion Inspector

        private Transform m_MoveDirection;

        private float Amplifer => Amplifered ? m_Amplifer : 1;

        private int m_AmpliferCount = 0;

        private bool Amplifered => m_AmpliferCount > 0;

        private string SaveTransformKey
        {
            get
            {
                if (m_SaveToEachScene)
                {
                    return name + SceneManager.GetActiveScene().name;
                }
                else
                {
                    return name;
                }
            }
        }

#if UNITY_EDITOR

        protected void OnValidate()
        {
            if (!this.IsPrefab())
            {
                SearchStartMarker();
            }
        }

        private void SearchStartMarker()
        {
            if (m_PositionMarker != null) { return; }

            Debug.Log($"{name} : SearchStartMarker");

            var marker = FindObjectOfType<StartMarker>();

            if (marker != null) { m_PositionMarker = marker.transform; }
        }

#endif

        private void Start()
        {
            if (m_Camera == null && Camera.main != null)
            {
                m_Camera = Camera.main.transform;
            }

            if (m_PositionMarker == null && StartMarker.IsExist)
            {
                m_PositionMarker = StartMarker.Instance.transform;
            }

            switch (m_DirectionType)
            {
                case EDirectionType.Origin:
                    m_MoveDirection = transform;
                    break;

                case EDirectionType.Camera:
                    m_MoveDirection = m_Camera;
                    break;

                case EDirectionType.Marker:
                    m_MoveDirection = m_PositionMarker;
                    break;
            }

            PositionMove();

            /*
            if (m_AutoPositionOnStart != EPositionResetType.None)
            {
                Observable.Timer(TimeSpan.FromSeconds(1)).First().Subscribe(_ => PositionMove());
            }
            */
        }

        private void PositionMove()
        {
            switch (m_AutoPositionOnStart)
            {
                case EPositionResetType.None:
                    break;

                case EPositionResetType.Reset:
                    PositionReset();
                    break;

                case EPositionResetType.Load:
                    LoadTransform();
                    break;
            }
        }

        private void LateUpdate()
        {
            if (Amplifered) { m_AmpliferCount--; }
        }

        public void SaveTransform()
        {
            PlayerPrefs.SetString(SaveTransformKey, transform.CreateSaveString(false, true, true, true));

            PlayerPrefs.Save();
        }

        public void LoadTransform()
        {
            if (!PlayerPrefs.HasKey(SaveTransformKey)) { return; }

            transform.SetupFromSaveString(PlayerPrefs.GetString(SaveTransformKey), false, true, true, true);
        }

        public void DeleteSave()
        {
            if (!PlayerPrefs.HasKey(SaveTransformKey)) { return; }

            PlayerPrefs.DeleteKey(SaveTransformKey);
        }

        public void PositionReset()
        {
            Quaternion rotation = Quaternion.AngleAxis(m_PositionMarker.rotation.eulerAngles.y - m_Camera.rotation.eulerAngles.y, Vector3.up);

            transform.rotation *= rotation;

            Vector3 position = m_PositionMarker.position - m_Camera.position;

            if (m_AdjustHight)
            {
                position.y += m_HeightOffset;
            }
            else
            {
                position.y = 0;
            }

            transform.position += position;
        }

        public void MoveRight()
        {
            transform.position += Vector3.ProjectOnPlane(m_MoveDirection.transform.right, transform.up).normalized * MoveDistance * Amplifer;
        }

        public void MoveLeft()
        {
            transform.position -= Vector3.ProjectOnPlane(m_MoveDirection.transform.right, transform.up).normalized * MoveDistance * Amplifer;
        }

        public void MoveForward()
        {
            transform.position += Vector3.ProjectOnPlane(m_MoveDirection.transform.forward, transform.up).normalized * MoveDistance * Amplifer;
        }

        public void MoveBack()
        {
            transform.position -= Vector3.ProjectOnPlane(m_MoveDirection.transform.forward, transform.up).normalized * MoveDistance * Amplifer;
        }

        public void MoveUp()
        {
            transform.position += transform.up * MoveDistance * Amplifer;
        }

        public void MoveDown()
        {
            transform.position -= transform.up * MoveDistance * Amplifer;
        }

        public void TurnRight()
        {
            transform.Rotate(transform.up * RotationAngle * Amplifer);
        }

        public void TurnLeft()
        {
            transform.Rotate(transform.up * -RotationAngle * Amplifer);
        }

        public void SetAmplifire()
        {
            m_AmpliferCount = 2;
        }
    }
}