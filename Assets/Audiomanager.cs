using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SFX
{
    Button_Click, Payline, ReelSpin, ReelStop, SpinBtn,
    Bonus_Hit, Transition, Wheelspin, BonusIntro, BonusOutro, WheelBuffalo, WheelOutcome, jackpotOpen,
    FreeIntro, FreeOutro, SuperWin, counterLoop, anticipation, slamStop
}
public enum Music
{
    Bgm,
    FreeBgm
}
[System.Serializable]
public class SFXItem
{
    public String name;
    public AudioClip _audioClip;
    public SFX _sfx;
}

[System.Serializable]
public class MusicItem
{
    public AudioClip _audioMusic;
    public Music _music;
}
public class Audiomanager : MonoBehaviour
{
    Dictionary<SFX, AudioClip> audioClipDictionary = new Dictionary<SFX, AudioClip>();
    public SFXItem[] _sfxitems;

    Dictionary<Music, AudioClip> audioMusicDictionary = new Dictionary<Music, AudioClip>();
    public MusicItem[] _musicItems;
    public static Audiomanager Instance;
    public AudioSource _uiAudioSource, _sfxAudioSource, _musicAudioSource, _sfxLoudVolAudioSource, _sfxLoopAudioSource;
    SFX SoundToPlayTwice;
    void Awake()
    {
        foreach (SFXItem clip in _sfxitems)
        {
            audioClipDictionary.Add(clip._sfx, clip._audioClip);
        }
        foreach (MusicItem clip in _musicItems)
        {
            audioMusicDictionary.Add(clip._music, clip._audioMusic);

        }
        if (!Instance)
        {
            Instance = this;
        }
    }
    void Start()
    {
        PlayMusic(Music.Bgm);
    }

    public void PlayUiSfx(SFX sfx) => _uiAudioSource.PlayOneShot(audioClipDictionary[sfx]);
    public void PlaySfx(SFX sfx, bool playMusicOnComplete = true, float delay = 0)
    {
        if (sfx == SFX.ReelSpin)
        {
            _sfxLoopAudioSource.clip = audioClipDictionary[sfx];
            _sfxLoopAudioSource.loop = true;
            _sfxLoopAudioSource.Play();
        }
        else if (sfx == SFX.BonusIntro || sfx == SFX.BonusOutro || sfx == SFX.SuperWin)
        {

            _sfxLoudVolAudioSource.PlayOneShot(audioClipDictionary[sfx]);
            _sfxAudioSource.Stop();

            float clipLength = audioClipDictionary[sfx].length;
            if (!_musicAudioSource.isPlaying && playMusicOnComplete)
                _musicAudioSource.PlayDelayed(clipLength);

        }
        else
        {
            _sfxAudioSource.PlayOneShot(audioClipDictionary[sfx]);
            if (sfx == SFX.WheelOutcome)
            {
                // float delay = 1.0f;
                // SoundToPlayTwice = sfx;
                Invoke(nameof(PlaySoundAgain), delay);
            }
        }
    }
    public void PlaySfx(AudioClip audioClip)
    {

        _sfxAudioSource.PlayOneShot(audioClip);


    }
    void PlaySoundAgain()
    {
        _sfxAudioSource.PlayOneShot(audioClipDictionary[SoundToPlayTwice]);
    }
    public void PlayMusic(Music _music)
    {

        _musicAudioSource.clip = audioMusicDictionary[_music];
        _musicAudioSource.Play();
        _musicAudioSource.loop = true;

        StartCoroutine(FadeIn());
    }
    private IEnumerator FadeIn()
    {
        float currentVolume = _musicAudioSource.volume;
        _musicAudioSource.volume = 0;

        while (_musicAudioSource.volume < currentVolume)
        {
            _musicAudioSource.volume += currentVolume * Time.deltaTime / 1f;
            yield return null;
        }

        StopCoroutine(FadeIn());
    }
    public void StopMusic(Music _music)
    {

        _musicAudioSource.clip = audioMusicDictionary[_music];
        _musicAudioSource.Stop();
    }
    public void StopSfx(SFX _sfx)
    {
        _sfxAudioSource.Stop();
        if (_sfx == SFX.ReelSpin)
        {
            _sfxLoopAudioSource.Stop();
        }

    }

    public void OnMusicMute(Music normalBg)
    {
        foreach (Music music in audioMusicDictionary.Keys)
        {
            StopMusic(music);
        }
    }


}
