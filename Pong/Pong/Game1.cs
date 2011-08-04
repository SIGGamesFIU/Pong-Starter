using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Pong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
public class Game1 : Microsoft.Xna.Framework.Game
{

#region enums
    /*
        * enum is a fancy way of giving an integer value a readable name
        * We could have easily used an integer variable instead of an enum such as
        * 1 stands for Main Menu
        * 2 stands for playing game
        * 3 stand for game over, ect, ect.
        * The problem with using integer values instead of enums is that it can quickly become confusing and you can make mistakes
        * By using an enum, you can create an enum variable which is used to keep track of the current state of the game
        * in a more readable context.
        */
    enum GameState //Pong game has two game states, either we are at the main menu or we are playing the game.
    {
        MainMenu,
        PlayingGame
    }

    enum MainMenuState //In the main menu, we define all the different menu options the user can select
    {
        Start, //Start option will change the gamestate from main menu to playing the game
        Exit //Exit option will simply close and exit the game
    }

#endregion

#region Variables
    //Constant variables
    const int mMaxScore = 5; //The first person to reach this score wins the game
    const int mPaddleSpeed = 5; //The Vertical Speed of the paddle
    const int mMinBallSpeed = 5; //The slowest speed the ball can start at
    const int mMaxBallSpeed = 5; //The fastest speed (plus the slowest speed) the ball can start at

    //System Variables
    GraphicsDeviceManager mgraphics; //Used for storing information about the user's screen or monitor. Can set certain variables to make the game full screen, ect.
    SpriteBatch mspriteBatch; //Used for drawing object texture's on the screen.
        
    //Score Variables, used for keeping track of plaer 1 and player 2 scores
    int mScore1 = 0; //Player 1 score
    int mScore2 = 0; //Player 2 score
        
    //Font Variables, used for drawing text onto the screen
    SpriteFont mSpriteFont;
        
    //Variables for tracking various game states
    Boolean mGameOver = false; //Determines when a game is over
    GameState mGameState = GameState.MainMenu;
    MainMenuState mMainMenuState = MainMenuState.Start;

    //Color variables used for showing the user which option they have currently selected in the main menu
    Color mColorStart = Color.Red; //Used for the Start Main Menu Option
    Color mColorExit = Color.Red; //Used for the Exit Main Menu Option

    //Pong Objects
    PongObject mpaddle1; //Player 1's paddle
    PongObject mpaddle2; //Player 2's paddle
    PongObject mball; //The ball

    //Miscellaneous variables
    Random mRandom; //Used for generating random numbers (used for generating a random direction and speed for the ball each time it is reset)
    int mCollisionCounter = 0; //A counter to ensure the ball does not get locked in the paddle. When a collision is detected, collision will not be checked again until the counter reaches 0 again to give the ball time to escape the paddle.
    private float mHalfScreenWidth = 0; //Used for storing half of the width of the image
    private float mHalfScreenHeight = 0; //Used for storing half of the height of the image
    private Vector2 mPaddleVelocity = new Vector2(0, mPaddleSpeed); //Used as a constant velocity vector for the speed of the paddle
    private Boolean mLaunchBallTowardsPlayer1 = true;

#endregion

#region Constructor
    //Constructor which gets called when the game is first ran.
    public Game1()
    {
        //Do any intialization here
        mgraphics = new GraphicsDeviceManager(this); //Do not change this, used for getting the user's graphics information
        Content.RootDirectory = "Content"; //Determines the location where you keep your content. If you keep your content in another folder, you would change this to point to that new location.
        mRandom = new Random(); //Initailize the Random Generator
    }
#endregion

#region Initialize
    /// <summary>
    /// Allows the game to perform any initialization it needs to before starting to run.
    /// This is where it can query for any required services and load any non-graphic
    /// related content.  Calling base.Initialize will enumerate through any components
    /// and initialize them as well.
    /// </summary>
    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        //Currently, this method seems useless. It might serve a purpose and it seems most tutorial videos use initialize instead of the constructor, but it is unclear why.
        base.Initialize();
    }
#endregion

