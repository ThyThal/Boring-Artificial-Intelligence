using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Main Information")]
    [SerializeField] public List<Node> nodesList;
    public LevelController levelController;
    public float orangeKilledAmount;
    public float greenKilledAmount;

    public static GameManager Instance;
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
        if (greenKilledAmount <= 0) {
            Debug.Log("Orange Wins!"); 
            SceneManager.LoadScene(0); }

        if (orangeKilledAmount <= 0) { 
            Debug.Log("Green Wins!"); 
            SceneManager.LoadScene(0); }
    }


}
