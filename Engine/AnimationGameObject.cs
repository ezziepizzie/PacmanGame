using GAlgoT2530.IO;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Graphics;
using MonoGame.Extended.Serialization.Json;
using MonoGame.Extended.Content;
using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace GAlgoT2530.Engine
{
    public class AnimationGameObject : GameObject
    {
        public string CurrentAnimation
        {
            get 
			{
                return AnimatedSprite.CurrentAnimation;
            }

            set
            {
                AnimatedSprite.SetAnimation(value);
            }
        }
		
		public Texture2D Texture;
        public Texture2DAtlas TextureAtlas;
        public SpriteSheet SpriteSheet;
        public AnimatedSprite AnimatedSprite;

        private string _spriteFactoryName;
        private SpriteFactoryFileData _spriteFactoryFileData;
        
		
        public AnimationGameObject(string spriteFactoryName) 
            : this(string.Empty, spriteFactoryName)
        {
            // Intentionally left blank.
        }

        public AnimationGameObject(string name, string spriteFactoryName) 
            : base(name)
        {
            _spriteFactoryName = spriteFactoryName;
        }

        public override void LoadContent()
        {
            // Load the sprite factory file data 
            _spriteFactoryFileData = _game.Content.Load<SpriteFactoryFileData>(_spriteFactoryName, new JsonContentLoader());

            // Load the texture specified in the sprite factory file data
            string textureName = _spriteFactoryFileData.TextureAtlas.TextureFileName;
            textureName = textureName.Substring(0, textureName.LastIndexOf('.')); // remove file extension
            Texture = _game.Content.Load<Texture2D>(textureName);

            SetupAnimations();
        }

		// NOTE: This override is only for testing the class.
		//       Most of the time, this method will be overriden
		//       by the derived class.
        public override void Update()
        {
            AnimatedSprite.Update(ScalableGameTime.GameTime);
        }

		// NOTE: This override is only for testing the class.
		//       Most of the time, this method will be overriden
		//       by the derived class.
        public override void Draw()
        {
            _game.SpriteBatch.Begin();
            _game.SpriteBatch.Draw(AnimatedSprite, Position, Orientation, Scale);
            _game.SpriteBatch.End();
        }

        private void SetupAnimations()
        {
        /********************************************************************************
            PROBLEM 2: Setup the animaton.


            HOWTOSOLVE : 1. Copy the code below.
                         2. Paste it below this block comment.
                         3. Fill in the blanks.

            TextureAtlas = Texture2DAtlas.Create("TextureAtlas", Texture,
                                                 _spriteFactoryFileData.Texture________.Region________,
                                                 _spriteFactoryFileData.Texture________.Region________);

            SpriteSheet = new SpriteSheet("SpriteSheet", TextureAtlas);

            foreach (var (animationName, cycle) in _spriteFactoryFileData.Cycles)
            {
                // Define animation in sprite sheet
                SpriteSheet.DefineAnimation(animationName, builder => {
                    builder.IsLooping(cycle.________);
                    foreach (var frame in cycle.________)
                    {
                        builder.AddFrame(________, TimeSpan.FromSeconds(cycle.________));
                    }
                });
            }

            AnimatedSprite = new AnimatedSprite(SpriteSheet, _spriteFactoryFileData.Cycles.First().Key);
            AnimatedSprite.OriginNor_____ = Vector2.Zero;

        ********************************************************************************/
		
		TextureAtlas = Texture2DAtlas.Create("TextureAtlas", Texture,
                                                 _spriteFactoryFileData.TextureAtlas.RegionWidth,
                                                 _spriteFactoryFileData.TextureAtlas.RegionHeight);
            SpriteSheet = new SpriteSheet("SpriteSheet", TextureAtlas);
            foreach (var (animationName, cycle) in _spriteFactoryFileData.Cycles)
            {
                // Define animation in sprite sheet
                SpriteSheet.DefineAnimation(animationName, builder => {
                    builder.IsLooping(cycle.IsLooping);
                    foreach (var frame in cycle.Frames)
                    {
                        builder.AddFrame(frame, TimeSpan.FromSeconds(cycle.FrameDuration));
                    }
                });
            }
            AnimatedSprite = new AnimatedSprite(SpriteSheet, _spriteFactoryFileData.Cycles.First().Key);
            AnimatedSprite.OriginNormalized = Vector2.Zero;





        }
    }
}
