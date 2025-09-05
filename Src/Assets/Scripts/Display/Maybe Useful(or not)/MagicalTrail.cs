using UnityEngine;

public class MagicalTrail : MonoBehaviour
{

    public float distance=10.0f;
    public Vector3 offset=Vector3.zero;
    void Update()
    { 
        Ray ray =Camera.main.ScreenPointToRay(Input.mousePosition);
        Vector3 newposition = ray.GetPoint(distance);
        transform.position = newposition+offset;
    }
}
