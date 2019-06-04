using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Obstacle : MonoBehaviour, Pausable {
    private Rigidbody2D rBody;
    [SerializeField] private float moveSpeed;
    private float disableBorder;
    private Collider2D[] crashColliders;
    private Collider2D[] safeColliders;
    private float width;
    private Vector2 beforePauseVelocity = Vector2.zero;

    private void OnEnable() {
        if (rBody == null) Init();
        rBody.velocity = new Vector2(-moveSpeed, 0);
    }

    private void Update() {
        if (transform.position.x + width <= disableBorder) gameObject.SetActive(false);
    }

    public void Init() {
        rBody = GetComponent<Rigidbody2D>();
        if (rBody == null) Debug.LogError(gameObject.name + ": object doesn't have RigidBody2D component.", gameObject);
        rBody.velocity = new Vector2(-moveSpeed, 0);
        InitializeCollidersArrays();
        width = CalculateWidth();
    }

    private void InitializeCollidersArrays() {
        List<Collider2D> tempCrashList = new List<Collider2D>();
        List<Collider2D> tempSafeList = new List<Collider2D>();
        foreach (Transform child in transform) {
            foreach (Transform obstaclePart in child) {
                GameObject obstaclePartObject = obstaclePart.gameObject;
                if (obstaclePartObject.CompareTag("Crash"))
                    tempCrashList.Add(obstaclePartObject.GetComponent<BoxCollider2D>());
                else if (obstaclePartObject.CompareTag("Safe"))
                    tempSafeList.Add(obstaclePartObject.GetComponent<BoxCollider2D>());
            }
        }
        crashColliders = tempCrashList.ToArray();
        safeColliders = tempSafeList.ToArray();
        tempCrashList.Clear();
        tempSafeList.Clear();
    }

    private float CalculateWidth() {
        float leftmostX = transform.position.x + 100f;
        float rightmostX = transform.position.x - 100f;
        List<Collider2D> allColliders = new List<Collider2D>();
        bool activeState = gameObject.activeSelf;
        if (!activeState) gameObject.SetActive(true);
        rBody.GetAttachedColliders(allColliders);
        foreach (Collider2D collider in allColliders) {
            if (collider.bounds.min.x < leftmostX) leftmostX = collider.bounds.min.x;
            if (collider.bounds.max.x > rightmostX) rightmostX = collider.bounds.max.x;
        }
        gameObject.SetActive(activeState);
        return rightmostX - leftmostX;
    }

    public bool CheckIfCrashCollider(Collider2D collider) {
        return crashColliders.Contains(collider);
    }

    public void SetMoveSpeed(float newMoveSpeed) {
        moveSpeed = newMoveSpeed;
    }

    public void SetXcoord(float newXcoord) {
        transform.position = new Vector2(newXcoord, transform.position.y);
    }

    public void SetDisableBorder(float newBorder) {
        disableBorder = newBorder;
    }

    public float GetWidth() {
        return width;
    }

    public void Flip() {
        transform.Rotate(180f, 0f, 0f);
        transform.localScale = Vector3.Scale(transform.localScale, new Vector3(1, 1, -1));
    }

    public void Pause() {
        beforePauseVelocity = rBody.velocity;
        rBody.velocity = Vector2.zero;
    }

    public void Unpause() {
        rBody.velocity = beforePauseVelocity;
    }
}