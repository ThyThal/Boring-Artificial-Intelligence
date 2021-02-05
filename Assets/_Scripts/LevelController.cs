using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] public float minionsAmount = 4f;
    [SerializeField] private List<Node> _nodesList;

    [Header("Orange Team")]
    [SerializeField] private GameObject orangeBoss;
    [SerializeField] private Transform orangeBossTransform;
    [SerializeField] private GameObject orangeMinion;
    [SerializeField] public List<GameObject> orangeTeamList = new List<GameObject>();

    [Header("Green Team")]
    [SerializeField] private GameObject greenBoss;
    [SerializeField] private Transform greenBossTransform;
    [SerializeField] private GameObject greenMinion;
    [SerializeField] public List<GameObject> greenTeamList = new List<GameObject>();

    private void Start()
    {
        GameManager.Instance.nodesList = _nodesList;
        GameManager.Instance.levelController = this;
        GenerateTeams();
    }

    public void GenerateTeams()
    {
        // Generate Amount to Kill
        GameManager.Instance.greenKilledAmount = minionsAmount + 1;
        GameManager.Instance.orangeKilledAmount = minionsAmount + 1;

        // Add Bosses to Lists
        greenTeamList.Add(greenBoss);
        orangeTeamList.Add(orangeBoss);

        // Generate Minions
        for (int i = 0; i < minionsAmount; i++)
        {
            // Gren Team Generator
            var green = Instantiate(greenMinion, greenBossTransform); // Create Minion
            greenTeamList.Add(green); // Add Minion to List

            Vector3 random = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
            green.transform.position += random; // Random Position

            // Orange Team Generator
            var orange = Instantiate(orangeMinion, orangeBossTransform); // Create Minion
            orangeTeamList.Add(orange); // Add Minion to List

            random = new Vector3(Random.Range(-2, 2), 0, Random.Range(-2, 2));
            orange.transform.position += random; // Random Position
        }

        for (int i = 0; i < greenTeamList.Count; i++)
        {
            var currentOrange = orangeTeamList[i].GetComponent<MinionController>();
            currentOrange.teamBoss = orangeBoss.GetComponent<MinionController>();
            currentOrange.allyMinionsList = orangeTeamList;
            currentOrange.enemiesMinionList = greenTeamList;

            var currentGreen = greenTeamList[i].GetComponent<MinionController>();
            currentGreen.teamBoss = greenBoss.GetComponent<MinionController>();
            currentGreen.allyMinionsList = greenTeamList;
            currentGreen.enemiesMinionList = orangeTeamList;

        }
    }
}
