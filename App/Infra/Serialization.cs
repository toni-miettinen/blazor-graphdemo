using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace VerticalSlice.Infra;

public static class Serialization
{
    private static JsonSerializer _serializer = new();

    public static string Serialize<T>(T obj) where T : class
    {
        MemoryStream ms = new();
        using BsonDataWriter writer = new(ms);
        _serializer.Serialize(writer, obj);

        return Convert.ToBase64String(ms.ToArray());
    }

    public static T? Deserialize<T>(string bsonBase64) where T : class
    {
        try
        {
            byte[] data = Convert.FromBase64String(bsonBase64);

            MemoryStream ms = new MemoryStream(data);
            using BsonDataReader reader = new(ms);
            T obj = _serializer.Deserialize<T>(reader) ?? throw new Exception("cannot deserialize");

            return obj;
        }
        catch
        {
            return null;
        }
    }
}