using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A manager for a custom Event system within Unity.
/// </summary>
public partial class EventManager : MonoBehaviour
{
    public delegate void BaseEvent();

    public static event BaseEvent OnInitiateXmlRefresh = () => { };
    public static void InitiateXmlRefresh() { OnInitiateXmlRefresh(); }

    public static event BaseEvent OnXmlSuccesfullyRefreshed = () => { };
    public static void XmlSuccesfullyRefreshed() { OnXmlSuccesfullyRefreshed(); }

    private static EventManager kInstance;
    private static Queue<Action> kEventsFromLastFrame;

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

        // 15 is a magic number to avoid any memory allocs/frees during runtime
        kEventsFromLastFrame = new Queue<Action>(15);
    }

    /// <summary>
    /// Invoke an event next frame.
    /// </summary>
    /// <para>
    /// The best way to invoke any events that are of a different type than BaseEvent
    /// is through enclosure. I.e., InvokeNextFrame(() => EventManager.ComplicatedEvent(1, 24, null, EnumVal));
    /// </para>
    /// <param name="e"></param>
    public static void Notify(Action e)
    {
        kEventsFromLastFrame.Enqueue(e);
    }
    
    /// <summary>
    /// Process all the events we received through notify() last frame.
    /// </summary>
    private void Update()
    {
        // Cache our number of events to prevent a potentially infinite loop if one event triggers another.
        int initialCount = kEventsFromLastFrame.Count;

        for (int i = 0; i < initialCount; ++i) 
        {
            Action e = kEventsFromLastFrame.Dequeue();
            try
            {
                e.Invoke();
            }
            catch (Exception except)
            {
                Debug.LogError(except.Message + "\n" + except.StackTrace);
            }
        }
    }
}
