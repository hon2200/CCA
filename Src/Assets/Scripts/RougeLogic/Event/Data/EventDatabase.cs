using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class EventDatabase : MonoSingleton<EventDatabase>
{
    /// <summary>All registered event instances (concrete EventDefine subclasses).</summary>
    public Dictionary<string, EventDefine> EventDictionary = new Dictionary<string, EventDefine>();

    /// <summary>
    /// Load JSON into EventDatabaseOrigin, then register all concrete events.
    /// </summary>
    public void LoadingEvents()
    {
        EventDatabaseOrigin.Instance.LoadingEvents();
    }

    private void Awake()
    {
        LoadingEvents();
        RegisterAllOptions();
    }

    /// <summary>
    /// Scans for all concrete EventDefine subclasses, creates one instance each, and registers them.
    /// Init() loads data from EventDatabaseOrigin.EventDictionary.
    /// </summary>
    private void RegisterAllOptions()
    {
        EventDictionary.Clear();

        var eventTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(EventDefine).IsAssignableFrom(t) && t != typeof(EventDefine));

        foreach (var type in eventTypes)
        {
            try
            {
                EventDefine evt = Activator.CreateInstance(type) as EventDefine;
                if (evt != null && !string.IsNullOrEmpty(evt.ID))
                {
                    EventDictionary[evt.ID] = evt;
                    Debug.Log($"[EventDatabase] Registered event: {evt.ID} ({type.Name})");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[EventDatabase] Failed to register event: {type.Name}, error: {e.Message}");
            }
        }

        Debug.Log($"[EventDatabase] Total registered events: {EventDictionary.Count}");
    }

    /// <summary>
    /// Get an event instance by ID.
    /// </summary>
    public EventDefine GetEvent(string eventID)
    {
        if (EventDictionary.TryGetValue(eventID, out EventDefine evt))
            return evt;
        Debug.LogWarning($"[EventDatabase] Event ID not found: {eventID}");
        return null;
    }
}
