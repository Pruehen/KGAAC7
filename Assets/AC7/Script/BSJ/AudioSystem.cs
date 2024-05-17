using UnityEngine;

[ExecuteInEditMode]
public class AudioSystem : MonoBehaviour
{
    public bool playAudio = false;

    private AudioSource audioSource;

    void OnValidate()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        // Check if the playAudio flag is set to true
        if (playAudio)
        {
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }

            // Reset the playAudio flag
            playAudio = false;
        }
    }
}