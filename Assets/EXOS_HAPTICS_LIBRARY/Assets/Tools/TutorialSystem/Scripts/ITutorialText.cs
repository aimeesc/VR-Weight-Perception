using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace exiii.Unity
{
    public interface ITutorialText
    {
        void TutorialStep(TutorialStepEnum step);
        void TutorialSelect(int index);
    }

    public enum TutorialStepEnum : int
    {
        Back = -1,
        Next = 1
    }
}