using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Pong
{

class PongObject
{
/*
* A Pong Object is a class that represents all the objects required in a pong game
* I.E. The ball and the two Paddles.
* Instead of using two different classes (one for the ball and one for the paddle) we
* decided to combine everything into one class. The reason is because both a paddle and a
* ball will have the same type of properties and behave similarly the same way.
* They need a texture for the image, position to keep track where the object is to draw it,
* center to keep track of the center position of the object, boundaries to detect collisions,
* and a velocity to control the speed of the paddle or ball.
*/
        
#region Variables
//Variables used for a pong object
//Naming convention for all variables is to start with a lowercase m, followed by the name of the varaible

    #region external Variables
        //The following variables should either be made public or accessed through a public property
        public Vector2 mCenter; //Used for storing the center position of the object
        private Vector2 mPosition; //Used for storing the upper left corner position of the object and determines where the object will be drawn
        private Texture2D mPaddleTexture; //Texture used to store the image of the object and what will be drawn on the screen
        private Rectangle mBounds; //Bounding Box which is updated every time the object's position moves and is used to determine if this object intersects another object
        private Vector2 mVelocity; //Velocity used to detemine how fast the object can move when it is moving. (This is like a speed variable.)

    #endregion

    #region Internal Variables
        //The following variables are used for internal use only.
        //I.E. variables used for calculations in procedures
        private float mHalfImageWidth = 0; //Used for storing half of the width of the image
        private float mHalfImageHeight = 0; //Used for storing half of the height of the image

    #endregion




#endregion

        
#region "Properties"
//Properties used to get and set the private variables
//(Properites are the same the thing as accessor and mutator methods in Java)
//The naming convention for a property is just the name of the variable without the "m"

    public Texture2D PaddleTexture 
    {
        get
        {
            return mPaddleTexture;
        }
        set
        {
            mPaddleTexture = value;
            //Internal Variables
            mHalfImageWidth = (PaddleTexture.Width / 2); //get half of the images width
            mHalfImageHeight = (PaddleTexture.Height / 2); //get half of the images height
        }
    }
    public Rectangle Bounds 
    {
        get
        {
            return mBounds;
        }
        set
        {
            mBounds = value;
        }
    }
    public Vector2 Velocity 
    {
        get
        {
            return mVelocity;
        }
        set
        {
            mVelocity = value;
        }
    }
    public float HalfImageHeight 
    {
        get
        {
            return mHalfImageHeight;
        }
    }

    //When an object's position changes, make sure to update the change for the object's center position and boundaries
    public Vector2 Position
    {
        get
        {
            return mPosition;
        }
        set
        {
            mPosition = value;
            //Bounds are always the same as the new position and always the same as the width and height of the texture image
            Bounds = new Rectangle((int)mPosition.X, (int)mPosition.Y, PaddleTexture.Width, PaddleTexture.Height);
            //Since the position is the top left corner, to get the center simply add half of the image's width to the x and add half of the image's height to the y
            mCenter = new Vector2(mPosition.X + mHalfImageWidth, mPosition.Y + mHalfImageHeight);
        }
    }

#endregion

        
#region Constructor
//Constructor must be called to instantiate a pong object.
//This is where all variables are initialized when a pong object is created
    public PongObject(Texture2D pPaddleTexture, Vector2 pPosition, Vector2 pVelocity)
    {
        PaddleTexture = pPaddleTexture; //Set the texture for the image
        Position = pPosition;//Set the position
        Velocity = pVelocity;//Set the velocity
        //The bounds and center variables are set when the Position is set through the property
        //The internal mHalfWidth and mHalfHeight variables are set when the Texture is set through the property
    }

#endregion

}
}
