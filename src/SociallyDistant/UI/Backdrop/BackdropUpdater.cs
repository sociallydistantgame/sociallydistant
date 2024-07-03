#nullable enable
using System.Runtime.Loader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;

namespace SociallyDistant.UI.Backdrop
{
	public class BackdropUpdater : GameComponent
	{
		private readonly IGameContext context;
		
		private Texture2D dayTime = null!;
		private Texture2D nightTime = null!;
		private Texture2D dayTimePanic = null!;
		private Texture2D nightTimePanic = null!;
		
		private IWorldManager worldManager = null!;
		private GameMode gameMode;
		private bool isPanicking = false;
		private bool isNightTime = false;
		private BackdropController backdrop = null!;
		private IDisposable? gameModeObserver;

		public override void Initialize()
		{
			base.Initialize();

			worldManager = context.WorldManager;

			this.MustGetComponent(out backdrop);

			gameModeObserver = context.GameModeObservable.Subscribe(OnGameModeChanged);
			
			dayTime = Game.Content.Load<Texture2D>("/Core/Backgrounds/Socially-Distant-Bg-wallpapers-light-normal");
			dayTimePanic = Game.Content.Load<Texture2D>("/Core/Backgrounds/Socially-Distant-Bg-wallpapers-light-distorted");
			nightTime = Game.Content.Load<Texture2D>("/Core/Backgrounds/Socially-Distant-Bg-wallpapers-dark-normal");
			nightTimePanic = Game.Content.Load<Texture2D>("/Core/Backgrounds/Socially-Distant-Bg-wallpapers-dark-distorted");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				gameModeObserver?.Dispose();
			}
			
			base.Dispose(disposing);
		}
		
		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			
			if (gameMode != GameMode.OnDesktop && gameMode != GameMode.InMission)
			{
				if (isNightTime && !isPanicking)
					return;

				isNightTime = true;
				isPanicking = false;
				UpdateBackdrop();
				return;
			}

			DateTime timeOfDay = worldManager.World.GlobalWorldState.Value.Now;

			bool day = timeOfDay.Hour >= 7 && timeOfDay.Hour < 19;
			bool night = !day;

			if (isNightTime == night)
				return;

			isNightTime = night;
			UpdateBackdrop();
		}
		
		private void UpdateBackdrop()
		{
			bool night = isNightTime;
			bool panic = isPanicking;

			Texture2D? texture = null;

			if (panic)
			{
				texture = night ? nightTimePanic : dayTimePanic;
			}
			else
			{
				texture = night ? nightTime : dayTime;
			}

			this.backdrop.SetBackdrop(new BackdropSettings(Color.White, texture));
		}

		private void OnGameModeChanged(GameMode newGameMode)
		{
			gameMode = newGameMode;
		}

		internal BackdropUpdater(SociallyDistantGame game) : base(game)
		{
			context = game;
		}
	}
}