using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using static Scaffolding.Utilities.JsonUtility;

namespace Scaffolding.Utilities.Converters;

internal class EnumWithContractJsonConverter : JsonConverter
{
    public static bool IgnoreEnumCase { get; set; }

    public override bool CanConvert(Type objectType)
    {
        return (IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType).GetTypeInfo().IsEnum;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        bool flag = IsNullableType(objectType);
        Type enumType = (flag ? Nullable.GetUnderlyingType(objectType) : objectType);
        string[] names = Enum.GetNames(enumType);
        if (reader.TokenType == JsonToken.String)
        {
            string enumText = reader.Value!.ToString().ToLowerCase();
            if (!string.IsNullOrEmpty(enumText))
            {
                string text = names.Where((string n) => string.Equals(n.ToLowerCase(), enumText, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (text != null)
                {
                    return Enum.Parse(enumType, text);
                }
            }
        }
        else if (reader.TokenType == JsonToken.Integer)
        {
            int value = Convert.ToInt32(reader.Value);
            if (((int[])Enum.GetValues(enumType)).Contains(value))
            {
                return Enum.Parse(enumType, value.ToString());
            }
        }

        if (!flag)
        {
            string text2 = names.Where((string n) => string.Equals(n, "Undefined", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (text2 == null)
            {
                text2 = names.First();
            }

            return Enum.Parse(enumType, text2);
        }

        return null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        string text = value.ToString();
        if (!IgnoreEnumCase)
        {
            if (serializer.ContractResolver is CamelCasePropertyNamesContractResolver || serializer.ContractResolver is CustomCamelCasePropertyNamesContractResolver)
            {
                text = text.ToCamelCase();
            }
            else if (serializer.ContractResolver is SnakeCasePropertyNamesContractResolver)
            {
                text = text.ToSnakeCase();
            }
            else if (serializer.ContractResolver is LowerCasePropertyNamesContractResolver)
            {
                text = text.ToLowerCase();
            }
        }

        writer.WriteValue(text);
    }

    private bool IsNullableType(Type t)
    {
        if (t.GetTypeInfo().IsGenericType)
        {
            return t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        return false;
    }
}