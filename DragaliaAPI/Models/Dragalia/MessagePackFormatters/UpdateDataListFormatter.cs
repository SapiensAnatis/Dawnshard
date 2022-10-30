using System.Reflection;
using System.Text;
using DragaliaAPI.Models.Dragalia.Responses.UpdateData;
using MessagePack;
using MessagePack.Formatters;

namespace DragaliaAPI.Models.Dragalia.MessagePackFormatters;

public class UpdateDataListFormatter : IMessagePackFormatter<UpdateDataList>
{
    public UpdateDataList Deserialize(
        ref MessagePackReader reader,
        MessagePackSerializerOptions options
    ) => throw new NotImplementedException();

    public void Serialize(
        ref MessagePackWriter writer,
        UpdateDataList value,
        MessagePackSerializerOptions options
    )
    {
        Type t = value.GetType();
        Dictionary<string, object> nonNullProps = new();
        foreach (PropertyInfo prop in t.GetProperties())
        {
            object? propValue = prop.GetValue(value);
            if (propValue is not null)
            {
                nonNullProps.Add(prop.Name, propValue);
            }
        }

        writer.WriteMapHeader(nonNullProps.Count);
        foreach (KeyValuePair<string, object> k in nonNullProps)
        {
            writer.WriteString(Encoding.UTF8.GetBytes(k.Key));

            byte[] serialized = MessagePackSerializer.Serialize(k.Value);
            writer.Write(serialized);
        }
    }
}
