using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//定义可观测量T struct，可以给它传一个参数
public abstract class ObservableAttribute<T> where T : struct
{
    public event Action<T, T, string> OnValueChanged; // 旧值, 新值, 操作类型描述
    //UI文本变化
    //特效
    //音效

    private T _value;
    public T Value
    {
        get => _value;
        protected set => SetValue(value, "DirectSet");
    }
    //备份//备份直到读取期间，观测模式关闭
    private T save;
    //是否开启观测模式
    public bool onObserve = true;

    protected void SetValue(T newValue, string operationType)
    {
        if (!_value.Equals(newValue))
        {
            T oldValue = _value;
            _value = newValue;
            if(onObserve)
                OnValueChanged?.Invoke(oldValue, _value, operationType);
        }
    }
    protected void Save()
    {
        save = _value;
        onObserve = false;
    }
    protected void Load()
    {
        _value = save;
        onObserve = true;
    }
}