using UnityEngine;

public class RepeatableBlockDynamicSectionController : MonoBehaviour
{

    private RepeatableBlockController repeatableBlockController;

    void Start()
    {
        repeatableBlockController = transform.parent.gameObject.GetComponent<RepeatableBlockController>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        repeatableBlockController.OnChildTriggerEnter2D(other);
    }
}