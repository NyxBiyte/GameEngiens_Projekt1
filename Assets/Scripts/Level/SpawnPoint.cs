using UnityEngine;


public class SpawnPoint : MonoBehaviour
{
     void Awake()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.playerSpawnPoint = transform;
        else
            Debug.LogWarning("SpawnPoint: no GameManager.Instance found.", this);
    }

    void Start()
    {
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.transform.position = transform.position;
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null &&
            GameManager.Instance.playerSpawnPoint == transform)
            GameManager.Instance.playerSpawnPoint = null;
    }

    public Transform GetSpawnPoint() => transform;

}