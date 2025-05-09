using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class DictionaryUtil
{
    //融合同类字典，如果后融合的字典有重复的TKey会覆盖之前的数据
    public static void Merge<TKey, TValue>(this Dictionary<TKey, TValue> target,
                                    params Dictionary<TKey, TValue>[] sources)
    {
        foreach (var dict in sources)
        {
            foreach (var kvp in dict)
            {
                target[kvp.Key] = kvp.Value;
            }
        }
    }
    //把子字典转化成父字典
    public static Dictionary<TKey, TParent> ConvertToParentDictionary<TKey, TParent, TChild>(
       this Dictionary<TKey, TChild> childDict)
        where TChild : TParent // 约束 TChild 必须继承自 TParent
    {
        return childDict.ToDictionary(
            kvp => kvp.Key,
            kvp => (TParent)kvp.Value // 直接强制转换
        );
    }
}
