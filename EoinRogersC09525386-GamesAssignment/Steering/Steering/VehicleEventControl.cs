using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFVisualiser
{
    class VehicleEventControl
    {
        /*
         * Use to control the vehicles motion and states at a low level 
         * */

        // Vechicle states and primitive motions 
        public static int FLIP_TO = 1; // Vehicle wants to flip it's wand to the memory bank 
        public static int FLIP_FROM = 2; // Vehicle wants to flip it's wand away from the memory bank 
        public static int MOVE = 3; // Vehicle wants to move along the memory bank 
        public static int NONE = 4; // Vehicle is finished it's actions for this instruction; the interpreter is free to execute the next instruction 
        public static int WAITING = 5; // Vehicle is busy, and the program must wait for it to finish what it's doing 
        public static int FINISHED = 6; // The vehicle is finished all of it's current instructions 

        List<int> events;
        public readonly Vehicle vehicle;
        int index;

        public VehicleEventControl(string source, Vehicle vehicle)
        {
            // Create a vehicle event control 
            this.vehicle = vehicle;
            index = 0;

            events = new List<int>();

            // Calculate a list of the events the vehicle will have to run 
            addEvents(source);
        }

        private void addEvents(string remaining)
        {
            // Add events 
            foreach (char c in remaining)
            {
                if (c == '+' || c == '-' || c == '.' || c == ',') // For these instructions, flip to the memory bank 
                {
                    events.Add(FLIP_TO);
                    events.Add(NONE);
                }
                else if (c == '>' || c == '<') // For these instructions, flip from the memory bank and move 
                {
                    events.Add(FLIP_FROM);
                    events.Add(MOVE);
                }
            }
            events.Add(FINISHED);
        }

        public void MoveVehicle(ByteCollection coll) { vehicle.MoveTo(coll.ptr); } // Move the vehicle to the current pointer location 
        public void MoveWandTo() { vehicle.FlipTo(); } // Move wand to memory bank 
        public void MoveWandFrom() { vehicle.FlipFrom(); } // Move wand from memory bank 

        public int NextEvent()
        {
            // Get the next event for from the vehicle 
            if (index >= events.Count) return NONE;
            if (vehicle.IsWorking()) return WAITING;
            return events[index++];
        }
    }
}
