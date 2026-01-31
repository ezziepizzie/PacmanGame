using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GAlgoT2530.IO
{
    public class SpriteFactoryFileData
    {
        [JsonPropertyName("textureAtlas")]
        public required SpriteFactoryTextureAtlas TextureAtlas { get; set; }

        [JsonPropertyName("cycles")]
        public required Dictionary<string, SpriteFacotryCycle> Cycles { get; set; }
    }
}
