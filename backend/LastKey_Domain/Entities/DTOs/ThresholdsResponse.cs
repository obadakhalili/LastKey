using System.Text.Json.Serialization;

namespace LastKey_Domain.Entities.DTOs;

public class ThresholdsResponse
{
    [JsonPropertyName("1e-3")]
    public float e3 { get; set; }

    [JsonPropertyName("1e-4")]
    public float e4 { get; set; }
    
    [JsonPropertyName("1e-5")]
    public float e5 { get; set; }
}