using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadableAudio : MonoBehaviour
{
    [SerializeField] Transform _parent = null;
    [SerializeField] float _volume = 1f;
    [SerializeField] bool _fadeIn = false;
    [SerializeField] bool _fadeOut = false;
    [SerializeField] float _fadeInLength = .5f;
    [SerializeField] float _fadeOutLength = .5f;

    [SerializeField] GameObject _SfxPrefab;

    private AudioSource _audioSource;
    private bool _fadeOuting;

    private void Awake()
    {
        _audioSource = bsj.SoundManager.Instance.PlayAttached(_SfxPrefab, _parent,true).GetComponent<AudioSource>();
        _audioSource.Stop();
        _volume = 1f;
    }

    public void Play()
    {
        if (_fadeIn)
        {
            StartCoroutine(FadeIn(1f / _fadeInLength));
        }
        else
        {
            _audioSource.volume = _volume;
            _audioSource.Play();
        }

    }
    public void Stop()
    {
        if (_fadeOut)
        {
            _fadeOuting = true;
            StartCoroutine(FadeOut(1f / _fadeOutLength));
        }
        else
        {
            _audioSource.volume = 0f;
            _audioSource.Stop();
        }
    }

    public void SetVolum(float vol)
    {
        _audioSource.volume = vol;
    }
    private IEnumerator FadeIn(float speed)
    {
        _audioSource.gameObject.SetActive(true);
        _audioSource.volume = 0f;
        _audioSource.Play();
        while (_audioSource.volume < _volume)
        {
            if (_fadeOuting)
                yield break;
            _audioSource.volume += speed * Time.deltaTime;
            yield return null;
        }
        _audioSource.volume = _volume;
    }
    private IEnumerator FadeOut(float speed)
    {
        while (_audioSource.volume > 0f)
        {
            _audioSource.volume -= speed * Time.deltaTime;
            yield return null;
        }
        _audioSource.volume = 0f;
        _audioSource.Stop();
        _fadeOuting = false;
    }


}
