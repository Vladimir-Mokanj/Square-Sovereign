using System;

namespace FT.Tools.Observers;

/// Provides methods for adding and removing observer delegates for a specific action.
/// <typeparam name="T">The type of the delegate to be observed.</typeparam>
public interface IObservableAction<in T> where T : Delegate
{
    /// Adds an observer delegate to be invoked when the observed action occurs.
    /// <param name="observer">The observer delegate to add.</param>
    void AddObserver(T observer);
        
    /// Removes a specific observer delegate so that it will no longer be invoked when the observed action occurs.
    /// <param name="observer">The observer delegate to remove.</param>
    void RemoveObserver(T observer);
}   