using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DVDEnemy : MonoBehaviour
{
    public Vector2 startDirection = new Vector2(1, 1);
    public float speed = 5f;

    void Start()
    {
        GetComponent<Rigidbody2D>().linearVelocity =
            startDirection.normalized * speed;
    }
}