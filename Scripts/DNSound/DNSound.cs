using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace DarkNaku {
    public sealed class Sound : SingletonBehaviour<Sound> {
        private string _resourcePath = null;
        private float _defaultFadeTimeOfBGM = 0.5F;
        private List<AudioSource> _players = new List<AudioSource>();
        private List<AudioSource> _bgmPlayers = new List<AudioSource>();
        private Dictionary<string, AudioClip> _clips = new Dictionary<string, AudioClip>();
        private Dictionary<string, AudioSource> _playedClipInThisFrame = new Dictionary<string, AudioSource>();
        private bool _isRunningDefender = false;

        public static string ResourcePath {
            get { return Instance._resourcePath; }
            set { Instance._resourcePath = value; }
        }

        public static float DefaultFadeTimeOfBGM {
            get { return Instance._defaultFadeTimeOfBGM; }
            set { Instance._defaultFadeTimeOfBGM = Mathf.Max(0F, value); }
        }

        public static int BGMChannelCount {
            get { return Instance._bgmPlayers.Count; }
            set { Instance.AdjustBGMChannel(Mathf.Max(1, value)); }
        }

        public static void Play(string clipName) {
            Instance._Play(clipName, 1F, false, 0F);
        }

        public static void Play(string clipName, float volume) {
            Instance._Play(clipName, volume, false, 0F);
        }

        public static void Play(string clipName, float volume, bool loop) {
            Instance._Play(clipName, volume, loop, 0F);
        }

        public static void Play(string clipName, float volume, bool loop, float fadeTime) {
            Instance._Play(clipName, volume, loop, fadeTime);
        }

        public static void PlayBGM(string clipName) {
            PlayBGM(0, clipName, 1F, DefaultFadeTimeOfBGM);
        }

        public static void PlayBGM(string clipName, float volume) {
            PlayBGM(0, clipName, volume, DefaultFadeTimeOfBGM);
        }

        public static void PlayBGM(int channel, string clipName) {
            PlayBGM(channel, clipName, 1F, DefaultFadeTimeOfBGM);
        }

        public static void PlayBGM(string clipName, float volume, float fadeTime) {
            PlayBGM(0, clipName, volume, fadeTime);
        }

        public static void PlayBGM(int channel, string clipName, float volume) {
            PlayBGM(channel, clipName, volume, DefaultFadeTimeOfBGM);
        }

        public static void PlayBGM(int channel, string clipName, float volume, float fadeTime) {
            Instance._PlayBGM(channel, clipName, volume, fadeTime);
        }

        public static void Stop(string clipName) {
            Instance._Stop(clipName, 0F);
        }

        public static void Stop(string clipName, float fadeTime) {
            Instance._Stop(clipName, fadeTime);
        }

        public static void StopBGM() {
            Instance._StopBGM(0, 0.5F);
        }

        public static void StopBGM(float fadeTime) {
            Instance._StopBGM(0, fadeTime);
        }

        public static void StopBGM(int channel, float fadeTime) {
            Instance._StopBGM(channel, fadeTime);
        }

        public static void PrevLoadClip(params string[] clipNames) {
            for (int i = 0; i < clipNames.Length; i++) {
                Instance.GetClip(clipNames[i]);
            }
        }

        public static void Clear() {
            Instance._Clear();
        }

        protected override void OnInitialize() {
            AdjustBGMChannel(1);
        }

        private AudioSource _Play(string clipName, float volume, bool loop, float fadeTime) {
            if (_playedClipInThisFrame.ContainsKey(clipName)) {
                return _playedClipInThisFrame[clipName];
            }

            AudioSource player = GetPlayer();

            RunPlayer(player, clipName, volume, loop, fadeTime);

            player.name = string.Format("Player - {0}", clipName);
            _playedClipInThisFrame.Add(clipName, player);
            StartCoroutine(CoDefendSimultaneityPlay());

            return player;
        }

        private void RunPlayer(AudioSource player, string clipName, float volume, bool loop, float fadeTime) {
            Assert.IsNotNull(player, "[Sound] RunPlay : Player can not be null.");

            player.clip = GetClip(clipName);
            player.loop = loop;

            if (fadeTime > 0F) {
                player.volume = 0F;
                StartCoroutine(CoPlay(player, volume, fadeTime));
            } else {
                player.volume = volume;
                player.Play();
            }
        }

        private IEnumerator CoPlay(AudioSource player, float volume, float fadeTime) {
            Assert.IsNotNull(player, "[Sound] CoPlay : Player can not be null.");

            float elapsedTime = 0F;
            player.Play();

            while (elapsedTime < fadeTime) {
                elapsedTime += Time.deltaTime;
                player.volume = Mathf.Lerp(0F, volume, elapsedTime / fadeTime);
                yield return null;
            }
        }

        private void _Stop(string clipName, float fadeTime) {
            for (int i = 0; i < _players.Count; i++) {
                if (_bgmPlayers.Contains(_players[i])) continue;
                if (_players[i].clip.name != clipName) continue;
                _Stop(_players[i], fadeTime);
            }
        }

        private void _Stop(AudioSource player, float fadeTime) {
            Assert.IsNotNull(player, "[Sound] _Stop : Player can not be null.");

            if (player.isPlaying) {
                if (fadeTime > 0F) {
                    StartCoroutine(CoStop(player, fadeTime));
                } else {
                    player.Stop();
                    player.clip = null;
                }
            }
        }

        private void _StopBGM(int channel, float fadeTime) {
            Assert.IsTrue(channel < _bgmPlayers.Count,
                string.Format("[Sound] _StopBGM : {0} is invalid channel.", channel));
            _Stop(_bgmPlayers[channel], fadeTime);
        }

        private IEnumerator CoStop(AudioSource player, float fadeTime) {
            Assert.IsNotNull(player, "[Sound] CoStop : Player can not be null.");
            Assert.IsTrue(player.isPlaying, "[Sound] CoStop : Player is not playing.");

            float elapsedTime = 0F;
            float startVolume = player.volume;

            while (player.isPlaying) {
                elapsedTime += Time.deltaTime;
                player.volume = Mathf.Lerp(startVolume, 0F, elapsedTime / fadeTime);

                if (player.volume <= 0F) {
                    player.Stop();
                    player.clip = null;
                }

                yield return null;
            }
        }

        private void _PlayBGM(int channel, string clipName, float volume, float fadeTime) {
            StartCoroutine(CoPlayBGM(channel, clipName, volume, fadeTime));
        }

        private IEnumerator CoPlayBGM(int channel, string clipName, float volume, float fadeTime) {
            Assert.IsTrue(channel < _bgmPlayers.Count,
                string.Format("[Sound] CoPlayBGM : {0} is invalid channel.", channel));

            for (int i = 0; i < _bgmPlayers.Count; i++) {
                if (i == channel) continue;
                Assert.IsFalse(_bgmPlayers[i].isPlaying && (_bgmPlayers[i].clip.name == clipName),
                    string.Format("[Sound] CoPlayBGM : '{0}' is aleady playing.", clipName));
            }

            if (_bgmPlayers[channel].isPlaying) {
                fadeTime *= 0.5F;

                if (fadeTime > 0F) {
                    yield return StartCoroutine(CoStop(_bgmPlayers[channel], fadeTime));
                }
            }

            RunPlayer(_bgmPlayers[channel], clipName, volume, true, fadeTime);
            _bgmPlayers[channel].name = string.Format("BGM - {0}", clipName);
        }

        private IEnumerator CoDefendSimultaneityPlay() {
            if (_isRunningDefender) yield break;
            _isRunningDefender = true;
            yield return new WaitForEndOfFrame();
            _playedClipInThisFrame.Clear();
            _isRunningDefender = false;
        }

        private void _Clear() {
            List<string> usingClipNames = new List<string>();
            List<string> unUsingClipNames = new List<string>(_clips.Keys);

            for (int i = 0; i < _players.Count; i++) {
                if (_players[i].isPlaying == false) {
                    _players[i].clip = null;
                    continue;
                }

                if (usingClipNames.Contains(_players[i].clip.name)) continue;
                usingClipNames.Add(_players[i].clip.name);
                unUsingClipNames.Remove(_players[i].clip.name);
            }

            for (int i = 0; i < unUsingClipNames.Count; i++) {
                _clips.Remove(unUsingClipNames[i]);
            }
        }

        private void AdjustBGMChannel(int channelCount) {
            Assert.IsTrue(channelCount > 0, "[Sound] AdjustBGMChannel : Channel count must be more than zero.");
            if (_bgmPlayers.Count == channelCount) return;

            if (_bgmPlayers.Count > channelCount) {
                _bgmPlayers.RemoveRange(channelCount - 1, _bgmPlayers.Count - channelCount);
            } else {
                int count = channelCount - _bgmPlayers.Count;

                for (int i = 0; i < count; i++) {
                    _bgmPlayers.Add(GetPlayer());
                }
            }
        }

        private AudioSource GetPlayer() {
            AudioSource player = null;

            for (int i = 0; i < _players.Count; i++) {
                if (_players[i].isPlaying) continue;
                if (_bgmPlayers.Contains(_players[i])) continue;
                player = _players[i];
                break;
            }

            if (player == null) {
                player = new GameObject("Player").AddComponent<AudioSource>();
                player.transform.parent = transform;
                _players.Add(player);
            }

            return player;
        }

        private AudioClip GetClip(string clipName) {
            AudioClip clip = null;

            string clipPath = string.IsNullOrEmpty(_resourcePath) ? clipName : System.IO.Path.Combine(_resourcePath, clipName);

            if (_clips.ContainsKey(clipName)) {
                clip = _clips[clipName];
            } else {
                clip = Resources.Load<AudioClip>(clipPath);
                Assert.IsNotNull(clip, string.Format("[Sound] _Play : Can not found clip - {0}", clipPath));
                _clips.Add(clipName, clip);
            }

            return clip;
        }
    }
}