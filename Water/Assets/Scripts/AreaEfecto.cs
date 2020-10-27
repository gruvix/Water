using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AreaEfecto : MonoBehaviour
{
    [Range(0, 50)]
    public int segments = 20;
    [Range(0, 5)]
    private float xradius = 3;
    [Range(0, 5)]
    private float yradius = 3;
    LineRenderer line;

    private void Start()
    {
        // Elijo el radio como el alcance dividido la escala del sprite
        xradius = gameObject.transform.GetComponentInParent<TrashCollector>().alcance / gameObject.transform.parent.transform.localScale.x;
        yradius = xradius;
        Debug.Log(gameObject.transform.parent);
        line = gameObject.GetComponent<LineRenderer>();
        
        line.SetVertexCount(segments + 1);
        line.useWorldSpace = false;
        CreatePoints();
    }

    private void CreatePoints()
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * xradius;
            y = Mathf.Cos(Mathf.Deg2Rad * angle) * yradius;

            line.SetPosition(i, new Vector3(x, y, 0));

            angle += (360f / segments);
        }
    }
}
