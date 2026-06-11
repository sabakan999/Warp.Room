using UnityEngine;

public class spin : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 90f;

    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.deltaTime);
    }
}