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
using System.IO;
using System.Threading;

namespace BFVisualiser
{
    public class ProgramSelector : DrawableGameComponent
    {
        /*
         * Menu that the user sees at startup
         * */
        static int DOWNCONST = 20; // Each entry in the menu will be 20 pixels below the last entry 

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont menu;
        string[] files;
        List<string> bfFiles;
        List<Vector2> positions;
        List<int> width;
        int selected, toBeRun;

        public ProgramSelector(Game g, SpriteFont sf) : base(g)
        {
            this.menu = sf;
        }

        public override void Initialize()
        {
            // Read a list of all files in the current directory 
            files = Directory.GetFiles(".");

            bfFiles = new List<string>();
            positions = new List<Vector2>();
            width = new List<int>();

            bool mustMakeInput = true;
            int startX = 19, startY = 30, increment = DOWNCONST;

            // Iterate through each file 
            foreach (string file in files)
            {
                if (file.EndsWith(".bf")) // If it ends with .bf, add it the list of files to show 
                {
                    bfFiles.Add(file);
                    positions.Add(new Vector2(startX, startY));
                    width.Add(file.Length * 16);
                    startY += increment;
                }
                else if (file == ".\\input.txt") mustMakeInput = false; // You will not need to make an input file if there is one already 
            }

            // If an input file does not exist, make one (we need an input file) 
            if (mustMakeInput)
            {
                StreamWriter sw = new StreamWriter("input.txt");
                sw.Write("");
                sw.Close();
            }

            selected = toBeRun = -1; // Intialise these to -1; i.e. no program has been selected, and none is to be run 

            base.Initialize();
        }

        public void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        public string ToBeRun()
        {
            // Return the name of the program the user has selected to run. If they have not selected one yet, return null. 
            if (toBeRun > -1) return bfFiles[toBeRun];
            return null;
        }

        public override void Update(GameTime gameTime)
        {

            MouseState ms = Mouse.GetState();
            Vector2 otherside;
            selected = -1; // Reset selected (the user may have moved the mouse away from the menu item they had just moused over, after all 

            // See if any menu item has been moused over. If so, store it's index in selected, so it can be drawn red. 
            for (int i = 0; i < bfFiles.Count; i++)
            {
                otherside = new Vector2(positions[i].X + width[i], positions[i].Y + DOWNCONST);
                if (ms.X < otherside.X && ms.Y < otherside.Y && ms.X > positions[i].X && ms.Y > positions[i].Y) selected = i;
            }

            if (selected > -1 && ms.LeftButton == ButtonState.Pressed)
            {
                toBeRun = selected; // If the user has selected a menu item and is clicking on it, this means we have to run it 
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // Show the title at the top 
            spriteBatch.DrawString(menu, "Please select a program to run: ", new Vector2(8, 8), Color.Orchid);
            
            // Display each menu item 
            for (int i = 0; i < bfFiles.Count; i++)
            {
                if (i == selected) spriteBatch.DrawString(menu, bfFiles[i], positions[i], Color.Red); // Red if moused over
                else spriteBatch.DrawString(menu, bfFiles[i], positions[i], Color.White); // White otherwise 
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
