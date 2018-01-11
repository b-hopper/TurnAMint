using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {

    [SerializeField] AudioClip[] clips;
    [SerializeField] float delayBetweenClips;

    AudioSource source;
    bool canPlay;

    private void Awake()
    {
        canPlay = true;
        source = GetComponent<AudioSource>();
    }

    public void Play(int clipIndex = -1)
    {
        if (!canPlay)
        {
            return;
        }

        GameManager.Instance.Timer.Add(() =>
        {
            canPlay = true;
        }, delayBetweenClips);

        canPlay = false;

        if (clips.Length == 0)
        {
            Debug.Log("No audio clips found on " + name + ". No clip played.", this);
            return;
        }


        if (clipIndex != -1)
        {
            if (clipIndex < clips.Length)
            {
                source.PlayOneShot(clips[clipIndex]);
            }
            else
            {
                Debug.LogError("Could not play Audio Clip - Index out of range. Clips length: " + clips.Length + ", selected index: " + clipIndex, this);
            }
        }
        else
        { 
            source.PlayOneShot(clips[Random.Range(0, clips.Length)]);
        }
    }
}
