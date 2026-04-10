using System.Collections.Generic;

public abstract class EventDefine
{
    public string ID { get; protected set; }
    public string Name { get; protected set; }
    public List<string> Options { get; protected set; }
    public string Description { get; protected set; }
    public string NextEvent { get; protected set;  }

    protected EventDefine(string id)
    {
        ID = id;
        Init();
    }

    protected virtual void Init()
    {
        Options = Options ?? new List<string>();
        if (EventDatabaseOrigin.Instance?.EventDictionary == null)
            return;
        if (!EventDatabaseOrigin.Instance.EventDictionary.TryGetValue(ID, out EventDefineOrigin data) || data == null)
            return;
        ApplyDataFrom(data);
    }

    /// <summary>
    /// Applies data from a JSON-loaded EventDefineOrigin into this instance.
    /// </summary>
    protected void ApplyDataFrom(EventDefineOrigin data)
    {
        if (data == null)
            return;
        ID = data.ID ?? ID;
        Description = data.Description;
        Name = data.Name;
        Options = data.Options != null ? new List<string>(data.Options) : new List<string>();
        NextEvent = data.NextEvent;
    }

    public virtual void OnChoose(string option)
    {
        
    }
}
