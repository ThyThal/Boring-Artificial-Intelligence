using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] public List<Node> nodesList;
    [SerializeField] public float orangeAmount = 5f;
    [SerializeField] public float greenAmount = 5f;
    [SerializeField] public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (greenAmount <= 0) { Debug.Log("Orange Wins!"); greenAmount = 5f; SceneManager.LoadScene(0); }
        if (orangeAmount <= 0) { Debug.Log("Green Wins!"); orangeAmount = 5f; SceneManager.LoadScene(0);  }
    }
}
