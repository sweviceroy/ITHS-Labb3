using System;
using System.Windows.Media;   // MediaPlayer ligger här

namespace ITHSLab3.Services
{
    public class AudioService
    {
        private readonly MediaPlayer _player = new MediaPlayer();

        public void PlayLoop(string filePath)
        {
            try
            {
                _player.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));

                // loopa genom att starta om från början när låten är slut
                _player.MediaEnded += (s, e) =>
                {
                    _player.Position = TimeSpan.Zero;
                    _player.Play();
                };

                _player.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Audio error: " + ex.Message);
            }
        }

        public void Stop()
        {
            _player.Stop();
        }
    }
}
