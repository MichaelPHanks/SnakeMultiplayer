using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Components
{
    public class Name : Component
    {

        public Name(string name)
        {
            this.name = name;
        }

        public string name { get; private set; }

    }
}
