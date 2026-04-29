using UnityEngine;

public class PlatformCarry : MonoBehaviour
{
    private Vector3 lastPosition;

    void Start()
    {
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = transform.position - lastPosition;

        foreach (var obj in ridingObjects)
        {
            if (obj != null)
            {
                obj.position += delta;
            }
        }

        lastPosition = transform.position;
    }

    private System.Collections.Generic.List<Transform> ridingObjects = new System.Collections.Generic.List<Transform>();

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!ridingObjects.Contains(collision.transform))
                ridingObjects.Add(collision.transform);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ridingObjects.Remove(collision.transform);
        }
    }
}