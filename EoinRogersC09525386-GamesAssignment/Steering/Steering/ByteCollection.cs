using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BFVisualiser
{
    class ByteCollection
    {
        /*
         * Stores all the bytes currently being used: i.e. this class represents the interpreter's memory. 
         * */
        public string code; // Stores the source code to be executed 

        List<Byte> memory; // Memory itself (a list of bytes) 
        int len;
        public int index // Index of the source code instruction to be executed (i.e. this is a program counter) 
        {
            get;
            set;
        }

        public int ptr // Memory pointer 
        {
            get;
            set;
        }

        public ByteCollection(string pathToSourceCode, int length)
        {
            // Read the source code from a file 
            StreamReader sr = new StreamReader(pathToSourceCode);
            code = sr.ReadToEnd();

            // Construct memory 
            memory = new List<Byte>();
            for (int i = 0; i < length; i++)
            {
                memory.Add(new Byte(0));
            }

            // Initialise 
            ptr = index = 0;
            len = length;
        }

        public void FilterCode()
        {
            char[] valid = { '+', '-', '>', '<', '.', ',', '[', ']' };
            string newcode = "";
            foreach (char c in code)
            {
                if (valid.Contains(c)) newcode += c;
            }
            code = newcode;
        }

        public Byte Get(int index) { return memory[index]; }

        public void IncrementPointer() { ptr = (((ptr + 1) % len) + len) % len; }
        public void DecrementPointer() { ptr = (((ptr - 1) % len) + len) % len; }

        public void ReplaceByte(int value)
        {
            memory[ptr] = new Byte(value);
        }

        public void ReplaceDirect(Byte newByte) { memory[ptr] = newByte; }

        public int Size() { return memory.Count; }
    }
}
