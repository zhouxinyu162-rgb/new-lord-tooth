using UnityEngine;

public class ToothSound : MonoBehaviour
{
    public AudioClip soundA;
    public AudioClip soundB;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayRandomSound();
        }
    }

    void PlayRandomSound()
    {
        AudioClip clip =
            Random.value > 0.5f ? soundA : soundB;

        audioSource.PlayOneShot(clip);
    }
}