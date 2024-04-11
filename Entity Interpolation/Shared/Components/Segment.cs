using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Components
{
    public class Segment : Component
    {
        public Segment(uint id) 
        {
            this.headId = id;
        }

        public uint headId {  get; private set; }    
    }
}
