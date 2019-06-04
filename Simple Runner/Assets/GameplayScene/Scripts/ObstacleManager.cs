using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour, Pausable {
    [SerializeField] private GameObject obstacleTemplatesGameObject;
    [SerializeField] private float obstacleSpawnDistance = 6f;
    [SerializeField] private float obstacleSpawnXcoord = 10f;
    [SerializeField] private float obstacleDisableBorder = -15f;
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private int obstacleDuplicatesCount = 2;
    private bool startDelayed;
    private bool paused;
    private float lastSpawnTime;
    private float timeTillNextObstacle;
    private float pauseTime;

    private GameCoordinator gameCoordinator;
    private List<GameObject> obstacleTemplates;

    private void OnEnable() {
        GameCoordinator.OnPause += Pause;
        GameCoordinator.OnUnpause += Unpause;
    }

    private void OnDisable() {
        GameCoordinator.OnPause -= Pause;
        GameCoordinator.OnUnpause -= Unpause;
    }

    private void Start() {
        if (obstacleTemplatesGameObject == null)
            Debug.LogError(gameObject.name + ": obstacles templates object not stated.", gameObject);
        gameCoordinator = GameCoordinator.Coordinator;
        if (gameCoordinator == null)
            Debug.LogError(gameObject.name + ": couldn't get GameCoordinator reference.", gameObject);
        InitializeObstacleTemplatesList();
        startDelayed = true;
        paused = false;
        lastSpawnTime = 0f;
        timeTillNextObstacle = 0f;
        StartCoroutine(DelayedStart(startDelay));
    }

    private void Update() {
        if (paused) return;
        if (startDelayed) return;
        if (Time.time - lastSpawnTime >= timeTillNextObstacle) {
            SpawnNewObstacle();
            lastSpawnTime = Time.time;
        }
    }

    private void InitializeObstacleTemplatesList() {
        obstacleTemplates = new List<GameObject>();
        Transform obstacleTemplatesObjectTransform = obstacleTemplatesGameObject.transform;
        int initialObstacleTemplatesCount = obstacleTemplatesObjectTransform.childCount;
        for (int i = 0; i < initialObstacleTemplatesCount; i++) {
            GameObject child = obstacleTemplatesObjectTransform.GetChild(i).gameObject;
            obstacleTemplates.Add(child);
            for (int j = 0; j < obstacleDuplicatesCount - 1; j++) {
                GameObject clone = Instantiate(child, obstacleTemplatesObjectTransform);
                obstacleTemplates.Add(clone);
            }
        }
        foreach (GameObject obstacleTemplate in obstacleTemplates) {
            Obstacle obstacle = obstacleTemplate.GetComponent<Obstacle>();
            obstacle.Init();
            obstacle.SetDisableBorder(obstacleDisableBorder);
        }
    }

    private void SpawnNewObstacle() {
        // Choose random inactive obstacle to spawn
        GameObject obstacleToSpawn;
        int obstacleChoice = Random.Range(0, obstacleTemplates.Count);
        for (int i = 0;; i++) {
            if (i >= 100) {
                Debug.LogError(gameObject.name + ": couldn't find inactive obstacle to spawn.", gameObject);
                return;
            }
            if (!obstacleTemplates[obstacleChoice].activeSelf) {
                obstacleToSpawn = obstacleTemplates[obstacleChoice];
                break;
            } else {
                obstacleChoice++;
                if (obstacleChoice >= obstacleTemplates.Count) obstacleChoice = 0;
            }
        }
        if (obstacleToSpawn == null) {
            Debug.LogError(gameObject.name + ": couldn't find obstacle to spawn.", gameObject);
            return;
        }
        // Spawn obstacle
        Obstacle obstacle = obstacleToSpawn.GetComponent<Obstacle>();
        if (obstacle == null) {
            Debug.LogError(gameObject.name + ": couldn't find obstacle script in " + obstacleToSpawn.name, gameObject);
            return;
        }
        float obstacleSpeed = gameCoordinator.GetGameSpeed();
        obstacle.SetMoveSpeed(obstacleSpeed);
        obstacle.SetXcoord(obstacleSpawnXcoord);
        if (Random.Range(0, 1) == 1) obstacle.Flip();
        obstacle.Flip();
        timeTillNextObstacle = (obstacleSpawnDistance + obstacle.GetWidth()) / obstacleSpeed;
        obstacleToSpawn.SetActive(true);
    }

    private IEnumerator DelayedStart(float delay) {
        yield return new WaitForSeconds(delay);
        startDelayed = false;
    }

    public void Pause() {
        pauseTime = Time.time;
        foreach (GameObject obstacle in obstacleTemplates)
            if (obstacle.activeSelf)
                obstacle.GetComponent<Obstacle>().Pause();
        paused = true;
    }

    public void Unpause() {
        foreach (GameObject obstacle in obstacleTemplates)
            if (obstacle.activeSelf)
                obstacle.GetComponent<Obstacle>().Unpause();
        float timeOnPause = Time.time - pauseTime;
        timeTillNextObstacle += timeOnPause;
        paused = false;
    }
}