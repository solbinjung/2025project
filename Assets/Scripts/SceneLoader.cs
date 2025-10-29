using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Inspector에서 첫 게임 플레이 씬("마을")의 이름을 입력
    [SerializeField] private string firstSceneName = "마을";

    void Start()
    {
        // 이 스크립트가 시작되자마자 지정된 씬을 로드
        SceneManager.LoadScene(firstSceneName);
    }
}