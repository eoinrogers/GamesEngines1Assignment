using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFVisualiser
{
    abstract class IOutputCallback
    {
        /*
         * Read the output from the interpreter 
         * */
        public class CycleResult
        {
            /*
             * Pass data from the interpreter
             * */
            public CycleResult(char instruction, char output, int pointer, int index)
            {
                this.instruction = instruction;
                this.output = output;
                this.pointer = pointer;
                this.index = index;
            }
            public char instruction;
            public char output;
            public int index;
            public int pointer;
        }

        public abstract void PassCycleResult(CycleResult result); // Get data from the interpreter 
    }
}
