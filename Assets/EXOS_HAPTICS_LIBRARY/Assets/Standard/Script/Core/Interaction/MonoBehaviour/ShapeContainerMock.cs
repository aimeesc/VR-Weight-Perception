namespace exiii.Unity
{
    public class ShapeContainerMock : ShapeContainerBase
    {
        public override bool HasShapeData { get { return false; } }

        protected override bool TryCalcPenetration(IPenetrator penetrator, out OrientedSegment penetration)
        {
            penetration = default(OrientedSegment);
            return false;
        }
    }
}