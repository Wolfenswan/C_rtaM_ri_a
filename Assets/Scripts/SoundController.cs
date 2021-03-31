using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField][Range(0f, 1f)] float _startVolume;
    private AudioSource _audioSourceA;
    private AudioSource _audioSourceB;
    private int _audionumber;
    private float _crossfadeTime;

    void Awake()
    {
        _audionumber = 0;
        _audioSourceA = gameObject.AddComponent<AudioSource>();
        _audioSourceB = gameObject.AddComponent<AudioSource>();
        _audioSourceB.volume = Mathf.Clamp(_startVolume, 0, 1);
        _audioSourceA.volume = 0;
        _crossfadeTime = 6f;
    }

    private void Start()
    {
        PlayNextAudio();
    }

    public void PlayNextAudio()
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

    IEnumerator Crossfade(AudioSource a, AudioSource b, float _transitionTime)
    {

        // teilt die zeit und das volume in kleine stücke
        float _interval_size = 20f;
        float _step_interval = _transitionTime / _interval_size;
        float _vol_interval = a.volume / _interval_size;

        // audiosourceB bekommt den nachsten clip zum spielen
        b.clip = audioClips[_audionumber];
        _audionumber++;
        // wenn _audionumber großer als das array ist, wirds auf 0 gesetzt
        if (_audionumber >= audioClips.Length)
        {
            _audionumber = 0;
        }
        b.Play();

        // die kleinen volumestuecke werden bei a weggenommen
        // und bei b hinzugefuegt. 
        for (int i = 0; i < 20; i++)
        {
            a.volume -= _vol_interval;
            b.volume += _vol_interval;

            yield return new WaitForSeconds(_step_interval);
        }
        a.Stop();
    }
}
