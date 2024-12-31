using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

using System.Collections.Generic;

namespace Navy.Audio
{
    public static class Audio
    {
        private static ContentManager contentManager;
        public static void Init(ContentManager content)
        {
            contentManager = content;
        }

        public static Song AddSong(Song song, string name)
        {
            songs[name] = song;
            return songs[name];
        }

        public static Song AddSong(string path, string name)
        {
            songs[name] = contentManager.Load<Song>(path);
            return songs[name];
        }

        public static SoundEffect AddSoundEffect(SoundEffect soundEffect, string name)
        {
            soundEffects[name] = soundEffect;
            return soundEffects[name];

        }

        public static SoundEffect AddSoundEffect(string path, string name)
        {
            soundEffects[name] = contentManager.Load<SoundEffect>(path);
            return soundEffects[name];
        }

        public static void PlaySound(string name, float volume)
        {
            SoundEffectInstance soundInstance = soundEffects[name].CreateInstance();
            soundInstance.Volume = volume;
            soundInstance.Play();
        }

        public static void PlaySong(string name, float volume, bool isLooping = false)
        {
            MediaPlayer.Volume = volume;
            MediaPlayer.IsRepeating = isLooping;
            MediaPlayer.Play(songs[name]);
        }

        public static void StopSong()
        {

            MediaPlayer.Stop();
        }

        public static void PauseSong()
        {
            MediaPlayer.Pause();
        }

        public static void ResumeSong()
        {
            MediaPlayer.Resume();
        }

        public static void FadeSong(double _endValue, double _fadeTime, bool _endOnFinish = true)
        {
            startValue = MediaPlayer.Volume;
            endValue = _endValue;
            fadeTime = _fadeTime;
            isFading = true;
            timeElapsed = 0;
            endOnFinish = _endOnFinish;
        }

        public static void FadeInSong(string name, bool isLooping, double _startValue, double _endValue, double _fadeTime)
        {
            MediaPlayer.Play(songs[name]);
            MediaPlayer.IsRepeating = isLooping;
            startValue = _startValue;
            endValue = _endValue;
            fadeTime = _fadeTime;
            isFading = true;
            timeElapsed = 0;
            endOnFinish = false;
        }

        public static void Update(GameTime gameTime)
        {
            for (int i = 0; i < playingSoundEffects.Count; i++)
            {
                if (playingSoundEffects[i].State == SoundState.Stopped)
                {
                    playingSoundEffects.RemoveAt(i);
                }
            }

            if (isFading)
            {
                if (timeElapsed >= fadeTime)
                {
                    isFading = false;

                    if (endOnFinish)
                    {
                        endOnFinish = false;
                        StopSong();
                    }
                }

                else
                {
                    MediaPlayer.Volume = (float)MathUtils.Lerp(startValue, endValue, timeElapsed / fadeTime);
                    timeElapsed += gameTime.ElapsedGameTime.TotalSeconds;
                }
            }
        }


        public static Song CurrentlyPlayingSong
        {
            get
            {
                return MediaPlayer.Queue.ActiveSong;
            }
        }

        // -1 out 0 no 1 in
        private static bool isFading, endOnFinish;
        private static double startValue, endValue, fadeTime, timeElapsed;

        private static readonly Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        private static readonly Dictionary<string, Song> songs = new Dictionary<string, Song>();

        private static readonly List<SoundEffectInstance> playingSoundEffects = new List<SoundEffectInstance>();
    }
}
