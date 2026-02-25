using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Robust.Shared.Network;

namespace Content.Client._Adventure.Lust;

public enum OutputType
{
    Unknown,
    Vibrate,
    Rotate,
    Oscillate,
    Constrict,
    Temperature,
    Led,
    Position,
    HwPositionWithDuration,
    Spray,
}

// TODO(c4llv07e): Decide what to do with robust sandbox

public sealed class LustManager
{
    [Dependency] private static readonly IHttpClientHolder _http = default!;

    private const string BaseUrl = "http://127.0.0.1:3000/api/v1"; // Update as needed

    public static Dictionary<int, DeviceInfo> Devices { get; private set; } = new();

    public static async Task RefreshDevicesAsync()
    {
        var response = await _http.Client.GetAsync($"{BaseUrl}/devices");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        Devices = JsonSerializer.Deserialize<Dictionary<int, DeviceInfo>>(json) ?? new();
    }

    public static async Task SetDeviceOutputAsync(int deviceIndex, OutputType outputType, double level)
    {
        var outputTypeStr = outputType.ToString().ToLower();
        var url = $"{BaseUrl}/devices/{deviceIndex}/outputs/{outputTypeStr}/{level}";
        var response = await _http.Client.PutAsync(url, null);
        response.EnsureSuccessStatusCode();
    }

    public static async Task StopAllDevicesAsync()
    {
        var response = await _http.Client.PutAsync($"{BaseUrl}/devices/stop", null);
        response.EnsureSuccessStatusCode();
    }

    public static async Task StopDeviceAsync(int deviceIndex)
    {
        var response = await _http.Client.PutAsync($"{BaseUrl}/devices/{deviceIndex}/stop", null);
        response.EnsureSuccessStatusCode();
    }

    public sealed class DeviceInfo
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("features")]
        public Dictionary<int, FeatureInfo> Features { get; set; } = new();
    }

    public sealed class FeatureInfo
    {
        [JsonPropertyName("FeatureIndex")]
        public int FeatureIndex { get; set; }

        [JsonPropertyName("FeatureDescription")]
        public string FeatureDescription { get; set; } = string.Empty;

        [JsonPropertyName("Output")]
        public Dictionary<string, OutputDetail>? Output { get; set; }

        [JsonPropertyName("Input")]
        public Dictionary<string, InputDetail>? Input { get; set; }
    }

    public sealed class OutputDetail
    {
        [JsonPropertyName("Value")]
        public List<int>? Value { get; set; }
    }

    public sealed class InputDetail
    {
        [JsonPropertyName("Value")]
        public List<List<int>>? Value { get; set; }

        [JsonPropertyName("Command")]
        public List<string>? Command { get; set; }
    }
}
