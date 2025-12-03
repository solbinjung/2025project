using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "¸¶À»";

    void Start()
    {
        UIManager.Instance.LoadSceneWithLoadingScreen(firstSceneName);
    }
}