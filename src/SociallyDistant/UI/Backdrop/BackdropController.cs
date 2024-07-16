#nullable enable

using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.UI.Backdrop
{
	public class BackdropController : DrawableGameComponent
	{
		private readonly IGameContext context;
		private readonly Queue<BackdropSettings> pendingBackdropChanges = new Queue<BackdropSettings>();
		private readonly float fadeDuration = 1;

		private Texture2D? white;
		
		private SpriteBatch batch = null!;
		private BackdropState  backdropImage = new();
		private BackdropState temporaryBackdropImage = new();
		private BackdropAnimationState animState;
		private float fadeProgress;
		

		public override void Initialize()
		{
			base.Initialize();

			this.batch = new SpriteBatch(Game.GraphicsDevice);
			
			// Hide the temporary backdrop until it's time to change the backdrop.
			this.temporaryBackdropImage.Tint = Color.Transparent;
			
			// Initialize the backdrop to its default settings.
			SetBackdropSettingsInternal(ref backdropImage, BackdropSettings.Default);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			switch (animState)
			{
				case BackdropAnimationState.Idle:
				{
					if (this.pendingBackdropChanges.TryDequeue(out BackdropSettings? settings))
					{
						animState = BackdropAnimationState.Fading;
						fadeProgress = 0;
						temporaryBackdropImage.Texture = settings.Texture;
						temporaryBackdropImage.Tint = settings.ColorTint;
					}
					break;
				}
				case BackdropAnimationState.Fading:
				{
					fadeProgress =
						(float) MathHelper.Clamp(fadeProgress + ((float) gameTime.ElapsedGameTime.TotalSeconds / fadeDuration), 0, 1);

					if (fadeProgress >= 1)
						animState = BackdropAnimationState.Done;
					
					break;
				}
				case BackdropAnimationState.Done:
				{
					backdropImage = temporaryBackdropImage;
					temporaryBackdropImage.Texture = null;
					temporaryBackdropImage.Tint = Color.Transparent;
					animState = BackdropAnimationState.Idle;
					break;
				}
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (white == null)
			{
				white = new Texture2D(Game.GraphicsDevice, 1, 1);
				white.SetData<Color>(new[] { Color.White });
			}

			int width = Game.GraphicsDevice.Viewport.Width;
			int height = Game.GraphicsDevice.Viewport.Height;

			Vector2 size = new Vector2(width, height);

			batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
				DepthStencilState.None, RasterizerState.CullNone);

			batch.Draw(this.backdropImage.Texture ?? white,
				new Rectangle(0,
					0,
					width,
					height),
				null,
				this.backdropImage.Tint,
				0,
				Vector2.Zero,
				SpriteEffects.None,
				0
			);
			batch.Draw(this.temporaryBackdropImage.Texture ?? white,
				new Rectangle(0,
					0,
					width,
					height),
				null,
				this.temporaryBackdropImage.Tint * fadeProgress,
				0,
				Vector2.Zero,
				SpriteEffects.None,
				0
			);

			batch.End();
		}

		private void SetBackdropSettingsInternal(ref BackdropState target, BackdropSettings settings)
		{
			target.Texture = settings.Texture;
			target.Tint = settings.ColorTint;
		}

		public void SetBackdrop(BackdropSettings settings)
		{
			pendingBackdropChanges.Enqueue(settings);
		}

		
		internal BackdropController(SociallyDistantGame game) : base(game)
		{
			this.context = game;
		}

		private struct BackdropState
		{
			public Texture2D? Texture;
			public Color Tint;
		}

		private enum BackdropAnimationState
		{
			Idle,
			Fading,
			Done
		}
	}
}