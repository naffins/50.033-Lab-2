using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    private const float scrollAccelerateTime = 0.5F;
    private const float distanceTolerance = 2F;
    private const float distanceThreshold = 10F;

    private float permMinX, permMaxX, minX, maxX, viewportHalfWidth, maxSpeedIncrement, distanceRange;
    private Transform player;

    void Start()
    {
        GameObject g = GameObject.FindGameObjectWithTag(Utils.HorizontalBoundsTag);

        permMinX = float.MaxValue;
        permMaxX = float.MinValue;

        foreach (Transform t in g.transform) {
            permMinX = Math.Min(permMinX,t.position.x);
            permMaxX = Math.Max(permMaxX,t.position.x);
        }

        permMinX -= distanceTolerance - 0.5F;
        permMaxX += distanceTolerance - 0.5F;

        UpdateDisplayParams();

        GameObject playerObject = GameObject.FindGameObjectWithTag(Utils.PlayerTag);

        player = playerObject.transform;
        maxSpeedIncrement = playerObject.GetComponent<PlayerController>().horizontalMaxSpeed;

        transform.position = new Vector3(player.position.x,transform.position.y,transform.position.z);

        distanceRange = distanceThreshold - distanceTolerance;
    }

    void Update()
    {
        UpdateDisplayParams();
        TrackPlayer();
    }

    private void UpdateDisplayParams() {
        viewportHalfWidth = transform.position.x - Camera.main.ViewportToWorldPoint(new Vector3(0F,0f,0f)).x;

        minX = permMinX + viewportHalfWidth;
        maxX = permMaxX - viewportHalfWidth;
    }

    private void TrackPlayer() {

        float targetX = player.position.x;
        targetX = Math.Min(Math.Max(targetX,minX),maxX);

        float distance = Math.Min(Math.Abs(targetX-transform.position.x),distanceThreshold);

        if (distance <= distanceTolerance) {
            return;
        }
        
        float targetChange = (distance - distanceTolerance) / distanceRange * maxSpeedIncrement * Time.deltaTime;
        targetChange = (float)Math.Sqrt(targetChange);
        
        transform.position = transform.position + new Vector3((targetX>=transform.position.x? 1F : -1F) * targetChange,0F,0F);
    }
}
