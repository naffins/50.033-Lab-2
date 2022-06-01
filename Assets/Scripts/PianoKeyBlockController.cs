using UnityEngine;

public class PianoKeyBlockController : RepeatableBlockController
{

    private AudioClip keyClip;
    

    private AudioSource audioSource;

    public new void Awake() {
        base.Awake();
        audioSource = GetComponent<AudioSource>();
    }

    public void SetKeyClip(AudioClip audioClip) {keyClip = audioClip;}

    protected override void OnTriggerDynamic() {
        audioSource.PlayOneShot(keyClip);    
    }

    protected override void OnDynamicSectionReturn() {}
    
}