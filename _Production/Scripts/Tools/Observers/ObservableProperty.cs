using System;

namespace FT.Tools.Observers;

/// Encapsulates a property value and allows external observers to be notified of changes.
/// <typeparam name="T">The type of the property being observed.</typeparam>
public class ObservableProperty<T> : IObservableProperty<T>
{
    #region FIELDS_PROPERTIES

    /// The delegate that gets invoked when the property value changes.
    private Action<T> Action { get; set; }

    private T _value;

    public T Value
    {
        get => _value;
        private set
        {
            if (_value != null && _value.Equals(value)) return;
            if (_value == null && value == null) return;
            _value = value;
            Action?.Invoke(_value);
        }
    }

    #endregion

    #region CONSTRUCTORS

    /// Default constructor for initializing an empty observable property.
    public ObservableProperty()
    {
    }

    /// Initializes the observable property with a given value.
    /// <param name="value">The initial value.</param>
    public ObservableProperty(T value) => _value = value;

    #endregion

    #region METHODS

    public void AddObserver(Action<T> observer) => Action = Delegate.Combine(Action, observer) as Action<T>;
    public void RemoveObserver(Action<T> observer) => Action = Delegate.Remove(Action, observer) as Action<T>;

    /// Sets a new value for the property and notifies any observers of the change.
    /// <param name="value">The new value to set.</param>
    public void Set(T value) => Value = value;

    /// Returns a string representation of the current value of the property.
    public override string ToString() => Value.ToString();

    #endregion

    #region OPERATORS

    /// Allows for an implicit conversion from an ObservableProperty&lt;T&gt; instance to its underlying value of type T.
    /// <param name="o">The ObservableProperty instance to convert.</param>
    /// <returns>Returns the underlying value of type T.</returns>
    public static implicit operator T(ObservableProperty<T> o) => o.Value;

    /// Allows for an implicit conversion from a value of type T to an ObservableProperty&lt;T&gt; that wraps this value.
    /// <param name="value">The value of type T to wrap in an ObservableProperty.</param>
    /// <returns>Returns a new ObservableProperty instance containing the specified value.</returns>
    public static implicit operator ObservableProperty<T>(T value) => new(value);

    #endregion
}
