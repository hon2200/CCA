using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ObservableString
{
    public event Action<string, string, string> OnValueChanged; // 旧值, 新值, 操作类型描述
    //UI文本变化
    //特效
    //音效

    private string _value;
    public string Value
    {
        get => _value;
        protected set => SetValue(value, "DirectSet");
    }
    public ObservableString()
    {
        _value = new("");
    }
    //备份//备份直到读取期间，观测模式关闭
    private string save;
    //是否开启观测模式
    public bool onObserve = true;

    protected void SetValue(string newValue, string operationType)
    {
        if (!_value.Equals(newValue))
        {
            string oldValue = _value;
            _value = newValue;
            if (onObserve)
                OnValueChanged?.Invoke(oldValue, _value, operationType);
        }
    }
    public void Save()
    {
        save = _value;
        onObserve = false;
    }
    public void Load()
    {
        _value = save;
        onObserve = true;
    }
}
