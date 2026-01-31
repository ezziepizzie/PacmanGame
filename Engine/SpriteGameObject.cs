using GAlgoT2530.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace GAlgoT2530
{
    public abstract class SpriteGameObject : GameObject
    {
        protected Texture2D Texture { get; set; }

        protected string _textureName;

        public SpriteGameObject(string textureName) : this("", textureName)
        {
            // Intentionally left blank
        }

        public SpriteGameObject(string name, string textureName) : base(name)
        {
            _textureName = textureName;
        }

        public override void LoadContent()
        {
            Texture = _game.Content.Load<Texture2D>(_textureName);
        }
    }
}
