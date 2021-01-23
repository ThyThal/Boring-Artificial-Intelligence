using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesController : MonoBehaviour
{
    [SerializeField] private List<Node> _nodesList;

    private void Start()
    {
        GameManager.Instance.nodesList = _nodesList;
    }
}
