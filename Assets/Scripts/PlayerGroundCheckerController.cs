using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheckerController : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = transform.parent.gameObject.GetComponent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        playerController.onChildTriggerEnter2D(other);   
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        playerController.onChildTriggerExit2D(other);   
    }
}
