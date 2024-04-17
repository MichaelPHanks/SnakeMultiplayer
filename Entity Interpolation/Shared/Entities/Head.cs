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
        public static Entity create(string texture,string name, Vector2 position, float size, float moveRate, float rotateRate)
        {
            Entity entity = new Entity();

            entity.add(new Appearance(texture));
            entity.add(new Name(name));
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
        public static bool rotateUp(Entity entity, TimeSpan elapsedTime)
        {

            // Tell the segments and the tail about this 'turn point'.
            var position = entity.get<Position>();




            float targetOrientation = (float)Math.Atan2(-1, 0); // Calculate the target orientation


            if (position.orientation != targetOrientation)
            {
                position.orientation = targetOrientation;
                return true;
            }
            return false;
        }

        public static bool rotateLeft(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();


            float targetOrientation = (float)Math.Atan2(0, -1); // Calculate the target orientation
            if (position.orientation != targetOrientation)
            {
                position.orientation = targetOrientation;
                return true;
            }
            return false;
        }
        public static bool rotateDown(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();


            float targetOrientation = (float)Math.Atan2(1, 0); // Calculate the target orientation
            if (position.orientation != targetOrientation)
            {
                position.orientation = targetOrientation;
                return true;
            }
            return false;
        }

        public static bool rotateRight(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();


            float targetOrientation = (float)Math.Atan2(0, 1); // Calculate the target orientation
            if (position.orientation != targetOrientation)
            {
                position.orientation = targetOrientation;
                return true;
            }
            return false;
        }
        public static void thrust(Entity entity, TimeSpan elapsedTime)
        {
            var position = entity.get<Position>();
            var movement = entity.get<Movement>();

            var vectorX = Math.Cos(position.orientation);
            var vectorY = Math.Sin(position.orientation);

            position.position = new Vector2(
                (float)(position.position.X + vectorX * movement.moveRate * elapsedTime.TotalMilliseconds),
                (float)(position.position.Y + vectorY * movement.moveRate * elapsedTime.TotalMilliseconds));
        }
    }
}
