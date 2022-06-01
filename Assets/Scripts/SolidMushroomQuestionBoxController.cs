using System.Collections;
using System;
using UnityEngine;

public class SolidMushroomQuestionBoxController : MonoBehaviour
{

    public float bounceConstant;
    public GameObject solidMushroomPrefab;

    private const string staticSectionName = "Static Section";
    private const string dynamicSectionName = "Dynamic Section";
    private const float dynamicSectionReturnTolerance = 0.01F;
    private const float initialSolidMushroomLaunchSpeed = 20F;
    private const float initialSpawnIgnoreTimer = 0.5F;

    private GameObject staticSectionGameObject, dynamicSectionGameObject;
    private Rigidbody2D dynamicSectionRigidbody2D;
    private bool hasBounced;
    private Vector2 bounceForce, initialSolidMushroomLaunchImpulse;

    void Awake() {
        hasBounced = false;

        bounceForce = new Vector2(0F,bounceConstant);
        initialSolidMushroomLaunchImpulse = new Vector2(0,initialSolidMushroomLaunchSpeed);
    }

    void Start()
    {
        foreach (Transform t in transform) {
            if (t.gameObject.name==staticSectionName) {
                staticSectionGameObject = t.gameObject;
            }
            else if (t.gameObject.name==dynamicSectionName) {
                dynamicSectionGameObject = t.gameObject;
            }
        }
        dynamicSectionRigidbody2D = dynamicSectionGameObject.GetComponent<Rigidbody2D>();
    }

    public void OnChildTriggerEnter2D(Collider2D other) {

        if (other.gameObject.name!=PlayerController.topCheckerName) return;
        if (!hasBounced) {
            hasBounced = true;
            dynamicSectionRigidbody2D.AddForce(bounceForce,ForceMode2D.Impulse);

            GameObject solidMushroom = Instantiate(solidMushroomPrefab,transform.position + new Vector3(0F,1F,0F),Quaternion.identity);
            SolidMushroomController solidMushroomController = solidMushroom.GetComponent<SolidMushroomController>();
            solidMushroomController.SetInitializingParameters(staticSectionGameObject,initialSpawnIgnoreTimer);

            solidMushroomController.GetComponent<Rigidbody2D>().AddForce(initialSolidMushroomLaunchImpulse,ForceMode2D.Impulse);
            
            StartCoroutine("OnDynamicSectionBounce");
        }
    }

    private bool HasDynamicSectionReturned() {
        return (Math.Abs(dynamicSectionGameObject.transform.position.y-transform.position.y) <= dynamicSectionReturnTolerance)
            && (dynamicSectionRigidbody2D.velocity.y < 0F);
    }

    IEnumerator OnDynamicSectionBounce() {
        if (!HasDynamicSectionReturned()) {
            yield return new WaitUntil(()=>HasDynamicSectionReturned());
        }

        staticSectionGameObject.GetComponent<SpriteRenderer>().enabled = true;

        dynamicSectionGameObject.SetActive(false);

    }
}
