using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class RepeatableBlockController : MonoBehaviour {

    public float bounceConstant;

    private const string staticSectionName = "Static Section";
    private const string dynamicSectionName = "Dynamic Section";
    private const float dynamicSectionReturnTolerance = 0.01F;

    private GameObject staticSectionGameObject, dynamicSectionGameObject;
    private Rigidbody2D dynamicSectionRigidbody2D;
    private EdgeCollider2D dynamicSectionEdgeCollider2D;
    private Vector2 bounceForce;

    public void Awake() {
        bounceForce = new Vector2(0F,bounceConstant);
    }

    public void Start()
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
        dynamicSectionEdgeCollider2D = dynamicSectionGameObject.GetComponent<EdgeCollider2D>();
    }

    public void OnChildTriggerEnter2D(Collider2D other) {
        if (other.gameObject.name!=PlayerController.topCheckerName) return;
        dynamicSectionEdgeCollider2D.enabled = false;
        dynamicSectionRigidbody2D.AddForce(bounceForce,ForceMode2D.Impulse);
        OnTriggerDynamic();
        StartCoroutine("OnDynamicSectionReturnCoroutine");
    }

    private bool HasDynamicSectionReturned() {
        return (Math.Abs(dynamicSectionGameObject.transform.position.y-transform.position.y) <= dynamicSectionReturnTolerance)
            && (dynamicSectionRigidbody2D.velocity.y < 0F);
    }

    protected abstract void OnTriggerDynamic();

    protected abstract void OnDynamicSectionReturn();

    IEnumerator OnDynamicSectionReturnCoroutine() {
        if (!HasDynamicSectionReturned()) yield return new WaitUntil(()=>HasDynamicSectionReturned());
        dynamicSectionEdgeCollider2D.enabled = true;
        OnDynamicSectionReturn();
    }

}