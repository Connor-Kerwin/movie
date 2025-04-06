using System.Text.Json;
using System.Text.Json.Serialization;

namespace Movie.Models;

/// <summary>
/// A custom json converter is used to guarantee that the list of genres returned via JSON is sent lowercase.
/// This is because we want to match the URL query parameters with the json responses (all lowercase!)
/// </summary>
public class GenreListJsonConverter : JsonConverter<List<MovieGenre>>
{
    public override List<MovieGenre>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // TODO: Ideally, we want to implement the read method.
        // But because we don't actually read them in yet, its fine
        
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, List<MovieGenre> value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var genre in value)
        {
            var asString = GenreMapper.GetGenreString(genre);
            writer.WriteStringValue(asString);
        }
        
        writer.WriteEndArray();
    }
}