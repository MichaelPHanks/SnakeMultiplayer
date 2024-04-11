using Shared.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Shared.Entities
{
    public class Tail
    {
        public static Entity create(string texture, Vector2 position, float size, float moveRate, float rotateRate, Queue<Tuple<Vector2, float>> turnPoints)
        {
            Entity entity = new Entity();

            entity.add(new Appearance(texture));
            entity.add(new Shared.Components.Tail());

            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));
            entity.add(new Movement(moveRate, rotateRate));
            entity.add(new TurnPoints(turnPoints));
            entity.add(new Collision());


            return entity;
        }
    }
}
