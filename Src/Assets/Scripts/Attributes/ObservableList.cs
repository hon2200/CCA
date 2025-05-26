using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.VersionControl;


//这个方法建立的ObservableList，当然就不希望外界通过List自带的原有函数来修改这个List
//而是希望所有修改这个List的方式都传递一个相应的message，以便这个List知道，并且通知委托的订阅
public class ObservableList<T> : List<T>
{
    public event Action<List<T>, string> OnListChanged; // 当前列表, 操作类型描述
    public bool IsObserving { get; private set; } = true; // 默认开启观察
    private List<T> _savedState; // 保存的状态

    // 保存当前列表状态（深拷贝）
    public void Save()
    {
        _savedState = new List<T>(this); // 创建当前列表的副本
        IsObserving = false; // 暂停触发事件
    }

    // 恢复到保存的状态
    public void Load()
    {
        if (_savedState == null)
            throw new InvalidOperationException("no save!!");

        // 清空当前列表并恢复保存的状态
        base.Clear();
        base.AddRange(_savedState);
        IsObserving = true; // 恢复观察
    }

    public void Add(T item, string message)
    {
        base.Add(item);
        if (IsObserving)
            OnListChanged?.Invoke(this, message);
    }

    public bool Remove(T item, string message)
    {
        bool removed = base.Remove(item);
        if (removed && IsObserving)
        {
            OnListChanged?.Invoke(this, message);
        }
        return removed;
    }

    public void Clear(string message)
    {
        base.Clear();
        if(IsObserving)
            OnListChanged?.Invoke(this, message);
    }

    public void RemoveAt(int index, string message)
    {
        T removedItem = this[index];
        base.RemoveAt(index);
        if(IsObserving)
            OnListChanged?.Invoke(this, message);
    }

    // 可选：重写其他方法，如 Insert, AddRange 等，但是目前懒的重写了
}
