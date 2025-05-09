using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public static class TypeConverter
{
    //将父类的属性浅复制一遍，并且返回以此创建的新的子类
    public static Child ShallowConvertToChild<Child,Father>(Father father ) where Child : Father, new()
    {

        Child child = new();
        // 获取所有基类属性
        PropertyInfo[] parentProperties = typeof(ActionDefine).GetProperties();
        foreach (PropertyInfo parentProperty in parentProperties)
        {
            if (parentProperty.CanRead && parentProperty.CanWrite)
            {
                object value = parentProperty.GetValue(father);
                parentProperty.SetValue(child, value);
            }
        }
        return child;
    }
}
