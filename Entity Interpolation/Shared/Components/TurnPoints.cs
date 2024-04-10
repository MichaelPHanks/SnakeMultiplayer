using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Components
{
    public class TurnPoints : Component
    {
        public TurnPoints(Queue<Tuple<int, int>> turnPoints)
        {
            this.turnPoints = turnPoints;
        }
        public Queue<Tuple<int,int>> turnPoints { get; private set; }
    }
}
