using Newtonsoft.Json;

namespace Domain.Totvs.Dtos.Writes
{
    /// <summary>
    /// escreve um campo object das escritas como chegou ao controller: o System.Text.Json do host entrega um JsonElement, que o Newtonsoft
    /// serializaria pelas propriedades (ValueKind); o texto bruto vai como está. Outros valores vão pelo serializador
    /// </summary>
    public class RawJsonConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override bool CanConvert(Type objectType) => true;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value is System.Text.Json.JsonElement element)
                writer.WriteRawValue(element.GetRawText());
            else
                serializer.Serialize(writer, value);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) =>
            throw new NotSupportedException();
    }
}
