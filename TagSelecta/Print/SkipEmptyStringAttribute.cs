using System.Text.Json.Serialization.Metadata;

namespace TagSelecta.Print;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class SkipEmptyOrZeroAttribute : Attribute { }

public static class JsonSerializationModifiers
{
    public static void ApplySkipEmptyOrZero(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
            return;

        foreach (var prop in typeInfo.Properties)
        {
            if (
                prop.AttributeProvider?.IsDefined(typeof(SkipEmptyOrZeroAttribute), inherit: true)
                == true
            )
            {
                prop.ShouldSerialize = static (obj, value) =>
                {
                    if (value is null)
                        return false;

                    // Skip empty strings
                    if (value is string str)
                        return !string.IsNullOrEmpty(str);

                    // Skip numeric values equal to 0
                    if (
                        value
                        is byte
                            or sbyte
                            or short
                            or ushort
                            or int
                            or uint
                            or long
                            or ulong
                            or float
                            or double
                            or decimal
                    )
                    {
                        try
                        {
                            return Convert.ToDecimal(value) != 0m;
                        }
                        catch
                        {
                            return true;
                        }
                    }

                    return true;
                };
            }
        }
    }
}
