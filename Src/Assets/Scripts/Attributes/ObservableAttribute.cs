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

    protected void SetValue(T newValue, string operationType)
    {
        if (!_value.Equals(newValue))
        {
            T oldValue = _value;
            _value = newValue;
            OnValueChanged?.Invoke(oldValue, _value, operationType);
            AfterValueChanged(oldValue, _value, operationType);
        }
    }

    protected virtual void AfterValueChanged(T oldValue, T newValue, string operationType) { }
}