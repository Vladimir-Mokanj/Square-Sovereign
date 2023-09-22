using System;

namespace FT.Tools.Observers;

/// Represents an observable property that notifies observers whenever its value changes.
/// <typeparam name="T">The type of the property being observed.</typeparam>
public interface IObservableProperty<out T> : IObservableAction<Action<T>>
{
    /// Gets the current value of the observable property.
    T Value { get; }
}