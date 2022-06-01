using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidMushroomQuestionBlockDynamicSectionController : MonoBehaviour
{

    private SolidMushroomQuestionBoxController questionBoxController;

    void Start()
    {
        questionBoxController = transform.parent.gameObject.GetComponent<SolidMushroomQuestionBoxController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        questionBoxController.OnChildTriggerEnter2D(other);
    }
}
