using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A manager for a custom Event system within Unity.
/// </summary>
public class EventManager : MonoBehaviour
{
    /// <summary>
    /// The public enumeration of all events in the game.
    /// </summary>
    public enum EventType
    {
        INVALID_EVENT = -1,                 //no data
        INITIATE_XML_REFRESH = 0,           //no data
        XML_SUCCESFULLY_RERESHED = 1,       //no data
    }

    private static Queue<KeyValuePair<EventType, object[]>> kEventsLastFrame;
    private static Dictionary<EventType, List<IEventListener>> kListeners;
    private static EventManager kInstance;

    public const string INVALID_EVENT_DATA_MESSAGE = "Didn't pass correct arguments to EventSystem::notify() for event: ";

    /// <summary>
    /// Instantiate the EventManager. 
    /// NOTE: No IEventListeners should register themselves in Awake, this should be done in Start!
    /// </summary>
    private void Awake()
    {
        if (kInstance == null)
            kInstance = this;
        else
            Destroy(gameObject);

        kListeners = new Dictionary<EventType, List<IEventListener>>();
		kEventsLastFrame = new Queue<KeyValuePair<EventType, object[]>>();
    }

    /// <summary>
    /// Add an IEventListener to our listener list for the provided event.
    /// </summary>
    /// <param name="lis">The listener to register. Cannot be null.</param>
    /// <param name="e">Which event to register the listener for.</param>
    /// <exception cref="NullReferenceException">regiserListener is called in Awake instead of Start.</exception>
    public static void registerListener(IEventListener lis, EventType e)
    {
        if (!kListeners.ContainsKey(e))
            kListeners[e] = new List<IEventListener>();

        kListeners[e].Add(lis);
    }

    /// <summary>
    /// Add an IEventListener to our listener list for the provided variable number of events.
    /// </summary>
    /// <param name="lis">The listener to register. Cannot be null.</param>
    /// <param name="elist">The variable length list of events to register for</param>
    /// <exception cref="NullReferenceException">regiserListener is called in Awake instead of Start.</exception>
    public static void registerListener(IEventListener lis, params EventType[] elist)
    {
        foreach (EventType e in elist)
        {
            registerListener(lis, e);
        }
    }

    /// <summary>
    /// Remove a listener from all EventTypes.
    /// </summary>
    /// <param name="lis">The listener to be removed.</param>
    public static void unregisterListener(IEventListener lis)
    {
        if (kListeners == null)
            return;

        foreach (KeyValuePair<EventType, List<IEventListener>> eventListenerListPair in kListeners)
        {
            if (eventListenerListPair.Value.Contains(lis))
                eventListenerListPair.Value.Remove(lis);
        }
    }

    /// <summary>
    /// Notify the entire EventSystem of an event that occurred. Processing will occur at the start of the next frame.
    /// </summary>
    /// <param name="e">The event that occurred.</param>
    /// <param name="data">The data associated with that event, as described in the EventType enumeration.</param>
    /// <exception cref="ArgumentException">If e is EventType.INVALID_EVENT.</exception>
    public static void notify(EventType e, params object[] data)
    {
        if (e == EventType.INVALID_EVENT)
            throw new ArgumentException("Notified of invalid event!!");

		kEventsLastFrame.Enqueue(new KeyValuePair<EventType, object[]>(e, data));
    }

    /// <summary>
    /// Notify the entire EventSystem of an event that occurred. Processing will occur immediately. 
    /// Prefer to use notify() over notifyImmedate() except in cases where data will be destroyed at the end of this frame.
    /// </summary>
    /// <param name="e">The event that occurred.</param>
    /// <param name="data">The data associated with that event, as described in the EventType enumeration.</param>
    /// <exception cref="ArgumentException">If e is EventType.INVALID_EVENT.</exception>
	public static void notifyImmediate(EventType e, params object[] data)
    {
        if (e == EventType.INVALID_EVENT)
            throw new ArgumentException("Notified of invalid event!!");

        if (!kListeners.ContainsKey (e))
			return;

		foreach (IEventListener lis in kListeners[e])
		{
			lis.receiveEvent(e, data);
		}
	}
    
    /// <summary>
    /// Process all the events we received through notify() last frame.
    /// </summary>
    private void Update()
    {
        // Cache our number of events to prevent a potentially infinite loop if one event triggers another.
        int initialCount = kEventsLastFrame.Count;

        for (int i = 0; i < initialCount; ++i) 
        {
            var e = kEventsLastFrame.Dequeue();

            if (!kListeners.ContainsKey(e.Key))
                continue;

            foreach (IEventListener lis in kListeners[e.Key])
            {
                try
                {
                    lis.receiveEvent(e.Key, e.Value);
                }
                catch (Exception except)
                {
                    Debug.LogError(except.Message + "\n" + except.StackTrace);
                }
            }
        }
    }
}
