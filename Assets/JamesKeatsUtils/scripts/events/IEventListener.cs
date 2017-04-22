using UnityEngine;

/// <summary>
/// Interface for handling events in the EventManager class.
/// </summary>
public interface IEventListener 
{
    /// <summary>
    /// Handle an event that we registered for from the EventManager class.
    /// </summary>
    /// <param name="e">The enumeration of the event.</param>
    /// <param name="data">The data associated with the event as described in the enumeration.</param>
    void receiveEvent(EventManager.EventType e, object[] data);
}
