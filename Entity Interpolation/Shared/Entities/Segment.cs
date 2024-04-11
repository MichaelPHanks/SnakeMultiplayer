using Microsoft.Xna.Framework;
using Shared.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Entities
{
    public class Segment
    {
        public static Entity create(string texture, Vector2 position, float size, float moveRate, float rotateRate, Queue<Tuple<Vector2, float>> turnPoints, float orientation, uint headId )
        {
            Entity entity = new Entity();

            entity.add(new Appearance(texture));
            entity.add(new Shared.Components.Segment(headId));

            entity.add(new Position(position, orientation));
            entity.add(new Size(new Vector2(size, size)));
            entity.add(new Movement(moveRate, rotateRate));
            entity.add(new TurnPoints(turnPoints));
            entity.add(new Collision());
            

            return entity;
        }
    }
}
