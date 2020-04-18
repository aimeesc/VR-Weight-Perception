using UnityEngine;

namespace exiii.Unity
{
    /// <summary>
    /// Vector 3 is stored in an array, and the maximum value, the minimum value, and the average value of the magnitude are obtained from the array
    /// </summary>
    public class Vector3FilterArray
    {
        private int pointer = 0;
        private Vector3[] datas;

        public Vector3 Average
        {
            get
            {
                Vector3 ans = Vector3.zero;

                foreach (var d in datas)
                {
                    ans += d;
                }

                return ans / datas.Length;
            }
        }

        public Vector3 Max
        {
            get
            {
                Vector3 ans = Vector3.zero;

                foreach (var data in datas)
                {
                    if (data.sqrMagnitude > ans.sqrMagnitude) ans = data;
                }

                return ans;
            }
        }

        public Vector3 Min
        {
            get
            {
                Vector3 ans = Vector3.positiveInfinity;

                foreach (var data in datas)
                {
                    if (data.sqrMagnitude < ans.sqrMagnitude) ans = data;
                }

                return ans;
            }
        }

        public Vector3FilterArray(int size)
        {
            datas = new Vector3[size];
        }

        public Vector3 SignalIn(Vector3 input)
        {
            datas[pointer] = input;
            pointer = (pointer + 1) % datas.Length;

            return Average;
        }

        public void Reset()
        {
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] = Vector3.zero;
            }
        }
    }
}