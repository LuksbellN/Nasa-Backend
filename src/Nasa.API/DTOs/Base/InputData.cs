using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Nasa.Domain.DTOs;

public class InputData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("lat")]
    public int Lat { get; set; }

    [JsonPropertyName("lon")]
    public int Lon { get; set; }

    [JsonPropertyName("depth_dm")]
    public int DepthDm { get; set; }

    [JsonPropertyName("temp_cC")]
    public int TempCc { get; set; }

    [JsonPropertyName("batt_mV")]
    public int BattMv { get; set; }

    [JsonPropertyName("acc_x")]
    public int AccX { get; set; }

    [JsonPropertyName("acc_y")]
    public int AccY { get; set; }

    [JsonPropertyName("acc_z")]
    public int AccZ { get; set; }

    [JsonPropertyName("gyr_x")]
    public int GyrX { get; set; }

    [JsonPropertyName("gyr_y")]
    public int GyrY { get; set; }

    [JsonPropertyName("gyr_z")]
    public int GyrZ { get; set; }

    [JsonPropertyName("crc16")]
    public int Crc16 { get; set; }

    [JsonPropertyName("ssha_ambiente")]
    public double SshaAmbiente { get; set; }

    [JsonPropertyName("chlor_a_ambiente")]
    public double ChlorAAmbiente { get; set; }
}