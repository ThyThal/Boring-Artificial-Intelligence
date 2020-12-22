using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] public List<Node> _neighbours;
    private void GetNeighbours(Vector3 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, 20f))
        {
            var node = hit.collider.GetComponent<Node>();
            if (node != null)
            {
                _neighbours.Add(node);
            }
        }
    } // Get Nodes With Raycast
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var item in _neighbours)
        {
            Vector3 line_finish = item.transform.position;
            line_finish.y += 2;
            Gizmos.DrawLine(transform.position, line_finish);
            Vector3 direction = (line_finish - transform.position).normalized;
            ForGizmo(line_finish, direction, Color.blue, false, 1f);
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1);
    } // Draw Nodes & Connections
    public void ForGizmo(Vector3 pos, Vector3 direction, Color? color = null, bool doubled = false, float arrowHeadLength = 0.2f, float arrowHeadAngle = 20.0f)
    {
        Gizmos.color = color ?? Color.white;

        //arrow shaft
        Gizmos.DrawRay(pos, direction);

        if (direction != Vector3.zero)
        {
            //arrow head
            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }
    }
}
