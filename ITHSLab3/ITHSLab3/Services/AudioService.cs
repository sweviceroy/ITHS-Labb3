using System;
using System.Windows.Media;   // MediaPlayer ligger här

namespace ITHSLab3.Services
{
    public class AudioService
    {
        // Music player for background music (looping)
        private readonly MediaPlayer _musicPlayer = new MediaPlayer();

        public void PlayLoop(string filePath)
        {
            try
            {
                // reset any previous loop handler
                _musicPlayer.Stop();
                _musicPlayer.MediaEnded -= MusicPlayerOnMediaEnded;

                _musicPlayer.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
                _musicPlayer.MediaEnded += MusicPlayerOnMediaEnded;
                _musicPlayer.Play();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Audio error (loop): " + ex.Message);
            }
        }

        private void MusicPlayerOnMediaEnded(object? sender, EventArgs e)
        {
            // loopa genom att starta om från början när låten är slut
            _musicPlayer.Position = TimeSpan.Zero;
            _musicPlayer.Play();
        }

        // Play a short, one-shot sound effect (does not affect music)
        public void PlayOneShot(string filePath, double volume = 1.0)
        {
            try
            {
                var player = new MediaPlayer();
                player.Open(new Uri(filePath, UriKind.RelativeOrAbsolute));
                player.Volume = volume;
                player.Play();

                // clean up when finished
                player.MediaEnded += (s, e) =>
                {
                    player.Close();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("Audio error bla bla bla (sfx): " + ex.Message);
            }
        }

        public void Stop()
        {
            _musicPlayer.Stop();
            _musicPlayer.MediaEnded -= MusicPlayerOnMediaEnded;
        }
    }
}
