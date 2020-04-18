using exiii.Unity.Device;

namespace exiii.Unity.EXOS
{
    public struct ExosForce
    {
        public EAxisType AxisType;
        public float ForceRatio;

        public ExosForce(EAxisType axisType, float forceRatio)
        {
            AxisType = axisType;
            ForceRatio = forceRatio;
        }
    }
}

