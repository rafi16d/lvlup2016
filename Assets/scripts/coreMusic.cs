using UnityEngine;
using System.Collections;
[RequireComponent(typeof(AudioSource))]
public class coreMusic : MonoBehaviour {

    public AudioClip core0;
    public AudioClip core1;
    public AudioClip core2;
    public AudioClip core3;

    private int counterCore = 0;
    private AudioSource audio;

	void Start () {
        this.audio = GetComponent<AudioSource>();
        this.audio.clip = core0;
        this.audio.loop = true;
        this.audio.Play();
    }

    public void upgradeMusic()
    {
        this.audio.Stop();
        switch (++this.counterCore) {
            case 1:
                this.audio.clip = core1;
                this.audio.loop = true;
                break;
            case 2:
                this.audio.clip = core2;
                this.audio.loop = true;
                break;
            case 3:
                this.audio.clip = core3;
                this.audio.loop = true;
                break;
        }
        this.audio.Play();
    }

    public void stopMusic() {
        this.audio.Stop();
    }
}
