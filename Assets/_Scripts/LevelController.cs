using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] public float minionsAmount = 4f;
    [SerializeField] private List<Node> _nodesList;

    [Header("Orange Team")]
    [SerializeField] private Transform orangeBoss;
    [SerializeField] private GameObject orangeMinion;

    [Header("Green Team")]
    [SerializeField] private Transform greenBoss;
    [SerializeField] private GameObject greenMinion;

    private void Start()
    {
        GameManager.Instance.nodesList = _nodesList;
        GameManager.Instance.levelController = this;
        GenerateTeams();
    }

    public void GenerateTeams()
    {
        GameManager.Instance.greenKilledAmount = minionsAmount + 1;
        GameManager.Instance.orangeKilledAmount = minionsAmount + 1;

        for (int i = 0; i < minionsAmount; i++)
        {
            var green = Instantiate(greenMinion, greenBoss);
            Vector3 random = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
            green.transform.position += random;

            var orange = Instantiate(orangeMinion, orangeBoss);
            random = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
            orange.transform.position += random;
        }

        GameObject[] greens = GameObject.FindGameObjectsWithTag("GreenEnemy");
        GameObject[] oranges = GameObject.FindGameObjectsWithTag("OrangeEnemy");

        foreach (var item1 in greens)
        {
            var minion1 = item1.GetComponent<MinionController>();
            minion1.GenerateLists();
        }

        foreach (var item2 in oranges)
        {
            var minion2 = item2.GetComponent<MinionController>();
            minion2.GenerateLists();
        }
    }
}
