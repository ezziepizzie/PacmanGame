using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GAlgoT2530.IO
{
    public class SpriteFacotryCycle
    {
        [JsonPropertyName("frames")]
        public required List<int> Frames { get; set; }

        [JsonPropertyName("isLooping")]
        public required bool IsLooping { get; set; }

        [JsonPropertyName("frameDuration")]
        public required float FrameDuration { get; set; }
    }
}
