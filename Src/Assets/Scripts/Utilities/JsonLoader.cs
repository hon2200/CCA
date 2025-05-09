using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


public class JsonLoader
{
    //输入文件地址，输出反序列化的Json文件
    public static TargetClass DeserializeObject<TargetClass>(string filePath, bool tupleConverter = false)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException("json文件未找到", filePath);

        string json = File.ReadAllText(filePath);
        if (tupleConverter == true)
        {
            var settings = new JsonSerializerSettings
            {
                Converters = { new TupleConverter() }
            };
            TargetClass result =  JsonConvert.DeserializeObject<TargetClass>(json, settings);
            return result ?? throw new InvalidDataException("JSON 内容无效");
        }

        else
        {
            TargetClass result = JsonConvert.DeserializeObject<TargetClass>(json);

            return result ?? throw new InvalidDataException("JSON 内容无效");
        }

    }
}

public class TupleConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(ValueTuple<,>);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var genericArgs = objectType.GetGenericArguments();
        var item1 = (jObject["Item1"] ?? jObject["item1"]).ToObject(genericArgs[0]);
        var item2 = (jObject["Item2"] ?? jObject["item2"]).ToObject(genericArgs[1]);
        return Activator.CreateInstance(objectType, item1, item2);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var tuple = (ITuple)value;
        writer.WriteStartObject();
        writer.WritePropertyName("Item1");
        serializer.Serialize(writer, tuple[0]);
        writer.WritePropertyName("Item2");
        serializer.Serialize(writer, tuple[1]);
        writer.WriteEndObject();
    }
}


