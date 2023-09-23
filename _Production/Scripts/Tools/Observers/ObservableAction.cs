using System;

namespace FT.Tools.Observers;

/// Manages observers that are interested in being notified when an action takes place.
/// <typeparam name="T">The type of the delegate representing the action. Must inherit from System.Delegate.</typeparam>
public class ObservableAction<T> : IObservableAction<T> where T : Delegate
{
    /// Represents the delegate that is being observed. 
    public T Action { get; private set; }

    public void AddObserver(T observer) => Action = Delegate.Combine(Action, observer) as T;
    public void RemoveObserver(T observer) => Action = Delegate.Remove(Action, observer) as T;
}
