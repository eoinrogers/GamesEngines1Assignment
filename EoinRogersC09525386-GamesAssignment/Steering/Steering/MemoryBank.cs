using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BFVisualiser
{
    class MemoryBank : GameEntity
    {
        /*
         * Show the memory state to the user as rows of boxes
         * */

        // Colours for zeros and ones 
        public static Vector3 ZERO = new Vector3(255, 0, 0);
        public static Vector3 ONE = new Vector3(0, 255, 0);
        static int BYTE_SIZE = 8; // A byte is 8 bits! 

        List<BankRow> rows;
        public readonly float Length, ByteDistance;
        public readonly int NumberOfBytes;

        public class BankRow
        {
            /*
             * A single row of blocks (show a byte to the user) 
             * */
            BepuEntity[] blocks;
            public BankRow(Vector3 firstBlock, float distance, int numberOfBits)
            {
                blocks = new BepuEntity[numberOfBits];
                float addOn = 0;

                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i] = XNAGame.Instance.createBox(firstBlock + new Vector3(firstBlock.X, firstBlock.Y, firstBlock.Z + addOn), 5, 5, 5);
                    blocks[i].body.BecomeKinematic();
                    blocks[i].LoadContent();
                    addOn -= distance;
                    blocks[i].diffuse = ZERO;
                }
            }

            public void setBitToZero(int index)
            {
                blocks[index].diffuse = ZERO; // Set the bit at index to zero 
            }

            public void setBitToOne(int index)
            {
                blocks[index].diffuse = ONE; // Set the byte at index to one 
            }

            public void Display(GameTime gt)
            {
                // Display the blocks 
                for (int i = 0; i < blocks.Length; i++)
                {
                    blocks[i].Draw(gt);
                }
            }
        }

        public MemoryBank(int bytes, Vector3 position, float distance)
        {
            // Create a new MemoryBank 
            rows = new List<BankRow>();
            Position = position;
            ByteDistance = distance;
            NumberOfBytes = bytes;
            Length = 0;
            for (int i = 0; i < bytes; i++)
            {
                rows.Add(new BankRow(position, distance, BYTE_SIZE));
                position = new Vector3(position.X - distance, position.Y, position.Z);
                Length += distance;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Display each row one by one 
            for (int i = 0; i < rows.Count; i++)
            {
                DepthStencilState state = new DepthStencilState();
                state.DepthBufferEnable = true;
                XNAGame.Instance.GraphicsDevice.DepthStencilState = state;
                rows[i].Display(gameTime);
            }
        }

        public void SetByte(int index, bool[] values)
        {
            // Set a byte at the specified index to the specified value 
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i])
                {
                    rows[index].setBitToOne(i);
                }
                else
                {
                    rows[index].setBitToZero(i);
                }
            }
        }

        public override void LoadContent()
        {
            //base.LoadContent();
        }
    }
}
