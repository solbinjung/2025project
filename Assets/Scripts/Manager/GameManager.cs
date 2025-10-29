using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Persistent References")]
    public List<GameObject> persistentObjects = new List<GameObject>();

    private EventSystem currentEventSystem;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            currentEventSystem = FindObjectOfType<EventSystem>();

            if (currentEventSystem != null)
            {
                DontDestroyOnLoad(currentEventSystem.gameObject);
                Debug.Log("[GameManager] EventSystem을 DontDestroyOnLoad 처리했습니다.");
            }
            else
            {
                Debug.LogError("[GameManager] 시작 씬에 EventSystem이 없습니다! UI가 작동하지 않을 수 있습니다.");
            }

            foreach (var obj in persistentObjects)
            {
                if (obj != null)
                    DontDestroyOnLoad(obj);
            }

            // 씬 로드 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[GameManager] Scene Loaded: {scene.name}");

        EventSystem[] sceneEventSystems = FindObjectsOfType<EventSystem>();

        if (sceneEventSystems.Length > 1)
        {
            foreach (EventSystem es in sceneEventSystems)
            {
                if (es != currentEventSystem)
                {
                    Debug.LogWarning($"[GameManager] 새로 로드된 씬 '{scene.name}'에서 중복 EventSystem을 제거합니다.");
                    Destroy(es.gameObject);
                }
            }
        }

        // 태그 기반으로 SpawnPoint 찾기
        GameObject spawn = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawn != null && persistentObjects.Count > 0 && persistentObjects[0] != null)
        {
            GameObject player = persistentObjects[0]; // 리스트 첫 번째를 Player라고 가정
            player.transform.position = spawn.transform.position;
            player.transform.rotation = spawn.transform.rotation;

            // 이동 관련 초기화
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // 입력 잠깐 막기
            PlayerController controller = player.GetComponent<PlayerController>();
            if (controller != null)
            {
                controller.StopMovement();
                controller.CanControl = false;
                StartCoroutine(ReenableControl(controller, 3f));
            }
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private IEnumerator ReenableControl(PlayerController controller, float delay)
    {
        Debug.Log("[GameManager] 캐릭터 입력 재활성화 대기 시작");
        yield return new WaitForSeconds(delay);

        if (controller != null)
        {
            controller.CanControl = true;
            Debug.Log("[GameManager] 캐릭터 입력 가능해짐");
        }
    }
}

