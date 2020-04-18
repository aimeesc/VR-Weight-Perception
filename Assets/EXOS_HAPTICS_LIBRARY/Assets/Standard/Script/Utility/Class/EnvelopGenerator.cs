using System.Diagnostics;

namespace exiii.Unity
{
    /// <summary>
    /// Generate a coefficient that varies with time
    /// </summary>
    public class EnvelopGenerator
    {
        public bool IsActive { get; set; } = false;

        public float AttackTime { get; set; }
        public float DecayTime { get; set; }
        public float ReleaseTime { get; set; }

        public float SustainLevel { get; set; }
        public float ReleaseLevel { get; set; }

        private Stopwatch sw = new Stopwatch();

        public float Value
        {
            get
            {
                if (IsActive == false) { return ReleaseLevel; }

                float totalMs = sw.ElapsedMilliseconds;
                float lapMs;

                if (totalMs < AttackTime && AttackTime != 0)
                {
                    lapMs = totalMs;
                    return (lapMs / AttackTime) * SustainLevel;
                }
                else if (totalMs < (AttackTime + DecayTime))
                {
                    return SustainLevel;
                }
                else if (totalMs < (AttackTime + DecayTime + ReleaseTime) && ReleaseTime != 0)
                {
                    lapMs = totalMs - (AttackTime + DecayTime);
                    return SustainLevel - (lapMs / ReleaseTime) * (SustainLevel - ReleaseLevel);
                    //      1 - (lapMs / 1000) * 0.7
                }
                else
                {
                    IsActive = false;
                    sw.Stop();

                    return ReleaseLevel;
                }
            }
        }

        public EnvelopGenerator(float attackTime = 1.0f, float decayTime = 1.0f, float releaseTime = 1.0f, float sustainLevel = 0.5f, float releaseLevel = 0.1f)
        {
            AttackTime = attackTime;
            DecayTime = decayTime;
            ReleaseTime = releaseTime;
            SustainLevel = sustainLevel;
            ReleaseLevel = releaseLevel;
        }

        public void Start()
        {
            sw.Reset();
            sw.Start();

            IsActive = true;
        }
    }
}