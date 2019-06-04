using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBall : MonoBehaviour, Pausable {
    [SerializeField] private float maxFallSpeed = 100f;
    [SerializeField] private float maxDragSpeed = 1f;
    [SerializeField] private float dragForce = 10f;
    [SerializeField] private float additionalGravityForce = 15f;
    [SerializeField] private float leftCrashBorder = -8.5f;
    private Rigidbody2D rBody;
    private float defaultX;
    private float defaultY;
    private bool paused;
    private Vector2 beforePauseVelocity = Vector2.zero;

    private void OnEnable() {
        GameCoordinator.OnPause += Pause;
        GameCoordinator.OnUnpause += Unpause;
        InputHandler.onScreenTapEvent += FlipGravity;
    }

    private void OnDisable() {
        GameCoordinator.OnPause -= Pause;
        GameCoordinator.OnUnpause -= Unpause;
        InputHandler.onScreenTapEvent -= FlipGravity;
    }

    private void Start() {
        rBody = GetComponent<Rigidbody2D>();
        if (rBody == null) Debug.LogError(gameObject.name + ": object doesn't have RigidBody2D component.", gameObject);
        defaultX = transform.position.x;
        defaultY = transform.position.y;
    }

    private void FixedUpdate() {
        if (paused) return;
        if (transform.position.x <= leftCrashBorder) GameCoordinator.Coordinator.GameOver();
        if (transform.position.x < defaultX) {
            rBody.AddForce(transform.right * dragForce);
        } else if (transform.position.x > defaultX) {
            transform.position = new Vector2(defaultX, transform.position.y);
            rBody.velocity = new Vector2(0, rBody.velocity.y);
        }

        AddAdditionalGravityForce();
        LimitSpeed();
    }

    public void FlipGravity() {
        if (paused) return;
        rBody.gravityScale *= -1;
    }

    public void SetGravityDown() {
        if (paused) return;
        if (rBody.gravityScale < 0) rBody.gravityScale *= -1;
    }

    public void SetGravityUp() {
        if (paused) return;
        if (rBody.gravityScale > 0) rBody.gravityScale *= -1;
    }

    public void Respawn() {
        if (paused) return;
        transform.position = new Vector2(defaultX, defaultY);
        SetGravityDown();
    }

    private void LimitSpeed() {
        if (paused) return;
        if (Mathf.Abs(rBody.velocity.y) > maxFallSpeed)
            rBody.velocity = new Vector2(rBody.velocity.x, Mathf.Sign(rBody.velocity.y) * maxFallSpeed);
        if (rBody.velocity.x > maxDragSpeed) rBody.velocity = new Vector2(maxDragSpeed, rBody.velocity.y);
    }

    // Adding additional gravity force when ball is moving in opposite direction of gravity vector.
    private void AddAdditionalGravityForce() {
        if (paused) return;
        // Gravity field is down, ball is moving up
        if (rBody.gravityScale > 0 && rBody.velocity.y > 0) rBody.AddForce(transform.up * -additionalGravityForce);
        // Gravity field is up, ball is moving down
        if (rBody.gravityScale < 0 && rBody.velocity.y < 0) rBody.AddForce(transform.up * additionalGravityForce);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (paused) return;
        if (collision.gameObject.CompareTag("Obstacle")) {
            Obstacle obstacle = collision.gameObject.GetComponent<Obstacle>();
            bool crash = obstacle.CheckIfCrashCollider(collision.collider);
            if (crash) GameCoordinator.Coordinator.GameOver();
        }
    }

    public void Pause() {
        beforePauseVelocity = rBody.velocity;
        rBody.isKinematic = true;
        rBody.velocity = Vector2.zero;
        paused = true;
    }

    public void Unpause() {
        rBody.isKinematic = false;
        rBody.velocity = beforePauseVelocity;
        paused = false;
    }
}