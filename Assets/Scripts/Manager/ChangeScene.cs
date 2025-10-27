using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneToLoad;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!triggered && other.CompareTag("Player"))
            {
                Debug.Log("¾À º¯°æ");
                triggered = true;
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}