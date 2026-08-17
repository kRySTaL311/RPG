using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public BoxCollider2D boxEnvironment;
    private float halfHeight;
    private float halfWidth;

    void Start()
    {
        halfHeight = Camera.main.orthographicSize;
        halfWidth = halfHeight * Camera.main.aspect;
    }

    void Update()
    {
        if (player != null)
        {
            transform.position=new Vector3(Mathf.Clamp(player.transform.position.x, boxEnvironment.bounds.min.x + halfWidth, boxEnvironment.bounds.max.x - halfWidth),
                                             Mathf.Clamp(player.transform.position.y, boxEnvironment.bounds.min.y + halfHeight, boxEnvironment.bounds.max.y - halfHeight),
                                             transform.position.z);
        }
    }
}
