using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Components
{
    public class TurnPoints : Component
    {

        public TurnPoints(Queue<Tuple<Vector2, float>> turnPoints)
        {
            this.turnPoints = turnPoints;
        }

        public Queue<Tuple<Vector2, float>> turnPoints { get; set; }
    }
}
