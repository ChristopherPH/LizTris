using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;

namespace GameStateEngine
{
    public abstract class GameState : IDisposable
    {
        /// <summary>
        /// called when state is pushed on stack
        /// </summary>
        public virtual void OnInit  () { }

        /// <summary>
        /// called when state is popped off stack
        /// </summary>
        public virtual void OnCompleted() { }

        /// <summary>
        /// called when state is pushed over current state
        /// </summary>
        public virtual void OnPause() { }

        /// <summary>
        /// called when state is popped over current state
        /// </summary>
        public virtual void OnResume() { }


        public abstract void Draw(GameTime gameTime, ExtendedSpriteBatch spriteBatch);

        public enum StateOperation
        {
            StateRunning,
            StateCompleted
        }

        public abstract StateOperation Update(GameTime gameTime);



        /// <summary>
        /// Amount of time spent in state
        /// </summary>
        public double stateTimer { get; set; }


        protected ContentManager Content { get; private set; }
        protected IServiceProvider Services;


        /*
         * GameState control options below
         */

        /// <summary>
        /// Next state to add when current state has completed
        /// </summary>
        public virtual GameState NextState => null;

        /// <summary>
        /// Base path for content
        /// </summary>
        protected virtual string ContentRoot => "Content";

        //TODO: implement
        public virtual bool RenderPreviousState => false;

        
        public void SetServiceProvider(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                return;

            if (Content != null)
                return;

            Services = serviceProvider;

            // Create a new content manager to load content used just by this level.
            Content = new ContentManager(Services, ContentRoot);
            LoadContent();
        }

        protected abstract void LoadContent();

        public void Dispose()
        {
            if (Content != null)
            {
                Content.Unload();
                Content = null;
            }
        }
    }
}
