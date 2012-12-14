using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BFVisualiser
{
    class Controller : IOutputCallback
    {
        /*
         * Controls the program at a high level, forcing the interpreter and the VechicleEventControl to take turns
         * */

        MemoryBank mb;
        Interpreter interpreter;
        ByteCollection collection;
        VehicleEventControl vec;

        public Controller(Vehicle v, MemoryBank mb, string pathToSourceCode, string pathToInputs)
        {
            // Create a ByteCollection 
            collection = new ByteCollection(pathToSourceCode, 300);
            collection.FilterCode();

            // Create an Interpreter 
            interpreter = new Interpreter(pathToInputs, this);
            interpreter.PrepareToRun(collection);

            this.mb = mb;

            // Create a VehicleEventControl 
            vec = new VehicleEventControl(interpreter.ExtractBlock(collection), v);
        }

        public void Update()
        {
            // Main control method for the entire program! 
            int action = vec.NextEvent(); // Find out what the vehicle has to do next 

            if (action == VehicleEventControl.FINISHED)
            {
                // If the vechicle has no more primative actions to carry out, update the interpreter and read the next block of source code 
                UpdateInterpreter();
                string nextBlock = interpreter.ExtractBlock(collection);
                
                // If we are at the end of the code, but the program into finished mode (it keeps running, but will not do anything) 
                if (collection.index >= collection.code.Length - 1)
                {
                    vec.MoveWandFrom();
                    XNAGame.Instance.Finish();
                }

                // Create a new VehicleEventControl to execute the next set of primitive vehicle actions 
                vec = new VehicleEventControl(nextBlock, vec.vehicle);
            }
            else if (action == VehicleEventControl.WAITING) return; // If we are waiting for the vehicle as it moves, exit straight away 
            if (action == VehicleEventControl.MOVE)
            {
                // If the vehicle has to move (i.e. increment/decrement pointer), let the interpreter work first 
                UpdateInterpreter();
                vec.MoveVehicle(collection);
            }
            else if (action == VehicleEventControl.FLIP_TO)
            {
                vec.MoveWandTo(); // Move wand towards a byte 
            }
            else if (action == VehicleEventControl.FLIP_FROM)
            {
                vec.MoveWandFrom(); // Move wand away from a byte 
            }
            else if (action == VehicleEventControl.NONE)
            {
                // Otherwise, run the interpreter and update the MemoryBank 
                UpdateInterpreter();
                mb.SetByte(collection.ptr, collection.Get(collection.ptr).ReadAsArray());
            }
        }

        public void UpdateInterpreter()
        {
            // Run a single cycle on the interpreter (i.e. execute a single instruction) 
            if (collection.index >= collection.code.Length) return;
            if (interpreter.RunNext(collection))
            {
                vec.MoveWandFrom();
                XNAGame.Instance.Finish();
            }
        }

        public override void PassCycleResult(CycleResult result)
        {
            // Used to read values from the interpreter (i.e. output from the program) 
            if (result.output != '\0') XNAGame.Instance.AddOutput(result.output);
        }
    }
}
