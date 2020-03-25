using Newtonsoft.Json;

namespace ShinebugReactor
{
    public class ReactorConfig
    {
        [JsonProperty] public bool ShouldReproduce { get; set; } = true;
    }
}
