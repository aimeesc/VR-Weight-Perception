using exiii.Unity.EXOS;
using System;
using System.Collections;
using System.Collections.Generic;
using exiii.Unity.Rx;
using exiii.Unity.Rx.Triggers;
using UnityEngine;

using Valve.VR;

namespace exiii.Unity.SteamVR
{
	public class SteamEHLEventGenerator : EHLEventGeneratorBase
    {
        [Header("ViveEventGenerator")]
        [SerializeField]
        private ELRType m_LRType;
        public ELRType LRType { get { return this.m_LRType; } }

        [Header("STEAMVR 2.2")]

        [SerializeField, Unchangeable]
        private SteamVR_Input_Sources m_HandType;
        public SteamVR_Input_Sources HandType { get { return this.m_HandType; } }

        [SerializeField]
        public SteamVR_Action_Boolean UseButton = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabPinch");

        [SerializeField]
        public SteamVR_Action_Boolean GrabButton = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("GrabGrip");

        private void OnValidate()
        {
            switch (m_LRType)
            {
                case ELRType.Right:
                    m_HandType = SteamVR_Input_Sources.RightHand;
                    break;

                case ELRType.Left:
                    m_HandType = SteamVR_Input_Sources.LeftHand;
                    break;
            }
        }

        private void Start()
        {
            if (UseButton == null || GrabButton == null)
            {
                EHLDebug.LogError("SteamVR_Action in ViveEventGenerator is not set.", this, "Controller", ELogLevel.Overview );
                enabled = false;
            }
        }

        private void Update()
        {
            if (IsStateDown(EManipulationType.Use))
            {
                UseStart();
            }
            else if (IsStateUp(EManipulationType.Use))
            {
                UseEnd();
            }
            else if (IsStateStay(EManipulationType.Use))
            {
                UseStay();
            }

            if (IsStateDown(EManipulationType.Grab))
            {
                GrabStart();
            }
            else if (IsStateUp(EManipulationType.Grab))
            {
                GrabEnd();
            }
            else if (IsStateStay(EManipulationType.Grab))
            {
                GrabStay();
            }
        }

        // is down state for manipulation types.
        public bool IsStateDown(EManipulationType type)
        {
            switch (type)
            {
                case EManipulationType.Use:
                    if (UseButton == null) { return false; }
                    return UseButton.GetStateDown(m_HandType);
                case EManipulationType.Grab:
                    if (GrabButton == null) { return false; }
                    return GrabButton.GetStateDown(m_HandType);
            }
            return false;
        }

        // is stay state for manipulation types.
        public bool IsStateStay(EManipulationType type)
        {
            switch (type)
            {
                case EManipulationType.Use:
                    if (UseButton == null) { return false; }
                    return UseButton.GetState(m_HandType);
                case EManipulationType.Grab:
                    if (GrabButton == null) { return false; }
                    return GrabButton.GetState(m_HandType);
            }
            return false;
        }

        // is up state for manipulation types.
        public bool IsStateUp(EManipulationType type)
        {
            switch (type)
            {
                case EManipulationType.Use:
                    if (UseButton == null) { return false; }
                    return UseButton.GetStateUp(m_HandType);
                case EManipulationType.Grab:
                    if (GrabButton == null) { return false; }
                    return GrabButton.GetStateUp(m_HandType);
            }
            return false;
        }

        private void UseStart()
        {
            EHLDebug.Log($"{name} : UseStart ", this, "Event");
            UseEventGenerator.Start.OnNext(Unit.Default);
        }

        private void UseStay()
        {
            EHLDebug.Log($"{name} : UseStay", this, "Event", ELogLevel.All);
            UseEventGenerator.Stay.OnNext(Unit.Default);
        }

        private void UseEnd()
        {
            EHLDebug.Log($"{name} : UseEnd", this, "Event");
            UseEventGenerator.End.OnNext(Unit.Default);
        }

        private void GrabStart()
        {
            EHLDebug.Log($"{name} : GrabStart", this, "Event");
            GrabEventGenerator.Start.OnNext(Unit.Default);
        }

        private void GrabStay()
        {
            EHLDebug.Log($"{name} : GrabStay", this, "Event", ELogLevel.All);
            GrabEventGenerator.Stay.OnNext(Unit.Default);
        }

        private void GrabEnd()
        {
            EHLDebug.Log($"{name} : GrabEnd", this, "Event");
            GrabEventGenerator.End.OnNext(Unit.Default);
        }
    }
}

