using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField]
    public AudioMixer audioMixer;
    [SerializeField]
    private AudioSource audioSourceObj;
    [SerializeField]
    AudioMixerGroup musicGroup;
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
    private void Update()
    {

    }
    void InitManager()
    {
        if (GameManager.instance == null)
        {
            Invoke("InitManaget", 0.1f);
            return;
        }
        if(musicSourceObj1 == null && GameManager.instance.playerCameraRef != null)
            musicSourceObj1 = Instantiate(audioSourceObj, GameManager.instance.playerCameraRef.transform);
        else
            musicSourceObj1 = Instantiate(audioSourceObj, FindObjectOfType<AudioListener>().transform);

        if (musicSourceObj2 == null && GameManager.instance.playerCameraRef != null)
            musicSourceObj2 = Instantiate(audioSourceObj, GameManager.instance.playerCameraRef.transform);
        else
            musicSourceObj2 = Instantiate(audioSourceObj, FindObjectOfType<AudioListener>().transform);
        musicSourceObj1.outputAudioMixerGroup = musicGroup;
        musicSourceObj1.volume = 1f;
        musicSourceObj1.loop = true;
        musicSourceObj2.outputAudioMixerGroup = musicGroup;
        musicSourceObj2.volume = 1f;
        musicSourceObj2.loop = true;
    }
    void InitManager(Scene _scene, LoadSceneMode _mode)
    {
        if (GameManager.instance == null)
        {
            Invoke("InitManaget", 0.1f);
            return;
        }
        if (musicSourceObj1 == null && GameManager.instance.playerCameraRef != null)
            musicSourceObj1 = Instantiate(audioSourceObj, GameManager.instance.playerCameraRef.transform);
        else
            musicSourceObj1 = Instantiate(audioSourceObj, FindObjectOfType<AudioListener>().transform);

        if (musicSourceObj2 == null && GameManager.instance.playerCameraRef != null)
            musicSourceObj2 = Instantiate(audioSourceObj, GameManager.instance.playerCameraRef.transform);
        else
            musicSourceObj2 = Instantiate(audioSourceObj, FindObjectOfType<AudioListener>().transform);
        musicSourceObj1.outputAudioMixerGroup = musicGroup;
        musicSourceObj1.volume = 1f;
        musicSourceObj1.loop = true;
        musicSourceObj2.outputAudioMixerGroup = musicGroup;
        musicSourceObj2.volume = 1f;
        musicSourceObj2.loop = true;

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
    public void PlayRandomOneShotSoundFXClip(AudioClip[] _audioclips, Transform _spawnTransform, float _volume, float falloffDistance)
    {
        int randI = Random.Range(0, _audioclips.Length);
        AudioSource audioSource = Instantiate(audioSourceObj, _spawnTransform.position, Quaternion.identity);
        audioSource.clip = _audioclips[randI];
        Vector3 dirToListener = GameManager.instance.playerRef.transform.position - audioSource.transform.position;
        if (dirToListener.magnitude <= 1.1f)
            audioSource.spatialBlend = 0f;
        audioSource.maxDistance = falloffDistance;
        audioSource.volume = _volume;
        audioSource.Play();
        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    [SerializeField]
    AudioSource musicSourceObj1;
    [SerializeField]
    AudioSource musicSourceObj2;
    public void PlayMusicForced(AudioClip _musicClip, bool isLooped)
    {
        if (musicSourceObj1.isPlaying == true)
        {
            musicSourceObj2.clip = _musicClip;
            musicSourceObj2.loop = isLooped;
            musicSourceObj2.PlayScheduled(AudioSettings.dspTime);
            SmoothChangeAudioSource(musicSourceObj2, musicSourceObj1);
        }
        else
        {
            musicSourceObj1.clip = _musicClip;
            musicSourceObj1.loop = isLooped;
            musicSourceObj1.PlayScheduled(AudioSettings.dspTime);
            SmoothChangeAudioSource(musicSourceObj1, musicSourceObj2);
        }

    }
    Coroutine musicSourceChanging;
    void SmoothChangeAudioSource(AudioSource _audioS1, AudioSource _audioS2)
    {
        if (musicSourceChanging != null)
            StopCoroutine(musicSourceChanging);
        musicSourceChanging = StartCoroutine(ChangingMusicSourceVolume(_audioS1, _audioS2, 3f));
    }
    IEnumerator ChangingMusicSourceVolume(AudioSource _audioSource1, AudioSource _audioSource2, float _timeToChange)
    {
        float time = 0;
        float currentVolume1 = _audioSource1.volume;
        float currentVolume2 = _audioSource2.volume;
        while (time < _timeToChange) 
        {
            _audioSource1.volume = Mathf.Lerp(currentVolume1, 1, time / _timeToChange);
            _audioSource2.volume = Mathf.Lerp(currentVolume2, 0, time / _timeToChange);
            time += Time.unscaledDeltaTime;
            yield return null;
        }
        _audioSource1.volume = 1;
        _audioSource2.volume = 0;
        _audioSource2.Stop();
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
