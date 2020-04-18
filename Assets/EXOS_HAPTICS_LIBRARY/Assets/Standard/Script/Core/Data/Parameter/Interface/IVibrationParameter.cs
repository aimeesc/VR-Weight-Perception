using UnityEngine;
using System.Collections;

namespace exiii.Unity
{
	public interface IVibrationParameter
	{
        float Duration { get; }
        float Frequency { get; }
        float Amplitude { get; }
    }
}
