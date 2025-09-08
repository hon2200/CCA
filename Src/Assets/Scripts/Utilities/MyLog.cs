using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


//输出日志
public static class MyLog
{
    //“深打印”字典，无论这个字典里面有多少复杂的类，统统给你把每个类里面的属性都打印出来
    //目前无法用于打印玩家字典，玩家的Attribute感觉继承可能过于复杂了，或是一些其他未知原因
    public static void PrintLoadedDictionary<key, Type>(Dictionary<key, Type> Dict, string LogPath = "Log/log.txt", bool overwrite = true)
    {
        string filePath = Path.Combine(Application.dataPath, LogPath);
        StringBuilder logContent = new StringBuilder();

        logContent.AppendLine($"Dict 内容 (Count: {Dict.Count})");
        logContent.AppendLine("====================================");

        foreach (var kvp in Dict)
        {
            logContent.AppendLine($"Key: {kvp.Key}");
            ProcessObject(kvp.Value, logContent, 1);
            logContent.AppendLine("------------------------------------");
        }

        WriteToFile(filePath, logContent, overwrite);
    }
    //打印玩家状态专用函数，复用性不强
    public static void PrintSimplifiedPlayerStatus(string LogPath = "Log/log.txt", bool overwrite = true)
    {
        string filePath = Path.Combine(Application.dataPath, LogPath);
        StringBuilder logContent = new StringBuilder();
        logContent.AppendLine($"PlayerStatus 内容");
        logContent.AppendLine("====================================");
        foreach (Player player in PlayerManager.Instance.Players.Values)
        {
            logContent.AppendLine($"player ID : {player.ID_inGame} MaxHP : {player.status.HP.Value} " +
                $"Bullet : {player.status.resources.Bullet.Value} Sword : {player.status.resources.Sword.Value} " + 
                $"SwordAvailable : {player.status.resources.AvailableSword.Value}");
            logContent.AppendLine();
        }

        WriteToFile(filePath, logContent, overwrite);
    }
    //打印字典中的特定字段
    public static void PrintSpecificPropertyInDictionary<key, Type>(Dictionary<key, Type> Dict,
    string propertyName,
    string LogPath = "Log/log.txt",
    bool overwrite = true)
    {
        string filePath = Path.Combine(Application.dataPath, LogPath);
        StringBuilder logContent = new StringBuilder();

        logContent.AppendLine($"Dict 内容 (Count: {Dict.Count}) - 仅显示属性: {propertyName}");
        logContent.AppendLine("====================================");

        foreach (var kvp in Dict)
        {
            logContent.AppendLine($"Key: {kvp.Key}");

            // 获取指定属性的值
            var property = typeof(Type).GetProperty(propertyName);
            if (property != null && property.CanRead)
            {
                object value = property.GetValue(kvp.Value);
                logContent.Append($"{propertyName}: ");
                ProcessObject(value, logContent, 1);
            }
            else
            {
                logContent.AppendLine($"无法访问属性: {propertyName}");
            }

            logContent.AppendLine("------------------------------------");
        }

        WriteToFile(filePath, logContent, overwrite);
    }

