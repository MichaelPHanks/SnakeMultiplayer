using Shared.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Shared.Components;
using System.Threading;

namespace Shared.Entities
{
    public class Food
    {
        public static Entity create(string texture, Vector2 position, float size)
        {
            Entity entity = new Entity();

            entity.add(new Appearance(texture));

            entity.add(new Position(position));
            entity.add(new Size(new Vector2(size, size)));

            

            return entity;
        }
    }
}