#region LoadContent
    /// <summary>
    /// LoadContent will be called once per game and is the place to load
    /// all of your content.
    /// </summary>
    protected override void LoadContent()
    {
        //Important to use this method to load images, sound, music, and fonts you may use.

        //Calculated Variables
        mHalfScreenWidth = GraphicsDevice.Viewport.Width / 2; //Half of the Screen's width
        mHalfScreenHeight = GraphicsDevice.Viewport.Height / 2; //Half of the Screen's height
            
        // Create a new SpriteBatch, which can be used to draw textures.
        mspriteBatch = new SpriteBatch(GraphicsDevice);

        //Load Fonts to be used to draw text in the game
        mSpriteFont = Content.Load<SpriteFont>("SpriteFont1");

        // TODO: use this.Content to load your game content here
        Texture2D tPaddleTexture = Content.Load<Texture2D>("paddle");
        Texture2D tBallTexture = Content.Load<Texture2D>("ball");

        //Initialize Pong Objects
        mball = new PongObject(tBallTexture, new Vector2(mHalfScreenWidth, mHalfScreenHeight), new Vector2(mMinBallSpeed, mMinBallSpeed)); //The starting position and velocity does not matter here, because the ball should be reset before the game starts
        mpaddle1 = new PongObject(tPaddleTexture, new Vector2(0, mHalfScreenHeight), mPaddleVelocity); //place paddle one on the left side of screen and in the center, make sure to set the velocity here as it should not be reset anywhere else
        mpaddle2 = new PongObject(tPaddleTexture, new Vector2(GraphicsDevice.Viewport.Width - tPaddleTexture.Width, mHalfScreenHeight), mPaddleVelocity); //place paddle two on right side of the screen and in the center, make sure to set the velocity here as it should not be reset anywhere else

        //Set up the game object positions
        ResetBall();
        ResetPaddles();
        ResetScores();

    }

    /// <summary>
    /// UnloadContent will be called once per game and is the place to unload
    /// all content.
    /// </summary>
    protected override void UnloadContent()
    {
        //UnloadContent method seems useless for the time being
        // TODO: Unload any non ContentManager content here
    }

#endregion

#region Update
    /// <summary>
    /// Allows the game to run logic such as updating the world,
    /// checking for collisions, gathering input, and playing audio.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        //Game Logic goes here
        switch (mGameState) //Check the game state the game is currently in
        {
            case GameState.PlayingGame: //if we are in playing game mode, then call the update function for playing the game
                UpdatePlayingGame(gameTime);
                break;
            case GameState.MainMenu://if we are in the main menu, then call the update function for the main menu
                UpdateMainMenu(gameTime);
                break;
        }
            
        base.Update(gameTime); //always recursive call for the main update method. Do not remove this.
    }
#endregion

#region Draw
    /// <summary>
    /// This is called when the game should draw itself.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        //Draw method is used for drawing all your images to the screen.

        GraphicsDevice.Clear(Color.Black); //Always clear the screen each time we are about to draw something new. Failure to do this will cause past drawings to be left on the screen.

        // TODO: Add your drawing code here
        mspriteBatch.Begin(); //Before starting to draw anything, always call the begin method of your spriteBatch you will use for drawing.

        switch (mGameState)//Check which State of the game we are in
        {
            case GameState.MainMenu: //if we are in the main menu, draw the main menu options
                DrawMainMenu(gameTime);
            break;
            case GameState.PlayingGame: //if we are playing the game, draw the game objects
                DrawPlayingGame(gameTime);
            break;
        }

        mspriteBatch.End(); //When finished drawing, always make sure to call the end method of your spriteBatch
            
        base.Draw(gameTime); //recursive call for Draw method. Do not remove this.
    }
#endregion

