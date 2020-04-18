using UnityEngine;
using System.Collections;
using exiii.Extensions;

namespace exiii.Unity
{
	public class DisableOnRelease : MonoBehaviour 
	{
		#region Inspector

		[Header(nameof(DisableOnRelease))]
		[SerializeField]
		private GameObject[] m_DisableTarget;

        #endregion Inspector

        private bool m_ForceDisable = true;

		private void Awake () 
		{
            if (!Debug.isDebugBuild || m_ForceDisable)
            {
                m_DisableTarget.Foreach(x => x.SetActive(false));
            }
		}
	}
}
