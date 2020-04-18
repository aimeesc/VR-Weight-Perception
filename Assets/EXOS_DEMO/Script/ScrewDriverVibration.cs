using exiii.Unity.EXOS;
using System;
using exiii.Unity.Rx;
using exiii.Unity.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using exiii.Unity.Device;

namespace exiii.Unity.Sample
{
    public class ScrewDriverVibration : MonoBehaviour, IExosGrabForceGenerator
    {
        public FastenableChecker fastenableChecker;
        public Screw screw;
        

        private EAxisType m_Abduction = EAxisType.Abduction;
        private EAxisType m_Flexion = EAxisType.Flexion;
        private float m_AbductionForceRatio;
        private float m_FlexionForceRatio;

        [SerializeField]
        protected float m_DeltaTime = 0.0f;
        [SerializeField]
        protected float m_DelayTime = 0.0f;

        public bool IsActive { get; private set; } = false;
        public bool IsDelay { get; private set; } = false;


        public void VibrateScrewDriver()
        {
            IsActive = true;
            IsDelay = true;

            
            ScrewDriverSoundController.Instance.StartPlay();
            Observable.Interval(TimeSpan.FromSeconds(m_DelayTime)).Subscribe(l =>
            {
                float vibrationAmplitude = 0.13f;
                bool isCollidingScrew = fastenableChecker.IsCollideScrew;
                bool isFastenable = fastenableChecker.IsFastenable;
                bool isScrewStuck = screw.IsScrewStuck;
                if (isCollidingScrew && !isFastenable && !isScrewStuck)
                {
                    vibrationAmplitude = 0.4f;
                }
                else if (isCollidingScrew && isScrewStuck)
                {
                    vibrationAmplitude = 0.5f;
                }

                IsDelay = false;
                m_AbductionForceRatio = vibrationAmplitude * Mathf.Sin(90.0f * l * Mathf.PI / 180.0f);
                m_FlexionForceRatio = vibrationAmplitude * Mathf.Sin(90.0f * l * Mathf.PI / 180.0f);
            }).AddTo(this);
        }

        public void StopVibration()
        {
        
            ScrewDriverSoundController.Instance.StopPlay();
            Observable.Timer(TimeSpan.FromSeconds(m_DelayTime+m_DeltaTime)).Subscribe(_ => 
            {
                IsActive = false;
            }).AddTo(this);
        }

        public void OnGenerate(IForceReceiver receiver, IGrabState state)
        {

        }

        public void OnGenerate(IExosForceReceiver receiver, IGrabState state)
        {
            if (!IsActive) { return; }
            if(IsDelay) { return; }

            receiver.AddDirectForceRatio(m_Abduction, m_AbductionForceRatio);
            receiver.AddDirectForceRatio(m_Flexion, m_FlexionForceRatio);
        }
    }
}
