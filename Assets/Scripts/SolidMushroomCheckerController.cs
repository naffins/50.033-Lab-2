using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidMushroomCheckerController : MonoBehaviour
{
    
    private SolidMushroomController solidMushroomController;

    void Start()
    {
        solidMushroomController = transform.parent.gameObject.GetComponent<SolidMushroomController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        solidMushroomController.OnChildTriggerEnter2D(other);
    }
}
