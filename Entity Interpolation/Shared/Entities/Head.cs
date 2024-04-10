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
}
