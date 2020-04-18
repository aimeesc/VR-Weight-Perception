using UnityEngine;

namespace exiii.Unity.Expantion
{
    /// <summary>
    /// Attach to the root of the model's bone and change the transform to be in the same posture as the target bone
    /// </summary>
    public class ArmatureSetter : MonoBehaviour
    {
        [SerializeField] private Transform origin;
        [SerializeField] private bool WorldSpace = false;

        public void SetArmature()
        {
            var children = GetComponentsInChildren<Transform>();
            var originChildren = origin.GetComponentsInChildren<Transform>();

            foreach (var c in children)
            {
                foreach (var oc in originChildren)
                {
                    if (c.gameObject.name == oc.gameObject.name)
                    {
                        if (WorldSpace)
                        {
                            c.rotation = oc.rotation;
                            c.position = oc.position;
                        }
                        else
                        {
                            c.localRotation = oc.localRotation;
                            c.localPosition = oc.localPosition;
                        }

                        continue;
                    }
                }
            }
        }
    }
}