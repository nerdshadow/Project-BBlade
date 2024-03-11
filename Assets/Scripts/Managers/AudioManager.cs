using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    AudioMixer audioMixer;
    [SerializeField]
    private AudioSource audioSourceObj;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitManager();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += InitManager;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= InitManager;
    }
    void InitManager()
    {
    
    }
    void InitManager(Scene _scene, LoadSceneMode _mode)
    {
        
    }
    public void PlayOneShotSoundFXClip(AudioClip _audioclip, Transform _spawnTransform, float _volume)
    {
        AudioSource audioSource = Instantiate(audioSourceObj, _spawnTransform.position, Quaternion.identity);

        audioSource.clip = _audioclip;
        audioSource.volume = _volume;
        Vector3 dirToListener = GameManager.instance.playerRef.transform.position - audioSource.transform.position;
        if (dirToListener.magnitude <= 1.1f)
            audioSource.spatialBlend = 0f;
        audioSource.Play();
        float clipLength = _audioclip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayOneShotSoundFXClip(AudioClip _audioclip, Transform _spawnTransform, float _volume, float falloffDistance)
    {
        AudioSource audioSource = Instantiate(audioSourceObj, _spawnTransform.position, Quaternion.identity);

        audioSource.clip = _audioclip;
        audioSource.volume = _volume;
        Vector3 dirToListener = GameManager.instance.playerRef.transform.position - audioSource.transform.position;
        if (dirToListener.magnitude <= 1.1f)
            audioSource.spatialBlend = 0f;
        audioSource.maxDistance = falloffDistance;
        audioSource.Play();
        float clipLength = _audioclip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
    public void PlayRandomOneShotSoundFXClip(AudioClip[] _audioclips, Transform _spawnTransform, float _volume)
    {
        int randI = Random.Range(0, _audioclips.Length);
        AudioSource audioSource = Instantiate(audioSourceObj, _spawnTransform.position, Quaternion.identity);
        audioSource.clip = _audioclips[randI];
        Vector3 dirToListener = GameManager.instance.playerRef.transform.position - audioSource.transform.position;
        if (dirToListener.magnitude <= 1.1f)
            audioSource.spatialBlend = 0f;
        audioSource.volume = _volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }
    //Set slider values to 0.0001 -> 1
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }
    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }
}
