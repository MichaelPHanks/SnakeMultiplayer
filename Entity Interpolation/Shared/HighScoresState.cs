using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{

    [DataContract(Name = "HighScores")]

    public class HighScoresState
    {
        /// <summary>
        /// Have to have a default constructor for the XmlSerializer.Deserialize method
        /// </summary>
        public HighScoresState()
        {
        }

        /// <summary>
        /// Overloaded constructor used to create an object for long term storage
        /// </summary>
        /// <param name="score"></param>
        /// <param name="level"></param>
        public HighScoresState(List<Tuple<int, DateTime>> highScores)
        {
            
            this.HighScores = highScores;


        }

        public void addHighScore(Tuple<int, DateTime> score)
        {
            HighScores.Add(score);
            HighScores = HighScores.OrderBy(tuple => tuple.Item1).ToList();

            if (HighScores.Count > 5)
            {
                HighScores.RemoveRange(5, HighScores.Count - 5);
            }
        }

        public List<Tuple<int,DateTime>> getHighScores()
        {
            return HighScores;
        }
        [DataMember()]
        List<Tuple<int, DateTime>> HighScores;

        
       
    }
}
