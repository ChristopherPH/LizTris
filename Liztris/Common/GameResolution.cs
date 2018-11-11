#define USE_INDEPENDANT_RESOLUTION
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Common
{
    //  Standard resolution displays
    //  4:3             16:10 (WS Monitor)  16:9 (WS TV)    16:9 Name
    //  ----------      ----------          ----------      ----------
    //  640, 480    
    //  800, 600
    //  960, 720
    //  1024, 768                           1024, 576
    //                                      1152, 648
    //  1280, 960       1280, 800           1280, 720       (720p, HD)
    //                                      1366, 768
    //  1400, 1050
    //  1440, 1080      1440, 900
    //  1600, 1200                          1600, 900
    //                  1680, 1050
    //  1856, 1392
    //  1920, 1440      1920, 1200          1920, 1080      (1080p, FHD)
    //  2048, 1536      
    //                  2560, 1600          2560, 1440      (1440p, QHD)
    //                                      3840, 2160      (2160p, UHD, 4k)

    public abstract class GameResolution : Game
    {
#if USE_INDEPENDANT_RESOLUTION
        protected int GamePixelWidth => IndependentResolutionRendering.Resolution.VirtualWidth;
        protected int GamePixelHeight => IndependentResolutionRendering.Resolution.VirtualHeight;
        protected Matrix GameToMonitorMatrix => IndependentResolutionRendering.Resolution.getTransformationMatrix();
#else
        protected int GamePixelWidth => graphics.PreferredBackBufferWidth;
        protected int GamePixelHeight => graphics.PreferredBackBufferHeight;
        protected Matrix GameToMonitorMatrix => Matrix.Identity;
#endif

        protected int MonitorPixelWidth => GraphicsDevice.DisplayMode.Width;
        protected int MonitorPixelHeight => GraphicsDevice.DisplayMode.Height;

        protected Rectangle GameRectangle => new Rectangle(0, 0, GamePixelWidth, GamePixelHeight);

        protected abstract int WantedGameResolutionWidth { get; }
        protected abstract int WantedGameResolutionHeight { get; }

        protected abstract int WindowWidth { get; }
        protected abstract int WindowHeight { get; }
        protected abstract bool WindowFullScreen { get; }

#if USE_INDEPENDANT_RESOLUTION
        protected void SetupGameResolution(ref GraphicsDeviceManager graphics)
        {
            IndependentResolutionRendering.Resolution.Init(ref graphics);

            //Virtual Resolution (Game Resolution): 
            //  Amount of pixels for Draw() to draw on. This will be scaled to the Window resolution for display.
            //  Note: VirtualResolution doesn't need to be a valid resolution
            IndependentResolutionRendering.Resolution.SetVirtualResolution(WantedGameResolutionWidth, WantedGameResolutionHeight);

            //Window/Screen Resolution
            IndependentResolutionRendering.Resolution.SetResolution(WindowWidth, WindowHeight, WindowFullScreen);
        }
#else
        protected void SetupGameResolution(ref GraphicsDeviceManager graphics)
        {
            //Note: this is what is actually happening (The game resolution is set to the window resolution)
            //GameResolutionWidth = WindowWidth;
            //GameResolutionHeight = WindowHeight;

            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.IsFullScreen = WindowFullScreen;
            graphics.ApplyChanges();
        }
#endif

        protected override void Draw(GameTime gameTime)
        {
#if USE_INDEPENDANT_RESOLUTION
            IndependentResolutionRendering.Resolution.BeginDraw();
#endif
        }

        protected Vector2 TranslateLocation(Vector2 Location)
        {
#if USE_INDEPENDANT_RESOLUTION
            var vp = new Vector2(
                IndependentResolutionRendering.Resolution.ViewportX, 
                IndependentResolutionRendering.Resolution.ViewportY);

            var invert = Matrix.Invert(
                IndependentResolutionRendering.Resolution.getTransformationMatrix());

            return Vector2.Transform(Location - vp, invert);
#else
            return Location;
#endif
        }
    }
}
