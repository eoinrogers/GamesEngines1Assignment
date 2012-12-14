/*
 * DT228/4 -- Games Engines 1
 * Assignment by Eoin Rogers (C09525386)
 * 3D Visualisation of a Brainfuck Interpreter
 * Those not familier with the brainfuck programming language should see http://en.wikipedia.org/wiki/Brainfuck 
 * */

using System;

namespace BFVisualiser
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args) {
            using (XNAGame game = new XNAGame())
            {
                game.Run();
                Console.WriteLine("finished");
            }
        }
    }
#endif
}

