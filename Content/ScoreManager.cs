using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace P1_Monogame.Content
{
    public class ScoreManager
    {
        private static string _fileName = "score.xml";

        public List<Score> Highscores { get; private set; }
        public List<Score> Scores { get; private set; }

        public ScoreManager()
       : this(new List<Score>())
        {

        }

        public ScoreManager(List<Score> scores)
        {
            Scores = scores;

            UpdareHighscores();
        }


        public void Add(Score score)
        {
            Scores.Add(score);

            Scores = Scores.OrderByDescending(c => c.Value).ToList();
        }

        public static ScoreManager Load()
        {
            if (!File.Exists(_fileName))
                return new ScoreManager();

            using (var reader = new StreamReader(new FileStream(_fileName, FileMode.Open)))
            {
                var serilizer = new XmlSerializer(typeof(List<Score>));

                var scores = (List<Score>)serilizer.Deserialize(reader);

                return new ScoreManager(scores);
            }
        }

        private void UpdareHighscores()
        {
            Highscores = Scores.Take(5).ToList();
        }

        public static void Save(ScoreManager scoreManager)
        {
            // Overrides the file if it alreadt exists
            using (StreamWriter writer = new StreamWriter(new FileStream(_fileName, FileMode.Create)))
            {
                XmlSerializer serilizer = new XmlSerializer(typeof(List<Score>));

                serilizer.Serialize(writer, scoreManager.Scores);
            }
        }
    }
}
