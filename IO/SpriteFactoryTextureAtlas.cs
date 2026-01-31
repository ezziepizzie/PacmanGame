using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GAlgoT2530.IO
{
    public class SpriteFactoryTextureAtlas
    {
        [JsonPropertyName("texture")]
        public required string TextureFileName { get; set; }

        [JsonPropertyName("regionWidth")]
        public required int RegionWidth { get; set; }

        [JsonPropertyName("regionHeight")]
        public required int RegionHeight { get; set; }
    }
}
