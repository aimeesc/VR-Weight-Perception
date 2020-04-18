using UnityEngine;
using System.Collections;
using exiii.Extensions;

namespace exiii.Unity
{
	public class PrefabMaker : MonoBehaviour 
	{
		#region Inspector

		[Header(nameof(PrefabMaker))]
		[SerializeField, PrefabField]
		private GameObject[] m_Prefabs;

		#endregion Inspector

		public void Instantiate() 
		{
            m_Prefabs.Foreach(x => Instantiate(x));
        }
	}
}
