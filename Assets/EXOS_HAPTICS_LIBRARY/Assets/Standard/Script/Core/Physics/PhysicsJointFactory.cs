using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace exiii.Unity
{
    public static class PhysicsJointFactory
    {
        public static void BuildSpring(ToolsHolder holder, ToolsHolder parent)
        {
            if (holder == null || parent == null) { return; }

            holder.Tools.ResetToolsPosition();
            parent.Tools.ResetToolsPosition();

            switch (EHLParameterHolder.SpringType)
            {
                case ESpringType.UnitySpring:
                    SetUnitySpring(holder, parent);
                    break;

                case ESpringType.ExSpring:
                    SetExSpring(holder, parent);
                    break;

                case ESpringType.UnityJoint:
                    SetUnityJoint(holder, parent);
                    break;

                case ESpringType.None:
                    break;
            }
        }

        // set ex spring.
        private static void SetExSpring(ToolsHolder holder, ToolsHolder parent)
        {
            if (holder == null || parent == null) { return; }

            var spring = holder.Tools.ToolsObject.GetOrAddComponent<ExSpring>();

            spring.Anchor = parent.Tools.ToolsObject.transform;
            spring.SetLength();

            spring.Force = EHLParameterHolder.SpringForce;
            spring.Power = EHLParameterHolder.SpringPower;

            spring.SetPhysicsRoot(holder.Tools.PhysicsRoot);

            if (EHLParameterHolder.DrawLine)
            {
                holder.Tools.ToolsObject
                    .UpdateAsObservable()
                    .Subscribe(_ => LineDrawerGL.DrawLine(spring.Anchor.position, spring.transform.position, Color.cyan));
            }
        }

        // set Unity spring.
        private static void SetUnitySpring(ToolsHolder holder, ToolsHolder parent)
        {
            if (holder == null || parent == null) { return; }

            var spring = holder.Tools.ToolsObject.GetOrAddComponent<SpringJoint>();

            spring.connectedBody = parent.Tools.ToolsObject.GetOrAddComponent<Rigidbody>();
            var distance = (spring.connectedBody.position - holder.Tools.ToolsObject.transform.position).magnitude;

            spring.maxDistance = distance;
            spring.minDistance = distance;

            spring.spring = EHLParameterHolder.SpringForce;
        }

        //private IDisposable m_FixedUpdateForJoint = null;

        private static void SetUnityJoint(ToolsHolder holder, ToolsHolder parent)
        {
            if (holder == null || parent == null) { return; }

            var joint = holder.Tools.ToolsObject.GetOrAddComponent<CharacterJoint>();
            joint.connectedBody = parent.Tools.ToolsObject.GetOrAddComponent<Rigidbody>();

            holder.Tools.ToolsObject.transform.LookAt(joint.connectedBody.transform);

            var vector = joint.connectedBody.position - holder.Tools.ToolsObject.transform.position;

            joint.anchor = holder.Tools.ToolsObject.transform.InverseTransformPoint(joint.connectedBody.position);
            joint.axis = holder.Tools.ToolsObject.transform.InverseTransformDirection(vector.normalized);

            var distance = vector.magnitude;

            SoftJointLimit limit;

            limit = joint.lowTwistLimit;
            limit.limit = 90;
            joint.lowTwistLimit = limit;

            limit = joint.highTwistLimit;
            limit.limit = 90;
            joint.lowTwistLimit = limit;

            limit = joint.swing1Limit;
            limit.limit = 90;
            joint.lowTwistLimit = limit;

            limit = joint.swing2Limit;
            limit.limit = 90;
            joint.lowTwistLimit = limit;

            /*
            if (m_FixedUpdateForJoint == null)
            {
                m_FixedUpdateForJoint = holder.Tools.ToolsObject
                    .FixedUpdateAsObservable()
                    .Subscribe(_ => holder.Tools.ToolsObject.transform.LookAt(joint.connectedBody.transform))
                    .AddTo(holder.Tools.ToolsObject);
            }
            */
        }

        public static void BuildJoint(ToolsHolder holder, ToolsHolder parent)
        {
            if (holder == null || parent == null) { return; }

            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.layer = holder.gameObject.layer;

            capsule.transform.parent = holder.Tools.ToolsObject.transform;

            capsule.transform.ResetLocal();

            capsule.transform.Rotate(Vector3.right * 90);
            capsule.transform.localPosition = -Vector3.forward;
            capsule.transform.localScale = Vector3.one * 0.9f;
        }
    }
}

