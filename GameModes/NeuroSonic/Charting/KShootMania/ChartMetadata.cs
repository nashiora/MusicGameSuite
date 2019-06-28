using System;
using System.IO;

namespace NeuroSonic.Charting.KShootMania
{
    public enum Difficulty
    {
        Novice,
        Challenge,
        Extended,
        Infinite,
    }

    public sealed class ChartMetadata
    {
        public static ChartMetadata Create(StreamReader reader)
        {
            var meta = new ChartMetadata();

            string line;
            while ((line = reader.ReadLine()) != Chart.SEP && line != null)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                line = line.Trim();

                if (line.TrySplit('=', out string key, out string value))
                    meta.Set(key.Trim(), value.Trim());
            }

            return meta;
        }

        public string Title = "";
        public string Artist = "";
        public string EffectedBy = "";

        public string JacketPath;
        public string Illustrator = "";

        public Difficulty Difficulty = Difficulty.Novice;
        public int Level = 1;

        public string BeatsPerMinute = "";
        public int Numerator = 4;
        public int Denominator = 4;

        public string MusicFile;
        public string MusicFileNoFx;
        public int MusicVolume = 100;

        public int OffsetMillis;

        public string Background = "";
        // Layer

        public int PreviewOffsetMillis;
        public int PreviewLengthMillis;

        public int PFilterGain = 100;
        public string FilterType = "";
            
        public int SlamAutoVolume = 100;
        public int SlamVolume = 100;

        public string Tags = "";

        public void Set(string name, string value)
        {
            switch (name)
            {
                case "title": Title = value; return;
                case "artist": Artist = value; return;
                case "effect": EffectedBy = value; return;

                case "jacket": JacketPath = value; return;
                case "illustrator": Illustrator = value; return;

                case "difficulty":
                    {
                        var dif = Difficulty.Novice;
                        if (value == "challenge")
                            dif = Difficulty.Challenge;
                        else if (value == "extended")
                            dif = Difficulty.Extended;
                        else if (value == "infinite")
                            dif = Difficulty.Infinite;
                        Difficulty = dif;
                    }
                    return;
                case "level": Level = int.Parse(value); return;
                        
                case "t": BeatsPerMinute = value; return;
                case "beat":
                    if (value.TrySplit('/', out string n, out string d))
                    {
                        Numerator = int.Parse(n);
                        Denominator = int.Parse(d);
                    }
                    return;

                case "m":
                    {
                        if (value.TrySplit(';', out string nofx, out string fx))
                        {
                            MusicFileNoFx = nofx;
                            MusicFile = fx;

                            if (fx.TrySplit(';', out fx, out string _))
                                ; // do something with the last file
                        }
                        else MusicFileNoFx = value;
                    }
                    return;

                case "mvol": MusicVolume = int.Parse(value); return;
                        
                case "o": OffsetMillis = int.Parse(value); return;

                case "po": PreviewOffsetMillis = int.Parse(value); return;
                case "plength": PreviewLengthMillis = int.Parse(value); return;

                case "pfiltergain": PFilterGain = int.Parse(value); return;
                case "filtertype": FilterType = value; return;

                case "chokkakuvol": SlamVolume = int.Parse(value); return;

                case "tags": Tags = value; return;
            }
        }
    }
}
