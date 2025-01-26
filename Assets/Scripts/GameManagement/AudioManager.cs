using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
    private AudioSource _audioSource;
    [FormerlySerializedAs("_audioClips")] [SerializeField] private AudioClip[] audioClips;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_audioSource.isPlaying) return;
        _audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)]);
    }
}
