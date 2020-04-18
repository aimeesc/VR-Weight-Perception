using System.Collections.Generic;

namespace exiii.Unity
{
    public interface IShapeContainer : IPenetrable
    {
        bool HasShapeData { get; }

        void CalcShapeStateSet(IEnumerable<IPenetrator> penetrators, ShapeStateSet shapeStateSet);
    }
}