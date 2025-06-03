using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Persistent References")]
    public GameObject player;
    public GameObject mainCamera;
    public GameObject virtualCamera;
    public GameObject uiRoot;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (player != null)
                DontDestroyOnLoad(player);

            if (mainCamera != null)
                DontDestroyOnLoad(mainCamera);

            if (virtualCamera != null)
                DontDestroyOnLoad(virtualCamera);

            if (uiRoot != null)
                DontDestroyOnLoad(uiRoot);

            // 씬 로드 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject); // 중복 방지
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 내 SpawnPoint 찾기
        GameObject spawn = GameObject.Find("SpawnPoint");
        if (spawn != null && player != null)
        {
            player.transform.position = spawn.transform.position;
            player.transform.rotation = spawn.transform.rotation;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.StopMovement();
                controller.canControl = false;  // 3초간 입력 막기
                StartCoroutine(ReenableControl(controller, 3f)); // 3초 후 재활성화
            }
        }
    }
    void OnDestroy()
    {
        // 메모리 누수 방지
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    IEnumerator ReenableControl(PlayerController controller, float delay)
    {
        yield return new WaitForSeconds(delay);
        controller.canControl = true;
    }
}

