using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour 
{
    public static SoundManager soundMgr = null;

    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    public AudioClip[] clip;
    public AudioSource audioSource;

    public void Initialize()
    {
        if (soundMgr == null) soundMgr = this;
        else if (soundMgr != this) Destroy(this.gameObject);

        audioClips.Add("MainBGM",   clip[0]);
        audioClips.Add("GameBGM",   clip[1]);
        audioClips.Add("Intro",     clip[2]);
        audioClips.Add("Logo",      clip[3]);
        audioClips.Add("Warning",   clip[4]);
        audioClips.Add("Normal",    clip[5]);
        audioClips.Add("Fail",      clip[6]);
        audioClips.Add("Click",     clip[7]);
        audioClips.Add("GameOver",  clip[8]);
        audioClips.Add("Suc",       clip[9]);
        audioClips.Add("LevelUp",   clip[10]);
    }

    public void PlayBGM(string name)
    {
        if (DataManager.b_Sound)
        {
            if (audioSource.isPlaying)
                audioSource.Stop();

            audioSource.clip = audioClips[name];
            audioSource.Play();
        }
    }

    public void PlayES(string name)
    {
        if(DataManager.b_Sound)
            AudioSource.PlayClipAtPoint(audioClips[name], Camera.main.transform.position);
    }
}
