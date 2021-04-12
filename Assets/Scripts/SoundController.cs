using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField][Range(0f, 1f)] float _startVolume;
    private AudioSource _audioSourceA;
    private AudioSource _audioSourceB;
    private int _audioNumber;
    private float _crossfadeTime;

    void Awake()
    {
        _audioNumber = 0;
        _audioSourceA = gameObject.AddComponent<AudioSource>();
        _audioSourceB = gameObject.AddComponent<AudioSource>();
        _audioSourceB.volume = Mathf.Clamp(_startVolume, 0, 1);
        _audioSourceA.volume = 0;
        _crossfadeTime = 6f;
    }

    void OnEnable() 
    {
        MapCoverController.MapCoverRevealedEvent += MapCoverController_MapCoverRevealedEvent;
    }

    void OnDisable() 
    {
        MapCoverController.MapCoverRevealedEvent -= MapCoverController_MapCoverRevealedEvent;
    }

    void PlayNextAudio()
    {
        if (_audioSourceA.isPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(Crossfade(_audioSourceA, _audioSourceB, _crossfadeTime));
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(Crossfade(_audioSourceB, _audioSourceA, _crossfadeTime));
        }
    }

    void MapCoverController_MapCoverRevealedEvent(GameObject mapCoverObject) => PlayNextAudio();

    IEnumerator Crossfade(AudioSource a, AudioSource b, float transitionTime)
    {

        // teilt die zeit und das volume in kleine st�cke
        float intervalSize = 20f;
        float stepInterval = transitionTime / intervalSize;
        float volInterval = a.volume / intervalSize;

        // audiosourceB bekommt den nachsten clip zum spielen
        b.clip = audioClips[_audioNumber];
        _audioNumber++;
        // wenn _audionumber gro�er als das array ist, wirds auf 0 gesetzt
        if (_audioNumber >= audioClips.Length)
        {
            _audioNumber = 0;
        }
        b.Play();

        // die kleinen volumestuecke werden bei a weggenommen
        // und bei b hinzugefuegt. 
        for (int i = 0; i < 20; i++)
        {
            a.volume -= volInterval;
            b.volume += volInterval;

            yield return new WaitForSeconds(stepInterval);
        }
        a.Stop();
    }
}
