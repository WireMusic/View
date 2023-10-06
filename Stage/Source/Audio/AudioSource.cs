using Stage.Utils;

using NAudio.Wave;
using NAudio.Wave.SampleProviders;

using System;
using System.IO;
using System.Threading;

namespace Stage.Audio
{
    public class AudioSource
    {
        public AudioSource(string path, CancellationToken token)
        {
            if (Path.GetExtension(path) != ".wav")
                throw new ArgumentException("Path must be of type 'wav'!");
        }
    }
}
