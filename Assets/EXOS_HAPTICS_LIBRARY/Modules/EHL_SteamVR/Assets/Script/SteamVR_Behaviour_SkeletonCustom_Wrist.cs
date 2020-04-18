using UnityEngine;
using Valve.VR;
using System.Linq;
using System;
using System.Collections.Generic;

namespace exiii.Unity.SteamVR
{

    public class SteamVR_Behaviour_SkeletonCustom_Wrist : SteamVR_Behaviour_Skeleton
    {
        [Serializable]
        private class Fingers
        {
            [HideInInspector]
            public string id;
            public Transform transform;
            private FingerNames fingersname;

        }

        [SerializeField]
        private bool m_AutoSetFingers=true;

        [SerializeField]
        List<Fingers> m_fingers = new List<Fingers>(Enum.GetNames(typeof(FingerNames)).Select(s => new Fingers() { id = s }));

        private void OnValidate()
        {
            if (m_AutoSetFingers)
            {
                m_fingers = new List<Fingers>(Enum.GetNames(typeof(FingerNames)).Select(s => new Fingers() { id = s }));

                AutoSetMetaFingers();
            }
        }
        
        // auto set fingers info of inspector by using MetaFingers
        void AutoSetMetaFingers()
        {
            var fingersmeta = GetComponentsInChildren<FingerMeta>();
            foreach (var fingermeta in fingersmeta)
            {
                m_fingers[GetFingerIndex(fingermeta)].transform = fingermeta.transform;
            }
        }
        
        //convert fingermeta to normal index
        int GetFingerIndex(FingerMeta fingermeta)
        {
            return 4 * ((int)fingermeta.FingerName / 10) + (int)fingermeta.FingerName % 10 - 1;
        }


        protected override void AssignBonesArray()
        {
            //default number of finger bones are 32
            bones = new Transform[32];
            
            for(int i = SteamVR_Skeleton_JointIndexes.thumbMetacarpal, j=0; j<m_fingers.Count&&i < bones.Length; i++)
            {
                if (IsMetacarpal(i))
                {
                    if (j == 0)
                    {
                        bones[i] = m_fingers[j].transform;
                        j++;
                    }
                    else
                    {
                        bones[i] = m_fingers[j].transform.parent;
                    }

                }
                else if(j<m_fingers.Count)
                {
                    bones[i] = m_fingers[j].transform;
                    j++;
                }
            }
        }

    

    }

}

