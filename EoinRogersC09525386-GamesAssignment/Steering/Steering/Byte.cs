using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFVisualiser
{
    class Byte
    {
        /*
         * Represents a single bytes held in the interpreter's memory. 
         * N.B. This does NOT handle the memory you see on the screen: memory visualisation is handled by MemoryBank
         * */
        private static int BYTE_SIZE = 8; // A byte is 8 bits 

        public class Bit
        {
            /*
             * Represents a single bit in the memory. A byte is thus just a collection of bits. 
             * This may seem like an inefficient way to implement the system, and it is, but due to the fact that the interpreter will be made run slowly anyway, this is not a problem. 
             * The Adder is implemented in this class, the RippleAdder in the main Byte class
             * */
            bool value; // The value itself 
            public Bit(bool value)
            {
                this.value = value;
            }

            private bool xor(bool x, bool y)
            {
                // XOR function for the adder 
                if (x && !y) return true;
                else if (!x && y) return true;
                return false;
            }

            public bool Value() { return value; } // Getter for the value 

            public void Flip() { value = !value; } // Flip the bit 

            public bool Adder(Bit bit, bool carry)
            {
                // Impementation of the adder circuit shown at: http://upload.wikimedia.org/wikipedia/commons/a/aa/Full_Adder.svg 
                bool oldval = value; // Backup the old value, since we will need to access it later 
                
                value = xor(xor(value, bit.value), carry);
                return (xor(oldval, bit.value) && carry) || (oldval && bit.value);
            }
        }

        Bit[] bits; // All the bits for the byte will be stored here 

        public Byte(int defaultValue)
        {
            // Convert the data to a binary string 
            string data = Convert.ToString(defaultValue, 2);
            data = pad(data) + data;
            if (data.Length != BYTE_SIZE) throw new Exception("Data incorrect length (should be 8 bits, i.e. in the range 0 - 255)...");
            char[] num = data.ToCharArray();
            Array.Reverse(num);
            data = new string(num);

            // Read the string and convert into an array of Bit objects 
            bits = new Bit[BYTE_SIZE];
            for (int i = 0; i < BYTE_SIZE; i++)
            {
                if (data[i] == '1') bits[i] = new Bit(true);
                else bits[i] = new Bit(false);
            }
        }

        private string pad(string n) {
            // Byte padding for the conversion to binary 
            int num = BYTE_SIZE - n.Length;
            string output = "";
            for (int i = 0; i < num; i++) {
                output += "0";
            }
            return output;
        }

        public int Size() { return bits.Length; } // Get teh number of bits in the array 

        public Bit GetBit(int index) { return bits[index]; } // Access a specific bit 

        private void flipAll()
        {
            // Flip all bits (i.e. every 1 becomes a 0 and vice versa). Needed for Two's Complement. 
            foreach (Bit bit in bits)
            {
                bit.Flip();
            }
        }

        public bool RippleAdd(Byte by)
        {
            // Implementation of a Ripple Adder circuit, which consists of a number of adders joined together. 
            // Used both to increment and decrement 
            if (by.Size() != Size()) throw new Exception("Can't add bytes of different sizes...");
            bool carry = false;
            for (int i = 0; i < bits.Length; i++)
            {
                carry = bits[i].Adder(by.GetBit(i), carry);
            }
            return carry;
        }

        public void Increment()
        {
            RippleAdd(new Byte(1)); // Increase the size of the byte by 1 (note that this does not check for overflows!) 
        }

        public void TwosComplement()
        {
            // Apply Two's complement to the byte (i.e. flip the bits and add one) 
            flipAll();
            Increment();
        }

        public void Decrement()
        {
            // Reduce the size of the byte by 1 (note that this does not check for overflows!) 
            Byte b = new Byte(1);
            b.TwosComplement();
            RippleAdd(b);
        }

        public int Read()
        {
            // Read the byte as an integer (only used during testing) 
            int output = 0;
            int n = 1;
            foreach (Bit b in bits)
            {
                if (b.Value()) output += n;
                n *= 2;
            }
            return output;
        }

        public char ReadToStdOut()
        {
            // Read the byte as a character 
            return Convert.ToChar(Read());
        }

        public bool[] ReadAsArray()
        {
            // Read the byte as an array (this is how the Byte class interacts with the MemoryBank) 
            bool [] output = new bool[Size()];
            for (int i = 0; i < Size(); i++)
            {
                output[i] = GetBit(i).Value();
            }
            return output;
        }
    }
}
