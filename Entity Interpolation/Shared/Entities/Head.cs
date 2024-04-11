using Shared.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class Head
    {
        public static Entity create(string texture, Vector2 position, float size, float moveRate, float rotateRate)
        {
            Entity entity = new Entity();

            entity.add(new Appearance(texture));
            entity.add(new Shared.Components.Head());

            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));
            entity.add(new Movement(moveRate, rotateRate));

            List<Input.Type> inputs = new List<Input.Type>();
            inputs.Add(Input.Type.RotateDown);
            inputs.Add(Input.Type.RotateUp);
            inputs.Add(Input.Type.RotateLeft);
            inputs.Add(Input.Type.RotateRight);
            entity.add(new Input(inputs));

            return entity;
        }
    }

    public class Utility
    {
        public static void rotateUp(Entity entity, TimeSpan elapsedTime)
        {

            // Tell the segments and the tail about this 'turn point'.
            var position = entity.get<Position>();




            float targetOrientation = (float)Math.Atan2(-1, 0); // Calculate the target orientation
            position.orientation = targetOrientation;
        }

        public static void rotateLeft(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();


            float targetOrientation = (float)Math.Atan2(0, -1); // Calculate the target orientation
            position.orientation = targetOrientation;
        }
        public static void rotateDown(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();


            float targetOrientation = (float)Math.Atan2(1, 0); // Calculate the target orientation
            position.orientation = targetOrientation;
        }


        /*public static void mouseRotation(Entity entity, TimeSpan elapsedTime)
        {
            // Given the location we are looking at, change the orientation.
            var position = entity.get<Position>();

            // Calculate the angle between the ship's current orientation and the direction of the mouse pointer
            float deltaX = mouseX - position.position.X; // Calculate the change in X
            float deltaY = mouseY - position.position.Y; // Calculate the change in Y
            float targetOrientation = (float)Math.Atan2(deltaY, deltaX); // Calculate the target orientation
            float rotationSpeed = 5f; // Adjust this value to control the rotation speed
            float deltaOrientation = targetOrientation - position.orientation;
            if (Math.Abs(deltaOrientation) > Math.PI) // Check if it's shorter to rotate the other way around
            {
                if (deltaOrientation > 0)
                    deltaOrientation -= 2 * (float)Math.PI;
                else
                    deltaOrientation += 2 * (float)Math.PI;
            }
            float rotationAmount = rotationSpeed * (float)elapsedTime.TotalSeconds;
            if (Math.Abs(deltaOrientation) < rotationAmount)
                position.orientation = targetOrientation;
            else
                position.orientation += Math.Sign(deltaOrientation) * rotationAmount;

            // Normalize orientation angle to be within [0, 2 * PI]
            if (position.orientation < 0)
                position.orientation += 2 * (float)Math.PI;
            else if (position.orientation >= 2 * Math.PI)
                position.orientation -= 2 * (float)Math.PI;

            // Now, you can use vectorX and vectorY to determine the direction the ship is facing
            float vectorX = (float)Math.Cos(position.orientation);
            float vectorY = (float)Math.Sin(position.orientation);

            


        }*/

        public static void rotateRight(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();


            float targetOrientation = (float)Math.Atan2(0, 1); // Calculate the target orientation
            position.orientation = targetOrientation;
        }
        public static void thrust(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            var vectorX = Math.Cos(position.orientation);
            var vectorY = Math.Sin(position.orientation);

            position.position = new Vector2(
                (float)(position.position.X + vectorX * movement.moveRate * elapsedTime.Milliseconds),
                (float)(position.position.Y + vectorY * movement.moveRate * elapsedTime.Milliseconds));
        }
    }
}
