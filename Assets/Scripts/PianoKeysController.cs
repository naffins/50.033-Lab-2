using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoKeysController : MonoBehaviour
{

    public AudioClip[] audioClips;
    public GameObject pianoKeyBlockPrefab, inversePianoKeyBlockPrefab;
    public float interkeySpacing = 1F;

    void Start() {
        float requiredDistance = (audioClips.Length-1) * (1F + interkeySpacing);
        float currentX = transform.position.x - (requiredDistance/2);
        for (int i=0;i<audioClips.Length;i++) {
            float targetYIncrement;
            GameObject targetPrefab;
            switch(i%12) {
                case 1:
                case 3:
                case 5:
                case 8:
                case 10:
                    targetYIncrement = 0.5F;
                    targetPrefab = inversePianoKeyBlockPrefab;
                    break;
                default:
                    targetYIncrement = 0F;
                    targetPrefab = pianoKeyBlockPrefab;
                    break;
            }
            GameObject g = GameObject.Instantiate(targetPrefab,
                new Vector3(currentX,transform.position.y + targetYIncrement,transform.position.z),Quaternion.identity,this.transform);
            g.GetComponent<PianoKeyBlockController>().SetKeyClip(audioClips[i]);
            currentX += (1F + interkeySpacing);
        }
    }
}
