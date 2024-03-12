using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicBeh : MonoBehaviour
{
    [SerializeField]
    AudioClip menuMusicClip;
    [SerializeField]
    AudioClip battleMusicClips;
    [ContextMenu("PlayAfterMenu")]
    public void PlayMenuMusic()
    {
        AudioManager.instance.PlayMusicForced(menuMusicClip, true);
    }
    [ContextMenu("PlayBattle")]
    public void PlayBattleMusic()
    {
        AudioManager.instance.PlayMusicForced(battleMusicClips, true);
    }
}
