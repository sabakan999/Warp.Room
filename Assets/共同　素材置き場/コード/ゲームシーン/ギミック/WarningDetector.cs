using System.Collections.Generic;
using UnityEngine;

public class WarningDetector : MonoBehaviour
{
    // 現在接触している枠
    private List<Collider2D> hitFrames = new();


    // 外部から取得する接触点
    public List<Vector3> GetHitPoints()
    {
        List<Vector3> points = new();


        foreach (Collider2D frame in hitFrames)
        {
            if (frame != null)
            {
                points.Add(
                    frame.ClosestPoint(transform.position)
                );
            }
        }


        return points;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("RoomFrame"))
        {
            if(!hitFrames.Contains(other))
            {
                hitFrames.Add(other);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("RoomFrame"))
        {
            if(hitFrames.Contains(other))
            {
                hitFrames.Remove(other);
            }
        }
    }


    public void Clear()
    {
        hitFrames.Clear();
    }

    public int HitCount()
{
    return hitFrames.Count;
}
}