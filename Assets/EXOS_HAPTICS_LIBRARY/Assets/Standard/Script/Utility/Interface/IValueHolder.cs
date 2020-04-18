using System;

namespace exiii.Unity
{
    //TODO: Change to ReactiveProperty or something
    public interface IValueHolder<TValueType>
    {
        TValueType Value { get; set; }

        IObservable<TValueType> OnValueChanged { get; }
    }
}