using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Perform low pass filter processing on Vector 3
    /// </summary>
    public class Vector3Filter
    {
        private const float defaultGain = 0.8f;
        private float Gain;

        private Vector3 filteredVector;

        public Vector3 LastOutput
        {
            get { return filteredVector; }
        }

        public Vector3Filter(float gain = defaultGain)
        {
            Gain = gain;
        }

        public Vector3 Input(Vector3 vector)
        {
            filteredVector.x = filteredVector.x * Gain + vector.x * (1 - Gain);
            filteredVector.y = filteredVector.y * Gain + vector.y * (1 - Gain);
            filteredVector.z = filteredVector.z * Gain + vector.z * (1 - Gain);

            return filteredVector;
        }
    }
}