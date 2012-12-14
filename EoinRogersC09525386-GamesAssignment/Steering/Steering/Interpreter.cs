using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BFVisualiser
{
    class Interpreter
    {
        /*
         * Executes the supplied brainfuck code 
         * */
        string input; // Input text to the program 
        int inIdx, loop; // Input index and loop counter 
        IOutputCallback callback; // Tells the interpreter where to send the output (will be an instance of Controller) 

        public Interpreter(string pathToInput, IOutputCallback callback)
        {
            this.callback = callback;
            inIdx = 0;

            // Read the input from a file 
            StreamReader sr = new StreamReader(pathToInput);
            input = sr.ReadToEnd();
        }

        public void PrepareToRun(ByteCollection coll) {
            // Initialise the counters in anticipation of running a program 
            loop = 0;
            coll.index = 0;
        }

        public string ExtractBlock(ByteCollection coll)
        {
            // Read out a block of code that the VehicleEventController can determine vehicle control primitives from 
            string output = "";
            foreach (char c in coll.code.Substring(coll.index))
            {
                if (c == '[' || c == ']') return output;
                output += c;
            }
            return output;
        }

        public bool RunNext(ByteCollection coll) {

            // Execute the next instruction 
            if (coll.index < coll.code.Length) // Only if there is an instruction to execute! 
            {
                char instruction = coll.code[coll.index]; // Read the instruction 

                char o = '\0';
                if (instruction == '+')
                {
                    coll.Get(coll.ptr).Increment(); // Increment memory contents 
                }
                else if (instruction == '-')
                {
                    coll.Get(coll.ptr).Decrement(); // Decrement memory contents 
                }
                else if (instruction == '>')
                {
                    coll.IncrementPointer(); // Increment pointer 
                }
                else if (instruction == '<')
                {
                    coll.DecrementPointer(); // Decrement pointer 
                }
                else if (instruction == '[' && coll.Get(coll.ptr).Read() != 0) loop += 1; // Enter a loop 
                else if (instruction == '[' && coll.Get(coll.ptr).Read() == 0) loop = forwardTrack(coll, loop); // Exit a loop 
                else if (instruction == ']') loop = backTrack(coll, loop); // Go back to the start of a loop 
                else if (instruction == '.') o = coll.Get(coll.ptr).ReadToStdOut(); // Write output to the Controller 
                else if (instruction == ',') insertInput(coll); // Read input from the input string 

                // Pass the output to the Controller 
                if (callback != null) callback.PassCycleResult(new IOutputCallback.CycleResult(instruction, o, coll.ptr, coll.index));

                coll.index += 1; // Increment the program counter 
                return false;
            }
            else { return true; }
        }

        private int forwardTrack(ByteCollection coll, int loop)
        {
            // Go to the end of a loop 
            int goal = loop;
            while (true)
            {
                coll.index += 1;
                if (coll.code[coll.index] == '[') loop += 1;
                else if (coll.code[coll.index] == ']' && loop == goal) return loop - 1;
                else if (coll.code[coll.index] == ']') loop -= 1;
            }
        }

        private int backTrack(ByteCollection coll, int loop)
        {
            // Go to the start of a loop 
            int goal = loop - 1;
            while (loop != goal)
            {
                coll.index -= 1;
                if (coll.code[coll.index] == ']') loop += 1;
                else if (coll.code[coll.index] == '[') loop -= 1;
            }
            coll.index -= 1;
            return loop;
        }

        private void insertInput(ByteCollection coll)
        {
            // Read input from the input string 
            if (inIdx < input.Length)
            {
                coll.ReplaceByte((int)input[inIdx]);
                inIdx++;
            }
        }
    }
}
