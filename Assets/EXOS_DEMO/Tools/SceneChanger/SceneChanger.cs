using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace exiii.Unity.Develop
{
    public class SceneChanger : ExMonoBehaviour
    {
        public void ChangeScene(int index)
        {
            if (SceneManager.sceneCountInBuildSettings <= index) { return; }

            SceneManager.LoadScene(index);
        }
    }
}
