using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    [DataContract(Name = "PlayerName")]

    public class PlayerNameState
    {
        /// <summary>
        /// Have to have a default constructor for the XmlSerializer.Deserialize method
        /// </summary>
        public PlayerNameState()
        {
        }

        /// <summary>
        /// Overloaded constructor used to create an object for long term storage
        /// </summary>
        /// <param name="score"></param>
        /// <param name="level"></param>
        public PlayerNameState(string playerName)
        {

            this.PlayerName = playerName;


        }

        

        public string getPlayerName()
        {
            return PlayerName;
        }
        
        [DataMember()]
        string PlayerName;


    }
}