#region "Functions"

    #region Reset Ball and Paddle Functions
    private void ResetBall()
    {
        //Since we want our objects to be in the center of the screen, we must subtract half of the height from the position
        mball.Position = new Vector2(mHalfScreenWidth, mHalfScreenHeight - mball.HalfImageHeight); //Reset the ball's position to the center of the screen

        if (mLaunchBallTowardsPlayer1) //Do a random boolean to determine which way the ball is going to go
            mball.Velocity = new Vector2((mRandom.Next(mMaxBallSpeed) + mMinBallSpeed) * -1, mRandom.Next(mMaxBallSpeed) + mMinBallSpeed); //To launch the ball towards player one, multiply the x direction by -1
        else
            mball.Velocity = new Vector2((mRandom.Next(mMaxBallSpeed) + mMinBallSpeed), mRandom.Next(mMaxBallSpeed) + mMinBallSpeed);

        mCollisionCounter = 0; //Reset the collision Counter if the ball is ever reset
    }

    private void ResetPaddles()
    {
        //Since we want our objects to be in the center of the screen, we must subtract half of the height from the position
        mpaddle1.Position = new Vector2(0, mHalfScreenHeight - mpaddle1.HalfImageHeight);
        mpaddle2.Position = new Vector2(GraphicsDevice.Viewport.Width - mpaddle2.PaddleTexture.Width, mHalfScreenHeight - mpaddle2.HalfImageHeight);
    }

    private void ResetScores()
    {
        mScore1 = 0; //Initialize score to 0
        mScore2 = 0; //Initialize score to 0
        mGameOver = false; //If the scores are ever reset, we are starting a new game
    }
    #endregion

    #region Update Functions
    private void UpdateMainMenu(GameTime gametime)
    {
        if(Keyboard.GetState().IsKeyDown(Keys.Enter))//If user presses enter, then execute the menu option the player has selected
        {
            if (mMainMenuState == MainMenuState.Start) //Player wishes to start playing the game
            {
                mGameState = GameState.PlayingGame; //Change the game state to PlayingGame mode
            }
            else if (mMainMenuState == MainMenuState.Exit) //The player wishes to leave the game
            {
                this.Exit(); //exit the game
            }
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Down))//The player wants to select the option below the current menu option they are on
        {
            if(mMainMenuState == MainMenuState.Start)//If the player is on the start option, then change the menu option to the one below start
                mMainMenuState = MainMenuState.Exit;//This would be the exit option
        }
        if (Keyboard.GetState().IsKeyDown(Keys.Up))//The player wants to select the option above the current menu option they are on
        {
            if (mMainMenuState == MainMenuState.Exit)//If the player is on the exit option, then change the menu option to the one above exit
                mMainMenuState = MainMenuState.Start;//This would be the start option
        }

    }

    private void UpdatePlayingGame(GameTime gametime)
    {
        //Always check to make sure if the game is over or still playing
        if (mGameOver)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter)) //If the player pressed enter, they wish to start another round
            {
                ResetScores();
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.Escape)) //If the player pressed escape, they wish to go back to the main menu
            {
                mGameState = GameState.MainMenu;
                ResetScores();
            }

        }
        else //The game is not yet over. Continue Normal Playing Logic
        {
            // Allows the game to exit. If the player presses escape while playing the game, close and exit the game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            //If there was a recent collision with the ball and paddle, decrement the counter
            if (mCollisionCounter > 0)
                mCollisionCounter -= 1;

            // TODO: Add your update logic here
            //If the W key is pressed, move the paddle 1 up if it is within the screen's boundaries
            if (Keyboard.GetState().IsKeyDown(Keys.W) && mpaddle1.Position.Y > 0) 
            {
                //We subtract to the Y position to move up
                mpaddle1.Position = new Vector2(mpaddle1.Position.X, mpaddle1.Position.Y - mPaddleSpeed); 
            }
            //If the S key is pressed, move the paddle 1 down if it is within the screen's boundaries
            if (Keyboard.GetState().IsKeyDown(Keys.S) && mpaddle1.Position.Y + mpaddle1.PaddleTexture.Height < GraphicsDevice.Viewport.Height) 
            {
                //We add to the Y position to move down
                mpaddle1.Position = new Vector2(mpaddle1.Position.X, mpaddle1.Position.Y + mPaddleSpeed); 
            }
            //If the Up key is pressed, move the paddle 2 up if it is within the screen's boundaries
            if (Keyboard.GetState().IsKeyDown(Keys.Up) && mpaddle2.Position.Y > 0) 
            {
                //We subtract to the Y position to move up
                mpaddle2.Position = new Vector2(mpaddle2.Position.X, mpaddle2.Position.Y - mPaddleSpeed); 
            }
            //If the Down key is pressed, move the paddle 2 down if it is within the screen's boundaries
            if (Keyboard.GetState().IsKeyDown(Keys.Down) && mpaddle2.Position.Y + mpaddle2.PaddleTexture.Height < GraphicsDevice.Viewport.Height) 
            {
                //We add to the Y position to move down
                mpaddle2.Position = new Vector2(mpaddle2.Position.X, mpaddle2.Position.Y + mPaddleSpeed); 
            }

            //Check if the ball hit the top of the screen
            if (mball.Position.Y + mball.Velocity.Y < 0)
            {
                //If so, reverse the Y direction of the ball
                mball.Velocity = new Vector2(mball.Velocity.X, mball.Velocity.Y * -1);
            }

            //Check if the ball hit the bottom of the screen
            if (mball.Position.Y + mball.Velocity.Y + mball.PaddleTexture.Height > GraphicsDevice.Viewport.Height)
            {
                //If so, reverse the Y direction of the ball
                mball.Velocity = new Vector2(mball.Velocity.X, mball.Velocity.Y * -1);
            }

            //Update the new ball position by moving the ball
            mball.Position = new Vector2(mball.Position.X + mball.Velocity.X, mball.Position.Y + mball.Velocity.Y);

            //Capture the direction the ball is moving vertically. By default assume it is moving down.
            int direction = 1;
            if (mball.Velocity.Y < 0) //If the ball is moving up, change the direction
            {
                direction = -1;
            }
            //The reason we wanted to capture the vertical direction the ball was moving 
            //was so when the ball collides with a paddle, we can keep it moving in the same direction

            //Check if the ball collides with a paddle
            if (mball.Bounds.Intersects(mpaddle1.Bounds) && mCollisionCounter == 0) //The ball collided with paddle 1 and did not have a recent collision based on the collision counter
            {
                //Reverse the balls x direction, and give the ball a new vertical speed based on the distance from the center of the paddle
                mball.Velocity = new Vector2(mball.Velocity.X * -1, Vector2.Distance(mball.mCenter, mpaddle1.mCenter) / 10 * direction);
                mCollisionCounter = 50; //Set the counter se we know there is a recent collision
            }
            if (mball.Bounds.Intersects(mpaddle2.Bounds) && mCollisionCounter == 0) //The ball collided with paddle 2 and did not have a recent collision based on the collision counter
            {
                //Reverse the balls x direction, and give the ball a new vertical speed based on the distance from the center of the paddle
                mball.Velocity = new Vector2(mball.Velocity.X * -1, Vector2.Distance(mball.mCenter, mpaddle2.mCenter) / 10 * direction);
                mCollisionCounter = 50; //Set the counter se we know there is a recent collision
            }

            if (mball.Position.X + mball.PaddleTexture.Width / 2 < 0) //Player 2 just scored, because more than half of the ball went off of the left side of the screen
            {
                mScore2 += 1; //give player 2 a point
                mLaunchBallTowardsPlayer1 = false; //set the ball to launch towards player 2
                mGameOver = (mScore2 == mMaxScore); //if the player 2 reached the max score, game over
                this.ResetBall(); //reset the ball to the center of the screen
                if (mGameOver) //if game over, reset the paddles to the center
                    this.ResetPaddles();
            }
            if (mball.Position.X + mball.PaddleTexture.Width / 2 > GraphicsDevice.Viewport.Width) //Player 1 just scored
            {
                mScore1 += 1; //give player 1 a point
                mLaunchBallTowardsPlayer1 = true; //set the ball to launch towards player 1
                mGameOver = (mScore1 == mMaxScore); //if the player 1 reached the max score, game over
                this.ResetBall(); //reset the ball to the center of the screen
                if (mGameOver) //if game over, reset the paddles to the center
                    this.ResetPaddles();
            }
        }
    }
    #endregion

    #region Draw Functions
    private void DrawMainMenu(GameTime gametime)
    {
        //Draw all the main menu options
        if (mMainMenuState == MainMenuState.Start) //if the user is at the start option, highlight the start menu by setting the color to green and all other menu colors to red
        {
            mColorStart = Color.LightGreen; //Start option is highligheted
            mColorExit = Color.Red;
        }
        else //if the user is at the exit option, highlight the exit menu by setting the color to green and all other menu colors to red
        {
            mColorStart = Color.Red;
            mColorExit = Color.LightGreen; //Exit option is highligheted
        }
        //Draw the text for each menu option item with its appropriate color
        mspriteBatch.DrawString(mSpriteFont, "Start", new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 4), mColorStart);
        mspriteBatch.DrawString(mSpriteFont, "Exit", new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2), mColorExit);
    }

    private void DrawPlayingGame(GameTime gametime)
    {
        //Draw all the pong objects to the screen
        mspriteBatch.Draw(mpaddle1.PaddleTexture, mpaddle1.Position, Color.White); //draw paddle 1
        mspriteBatch.Draw(mpaddle2.PaddleTexture, mpaddle2.Position, Color.White);//draw paddle 2
        mspriteBatch.Draw(mball.PaddleTexture, mball.Position, Color.White);//draw the ball

        //Draw the player's scores as text near the top of the screen.
        mspriteBatch.DrawString(mSpriteFont, "Player One: " + mScore1.ToString() + "   Player Two: " + mScore2.ToString(), new Vector2(GraphicsDevice.Viewport.Width / 4, 20), Color.Red);


        if (mGameOver) //If its game over, draw a message for the winning player and options to continue or leave the game
        {
            if (mScore1 == mMaxScore) //if player 1 score matches the max score, player 1 is the winner
                mspriteBatch.DrawString(mSpriteFont, "PLAYER 1 WINS!!!  \n[Enter] to restart or [Esc] for Menu", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 4), Color.Green);
            else//otherwise, player 2 is the winner
                mspriteBatch.DrawString(mSpriteFont, "PLAYER 2 WINS!!!   \n[Enter] to restart or [Esc] for Menu", new Vector2(GraphicsDevice.Viewport.Width / 4, GraphicsDevice.Viewport.Height / 4), Color.Green);
        }
    }
    #endregion

#endregion

}
}