    // 打印字典里的多个指定字段
    public static void PrintSpecificPropertiesInDictionary<key, Type>(Dictionary<key, Type> Dict,
        string[] propertyNames,
        string LogPath = "Log/log.txt",
        bool overwrite = true)
    {
        string filePath = Path.Combine(Application.dataPath, LogPath);
        StringBuilder logContent = new StringBuilder();

        logContent.AppendLine($"Dict 内容 (Count: {Dict.Count}) - 显示属性: {string.Join(", ", propertyNames)}");
        logContent.AppendLine("====================================");

        foreach (var kvp in Dict)
        {
            logContent.AppendLine($"Key: {kvp.Key}");

            foreach (var propertyName in propertyNames)
            {
                var property = typeof(Type).GetProperty(propertyName);
                if (property != null && property.CanRead)
                {
                    object value = property.GetValue(kvp.Value);
                    logContent.Append($"{propertyName}: ");
                    ProcessObject(value, logContent, 1);
                }
                else
                {
                    logContent.AppendLine($"无法访问属性: {propertyName}");
                }
            }

            logContent.AppendLine("------------------------------------");
        }

        WriteToFile(filePath, logContent, overwrite);
    }
    //打印字典里的嵌套字段
    public static void PrintNestedPropertyInDictionary<key, Type>(
    Dictionary<key, Type> Dict,
    string nestedPropertyPath,  // 例如 "action.Value"
    string LogPath = "Log/log.txt",
    bool overwrite = true)
    {
        string filePath = Path.Combine(Application.dataPath, LogPath);
        StringBuilder logContent = new StringBuilder();

        logContent.AppendLine($"Dict 内容 (Count: {Dict.Count}) - 仅显示属性路径: {nestedPropertyPath}");
        logContent.AppendLine("====================================");

        foreach (var kvp in Dict)
        {
            logContent.AppendLine($"Key: {kvp.Key}");

            // 解析嵌套属性路径
            string[] propertyNames = nestedPropertyPath.Split('.');
            object currentObj = kvp.Value;

            bool propertyFound = true;
            foreach (var propertyName in propertyNames)
            {
                var property = currentObj?.GetType().GetProperty(propertyName);
                if (property != null && property.CanRead)
                {
                    currentObj = property.GetValue(currentObj);
                }
                else
                {
                    logContent.AppendLine($"无法访问属性: {propertyName}");
                    propertyFound = false;
                    break;
                }
            }

            if (propertyFound)
            {
                logContent.Append($"{nestedPropertyPath}: ");
                ProcessObject(currentObj, logContent, 1);
            }

            logContent.AppendLine("------------------------------------");
        }

        WriteToFile(filePath, logContent, overwrite);
    }
    // 提取出的公共写入文件方法
    public static void WriteToFile(string filePath, StringBuilder content, bool overwrite)
    {
        try
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!overwrite && File.Exists(filePath) && new FileInfo(filePath).Length > 0)
            {
                File.AppendAllText(filePath, content.ToString());
                //Debug.MyLog($"内容已追加到现有文件: {filePath}");
            }
            else
            {
                File.WriteAllText(filePath, content.ToString());
                //Debug.Log($"内容已写入新文件: {filePath}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"写入日志文件失败: {e.Message}");
        }
    }
    private static void ProcessObject(object obj, StringBuilder logContent, int indentLevel)
    {
        if (obj == null)
        {
            logContent.AppendLine($"{GetIndent(indentLevel)}null");
            return;
        }

        Type objType = obj.GetType();

        // Handle primitive types, strings, etc.
        if (objType.IsPrimitive || objType == typeof(string) || objType.IsEnum)
        {
            logContent.AppendLine($"{GetIndent(indentLevel)}{obj}");
            return;
        }

        // Handle collections
        if (typeof(System.Collections.IEnumerable).IsAssignableFrom(objType) && objType != typeof(string))
        {
            logContent.AppendLine();
            ProcessCollection((System.Collections.IEnumerable)obj, logContent, indentLevel);
            return;
        }

        // Handle complex objects
        var fields = objType.GetFields();
        var properties = objType.GetProperties();

        if (fields.Length == 0 && properties.Length == 0)
        {
            logContent.AppendLine($"{GetIndent(indentLevel)}{obj}");
            return;
        }
        //如果处理的复杂类有属性，先打回车再输出
        logContent.AppendLine();

        foreach (var field in fields)
        {
            object value = field.GetValue(obj);
            logContent.Append($"{GetIndent(indentLevel)}{field.Name}: ");
            ProcessObject(value, logContent, indentLevel + 1);
        }

        foreach (var property in properties)
        {
            if (property.CanRead)
            {
                try
                {
                    object value = property.GetValue(obj);
                    logContent.Append($"{GetIndent(indentLevel)}{property.Name}: ");
                    ProcessObject(value, logContent, indentLevel + 1);
                }
                catch
                {
                    logContent.AppendLine($"{GetIndent(indentLevel)}{property.Name}: <无法读取>");
                }
            }
        }
    }

    private static void ProcessCollection(System.Collections.IEnumerable collection, StringBuilder logContent, int indentLevel)
    {
        Type collectionType = collection.GetType();

        // Check if it's a dictionary
        if (collectionType.IsGenericType && collectionType.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            logContent.AppendLine($"{GetIndent(indentLevel)}Dictionary ({collectionType.Name})");
            int count = 0;
            foreach (var item in collection)
            {
                var keyProp = item.GetType().GetProperty("Key");
                var valueProp = item.GetType().GetProperty("Value");
                object key = keyProp.GetValue(item, null);
                object value = valueProp.GetValue(item, null);

                logContent.Append($"{GetIndent(indentLevel + 1)}[{count}] Key: ");
                ProcessObject(key, logContent, indentLevel + 2);
                logContent.Append($"{GetIndent(indentLevel + 1)}  Value: ");
                ProcessObject(value, logContent, indentLevel + 2);
                count++;
            }
        }
        else // Handle other collections (List, Array, etc.)
        {
            logContent.AppendLine($"{GetIndent(indentLevel)}{collectionType.Name} (Count: {GetCollectionCount(collection)})");
            int index = 0;
            foreach (var item in collection)
            {
                logContent.Append($"{GetIndent(indentLevel + 1)}[{index}]: ");
                ProcessObject(item, logContent, indentLevel + 2);
                index++;
            }
        }
    }

    private static int GetCollectionCount(System.Collections.IEnumerable collection)
    {
        if (collection is System.Collections.ICollection col)
        {
            return col.Count;
        }

        // Fallback for non-ICollection enumerables
        int count = 0;
        foreach (var item in collection) count++;
        return count;
    }

    //处理首行缩进
    private static string GetIndent(int level)
    {
        return new string(' ', level * 2);
    }
}
