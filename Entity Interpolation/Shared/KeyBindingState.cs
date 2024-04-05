using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shared
{
    /// <summary>
    /// This class demonstrates how to create an object that can be serialized
    /// under the XNA framework.
    /// </summary>
    //[Serializable]
    [DataContract(Name = "KeyControls")]
    public class KeyControls
    {
        /// <summary>
        /// Have to have a default constructor for the XmlSerializer.Deserialize method
        /// </summary>
        public KeyControls() {
        }

        /// <summary>
        /// Overloaded constructor used to create an object for long term storage
        /// </summary>
        /// <param name="score"></param>
        /// <param name="level"></param>
        public KeyControls(Keys left, Keys right, Keys up, Keys down)
        {
            this.Up = up;
            this.Left = left;
            this.Right = right;
            this.Down = down;
            this.TimeStamp = DateTime.Now;

           
        }

        [DataMember()]
        public Keys Up { get; set; }
        [DataMember()]
        public Keys Down { get; set; }
        [DataMember()]
        public Keys Left { get; set; }
        [DataMember()]
        public Keys Right { get; set; }
        [DataMember()]
        public DateTime TimeStamp { get; set; }
        
    }
}
