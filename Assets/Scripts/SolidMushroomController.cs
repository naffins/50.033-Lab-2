using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SolidMushroomController : MonoBehaviour
{

    private const float movementSpeed = 5F;

    private new Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;
    private bool isMovingRight;
    private bool isMoving;
    private Vector3 movementIncrement;

    private Collider2D initialIgnoredCollider;
    private float initialIgnoreTimer;
    
    void Awake() {
        rigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        isMoving = true;
        isMovingRight = (new System.Random().Next() % 2 == 1);
        movementIncrement = new Vector3(movementSpeed*Time.fixedDeltaTime,0F,0F);
    }

    void FixedUpdate()
    {
        if (!isMoving) return;
        transform.position += (isMovingRight? 1F : -1F) * movementIncrement;
    }

    public void OnChildTriggerEnter2D(Collider2D other) {
        if ((!other.gameObject.CompareTag(Utils.PlayerTag)) && (!other.gameObject.CompareTag(Utils.TerrainTag))) return;
        if (other.gameObject.CompareTag(Utils.PlayerTag)) {
            if (isMoving) {
                isMoving = false;
                return;
            }
            isMoving = true;
            isMovingRight = other.gameObject.transform.position.x < transform.position.x;
            return;
        }
        isMovingRight = !isMovingRight;
    }

    public void SetInitializingParameters(GameObject other, float ignoreTimer) {
        initialIgnoredCollider = other.GetComponent<Collider2D>();
        initialIgnoreTimer = ignoreTimer;
        Physics2D.IgnoreCollision(boxCollider2D,initialIgnoredCollider);
        StartCoroutine("UnignoreSpawner");
    }

    private IEnumerator UnignoreSpawner() {
        yield return new WaitForSeconds(initialIgnoreTimer);
        Physics2D.IgnoreCollision(boxCollider2D,initialIgnoredCollider,false);
    }

    private void OnBecameInvisible() {
        Destroy(this.gameObject);
    }
}
