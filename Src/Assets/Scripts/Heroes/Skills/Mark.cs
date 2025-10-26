using System.Collections.Generic;
using UnityEngine;

public class MarkManager
{
    private Dictionary<string, int> marks = new Dictionary<string, int>();

    public void Add(string id, int count = 1)
    {
        if (marks.ContainsKey(id))
            marks[id] += count;
        else
            marks[id] = count;
        Debug.Log($"🔖 获得印记：{id}（总计 {marks[id]}）");
    }

    public void Remove(string id, int count = 1)
    {
        if (!marks.ContainsKey(id)) return;
        marks[id] -= count;
        if (marks[id] <= 0)
        {
            marks.Remove(id);
            Debug.Log($"🗑️ 移除印记：{id}");
        }
    }

    public bool Has(string id) => marks.ContainsKey(id);
    public int Count(string id) => marks.ContainsKey(id) ? marks[id] : 0;

    public void Clear() => marks.Clear();

    public void Save() { /* 可序列化保存 */ }
    public void Load() { /* 可反序列化加载 */ }
}