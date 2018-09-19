using System;
using System.Collections.Generic;
using System.Text;

namespace OpenRM
{
    public sealed class ChartMetadata
    {
        /// <summary>
        /// Initialize the m_audioFiles with a single placeholder primary.
        /// </summary>
        private List<string> m_audioFiles = new List<string>() { ".wav" };
        /// <summary>
        /// All audio files associated with this chart.
        /// 
        /// Technically, any object can store a string pointing to
        ///  an audio source and load it as well, but this provides
        ///  a unified interface which can simplify that significantly
        ///  by simply aggregating all audio file names and refering
        ///  to them by index instead of path.
        ///  
        /// Rhythm game charts all tend to have at least one audio file;
        ///  it's rare to have a rhythm game without external music, but not
        ///  unheard of.
        /// The most common placement for that audio track should be the primary one.
        /// For charts which have multiple layers of audio playing at the same time
        ///  to form a complete song, the primary should be the one constant and
        ///  probably in the background of the rest.
        /// For example, in Guitar Hero you'll have the rest of the band play behind
        ///  the guitar track, so having the band be Primary and the guitar be 
        ///  Secondary would be the prefered orientation.
        /// </summary>
        public IList<string> AudioFiles
        {
            get => m_audioFiles;
            set
            {
                m_audioFiles.Clear();
                // if the value is null, just don't add anything.
                if (value != null)
                    // copy the data in, instead of maintaining a reference to the value
                    m_audioFiles.AddRange(value);
            }
        }

        /// <summary>
        /// Returns true if this chart provides an audio file at index `n` and false otherwise.
        /// </summary>
        /// <param name="n">The index of an audio file, starting at 0.</param>
        public bool HasNthAudioFile(int n) => m_audioFiles.Count > n;

        #region Audio File Access Wrappers

        /// <summary>
        /// The primary audio, the first audio file listed.
        /// </summary>
        public string PrimaryAudioFile { get => m_audioFiles[0]; set => m_audioFiles[0] = value; }
        public bool HasPrimaryAudioFile => HasNthAudioFile(0);

        /// <summary>
        /// The secondary audio, the second audio file listed.
        /// </summary>
        public string SecondaryAudioFile { get => m_audioFiles[1]; set => m_audioFiles[1] = value; }
        public bool HasSecondaryAudioFile => HasNthAudioFile(1);

        /// <summary>
        /// The tertiary audio, the third audio file listed.
        /// </summary>
        public string TertiaryAudioFile { get => m_audioFiles[2]; set => m_audioFiles[2] = value; }
        public bool HasTertiaryAudioFile => HasNthAudioFile(2);

        #endregion
    }
}
