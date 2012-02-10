using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ProtectIt
{
    public class MouseManager : GameComponent
    {
        private MouseState previousState;

        public MouseState CurrentState { get; private set; }

        public Vector2 CurrentPosition
        {
            get
            {
                return new Vector2(
                    this.CurrentState.X,
                    this.CurrentState.Y
                    );
            }
        }

        public MouseManager(Game game)
            : base(game)
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            // Register mouse manager
            this.Game.Services.AddService(typeof(MouseManager), this);
        }

        public override void Update(GameTime gameTime)
        {
            this.previousState = this.CurrentState;
            this.CurrentState = Mouse.GetState();
        }

        public bool LeftClick
        {
            get 
            { 
                return this.previousState.LeftButton == ButtonState.Released && this.CurrentState.LeftButton == ButtonState.Pressed; 
            }
        }
    }
}
