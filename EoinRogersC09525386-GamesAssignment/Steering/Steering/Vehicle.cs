using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BEPUphysics.Constraints.TwoEntity.Motors;
using BEPUphysics.Constraints.SolverGroups;
using BEPUphysics.Entities.Prefabs;
using Microsoft.Xna.Framework.Input;

namespace BFVisualiser
{
    class Vehicle : GameEntity
    {
        /*
         * Represents the moving vehicle
         * */

        BepuEntity chassis, wand; // The read/write head thing is called the wand 
        PrismaticJoint slider; // Moves the vehicle along the memory bank 
        RevoluteJoint wandHinge; // Moves the wand 
        Box sliderBase; // Invisibe box for the slider to attatch to 
        float byteDist; // Distance between bytes (needed to calculate movements) 
        int bytes, delay;
        bool wandGoal, sliderGoal;

        public Vehicle(MemoryBank memBank, float distance)
        {
            // Figure out the positions of the vehicle and it's sliderBase 
            Vector3 position = new Vector3(memBank.Position.X, memBank.Position.Y, memBank.Position.Z + 10);
            Vector3 motorPos = new Vector3(position.X - memBank.Length - distance, position.Y, position.Z);
            byteDist = memBank.ByteDistance;
            bytes = memBank.NumberOfBytes;

            wandGoal = sliderGoal = false;
            delay = 0;

            // Build the main vechicle 
            chassis = XNAGame.Instance.createVehicle(position);

            // Build the slider, and attatch 
            sliderBase = new Box(motorPos, 1, 1, 1);
            slider = new PrismaticJoint(sliderBase, chassis.body, sliderBase.Position, Vector3.UnitX, chassis.body.Position);
            XNAGame.Instance.space.Add(slider);
            XNAGame.Instance.space.Add(sliderBase);

            // Change slider settings 
            slider.Motor.Settings.Mode = MotorMode.Servomechanism;
            slider.Motor.Settings.Servo.SpringSettings.StiffnessConstant /= 20;
            slider.Motor.Settings.Servo.MaxCorrectiveVelocity = 200;
            slider.Motor.Settings.Servo.BaseCorrectiveSpeed = 120;

            // Create the wand 
            setUpWand(position);
        }

        private void setUpWand(Vector3 position)
        {
            // Make a wand 
            wand = XNAGame.Instance.createBox(new Vector3(position.X, position.Y + 8, position.Z), 2, 8, 2);

            // Make the wand hinge (it's joint with the main vehicle) 
            wandHinge = new RevoluteJoint(chassis.body, wand.body, chassis.body.Position, Vector3.UnitX);
            wandHinge.Limit.IsActive = true;
            wandHinge.Limit.MaximumAngle = MathHelper.ToRadians(45.0f);
            wandHinge.Limit.MinimumAngle = MathHelper.ToRadians(-46.0f);
            wandHinge.Motor.Settings.Mode = MotorMode.Servomechanism;
            wandHinge.Motor.Settings.Servo.BaseCorrectiveSpeed = 120;
            wandHinge.Motor.Settings.MaximumForce = 200;
            wandHinge.Motor.Settings.Servo.SpringSettings.StiffnessConstant /= 20;

            // Add to the space 
            XNAGame.Instance.space.Add(wandHinge);
        }

        public void FlipFrom()
        {
            wandHinge.Motor.Settings.Mode = MotorMode.Servomechanism;
            
            wandHinge.Motor.Settings.Servo.Goal = MathHelper.ToRadians(45);
            wandHinge.Motor.IsActive = true;

            wandGoal = true;
            delay = 0;
        }

        public void FlipTo()
        {
            // Point the wand at the memory bank 

            // The motor should act in servo mode 
            wandHinge.Motor.Settings.Mode = MotorMode.Servomechanism;
            
            // Tell the motor to move 
            wandHinge.Motor.Settings.Servo.Goal = MathHelper.ToRadians(-45.0f);
            wandHinge.Motor.IsActive = true;

            // We need this to keep track of the vehicles movement 
            wandGoal = true;
            delay = 0; // Delay fixes what I think is a minor bug in BEPU: it takes a while to update the goal... 
        }

        public void MoveTo(int index)
        {
            // Move to a given location (index will be a byte index; this will move to that byte) 

            // Figure out the destionation 
            float destination = (float)bytes * byteDist + byteDist;
            destination -= 16f * index;

            // Move to it 
            slider.Motor.Settings.Servo.Goal = destination;
            slider.Motor.IsActive = true;

            sliderGoal = true;
            delay = 0;
        }

        public void StopMotors()
        {
            // Stop all motors 
            slider.Motor.IsActive = false;
            wandHinge.Motor.IsActive = false;
        }

        public bool IsWorking()
        {
            // Returns true if the vehicle is moving, false otherwise 
            return sliderGoal || wandGoal;
        }

        public override void LoadContent()
        {
            //base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            
        }

        public bool floatEquality(float x, float y, Nullable<float> epsilon)
        {
            // Epsilon method for floats 
            if (epsilon == null) epsilon = 0.005f;
            if (Math.Abs(x - y) <= epsilon) return true;
            return false;
        }

        public override void Update(GameTime gameTime)
        {
            // Delay seems to fix a BEPUPhysics problem (see the wand movement methods) 
            if (delay < 3)
            {
                delay++;
                return;
            }
            if (sliderGoal && floatEquality(slider.Motor.Error, 0, 0.6f))
            {
                sliderGoal = false; // If we are at the sliders goal, it no longer has to move 
            }
            else if (wandGoal && floatEquality(wandHinge.Motor.Error, 0, null))
            {
                wandGoal = false; // If we are at the wands goal it no longer has to move 
            }
            
            base.Update(gameTime);
        }
    }
}
