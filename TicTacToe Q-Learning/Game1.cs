using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace TicTacToe_Q_Learning
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Board board;

        GameState state = GameState.MainMenu;
        Button aboutButton;
        Button backButton;
        Button startButton;
        Button quitButton;

        SpriteFont font;
        SpriteFont smallFont;

        Texture2D logoImg;
        Rectangle logoRect;
        const int LOGO_WIDTH = 600;
        const int LOGO_HEIGHT = 173;

        Texture2D bgImg;
        Rectangle bgRect;

        #region About
        string ABOUT =
            "This game was created in 24 hours for CodeDay Chicago 2018. " +
            "This uses a very basic Tic-Tac-Toe learning algorithm. " +
            "It works by assigning a value to each square and storing this value in a file. " +
            "When the computer wins, it increases the value of the squares it placed in, " + 
            "and decreases those that the player placed in. And if the player wins, the computer does the opposite, " + 
            "increasing the squares the player placed in and decreasing those it placed in. " + 
            "It tries to mimic the strategy of the winner of each game. " + 
            "However, this results in a more \"aggressive\" playstyle, in which the computer " + 
            "attempts to win rather than block. It would rather accept a loss than a draw. " + 
            "It also evolves some noticeable patterns as it learns. " + 
            "The best way to teach the computer is to have it train against itself, but " +
            "also have a human play it every once in a while. This will get the computer out of it's \"Comfort Zone\", " +  
            "and force it to try out new strategies. " + 
            "The computer learns from Human v. Human games as well, so set a good example! " + 
            "\n\nCreated by Duoplus Software, 12:00 pm Feb. 17 - 12:00 pm Feb. 18, 2018"
            ;
        #endregion

        const int SPACING = 10;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = graphics.PreferredBackBufferHeight = Info.WINDOW_SIZE;

            Window.Title = "Tic-Tac-Toe Basic Learning Algorithm";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //ComputerInformation c = new ComputerInformation(false);
            //c.ActionPairs.Clear();
            //c.SaveFile(Computer.FILE_PATH);

            //File.Create(Computer.FILE_PATH);

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("mainfont");
            smallFont = Content.Load<SpriteFont>("smallfont");
            ABOUT = ABOUT.FormatToWidth(smallFont, Info.WINDOW_SIZE - SPACING);

            board = new Board(Content.Load<Texture2D>("x content"), Content.Load<Texture2D>("o content"),
                Content.Load<Texture2D>("boardsquare"), Info.WINDOW_SIZE / 2 - Board.SIZE / 2,
                Info.WINDOW_SIZE / 2 - Board.SIZE / 2, font, GraphicsDevice);

            backButton = new Button("Back", font, SPACING, SPACING, Color.White, GraphicsDevice,
                Color.SkyBlue);
            backButton.AddClickHandler(x => GoBack());

            logoImg = Content.Load<Texture2D>("logo");
            logoRect = new Rectangle(Info.WINDOW_SIZE / 2 - LOGO_WIDTH / 2, SPACING, LOGO_WIDTH, LOGO_HEIGHT);

            startButton = new Button("Start", font, 0, logoRect.Y + logoRect.Height + SPACING, Color.White, 
                GraphicsDevice, Color.Lime);
            startButton.X = Info.WINDOW_SIZE / 2 - startButton.Width / 2;
            startButton.AddClickHandler(x => state = GameState.Playing);
            
            aboutButton = new Button("About", font, 0, startButton.Y + startButton.Height + SPACING, Color.White, GraphicsDevice,
                Color.SkyBlue);
            aboutButton.X = Info.WINDOW_SIZE / 2 - aboutButton.Width / 2;
            aboutButton.AddClickHandler(x => state = GameState.AboutMenu);

            quitButton = new Button("Quit", font, 0, aboutButton.Y + aboutButton.Height + SPACING, Color.White,
                GraphicsDevice, Color.DarkRed);
            quitButton.X = Info.WINDOW_SIZE / 2 - quitButton.Width / 2;
            quitButton.AddClickHandler(x => Exit());

            bgImg = Content.Load<Texture2D>("background");
            bgRect = new Rectangle(0, 0, Info.WINDOW_SIZE, Info.WINDOW_SIZE);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Input.Update();

            //board.UpdateStates(gameTime);
            if (state == GameState.Playing)
            {
                board.Update(gameTime);
                backButton.Update(gameTime);
            }
            else if (state == GameState.AboutMenu)
            {
                backButton.Update(gameTime);
            }
            else if (state == GameState.MainMenu)
            {
                startButton.Update(gameTime);
                aboutButton.Update(gameTime);
                quitButton.Update(gameTime);
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (state == GameState.Playing)
            {
                board.Draw(spriteBatch);
                backButton.Draw(spriteBatch);
            }
            else if (state == GameState.AboutMenu)
            {
                spriteBatch.Draw(bgImg, bgRect, Color.White);
                backButton.Draw(spriteBatch);
                spriteBatch.DrawString(smallFont, ABOUT, new Vector2(SPACING, backButton.Y + backButton.Height + SPACING),
                    Color.White);
            }
            else if (state == GameState.MainMenu)
            {
                spriteBatch.Draw(bgImg, bgRect, Color.White);
                startButton.Draw(spriteBatch);
                aboutButton.Draw(spriteBatch);
                quitButton.Draw(spriteBatch);
                spriteBatch.Draw(logoImg, logoRect, Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void GoBack()
        {
            switch (state)
            {
                case GameState.Playing:
                case GameState.AboutMenu:
                    state = GameState.MainMenu;
                    break;
            }
        }
    }

    public enum GameState
    {
        Playing,
        AboutMenu,
        MainMenu,
    }
}
