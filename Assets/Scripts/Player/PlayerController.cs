using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public float speed;
    private Animator anim;
    private Rigidbody2D rb;

    private const string PrefX = "PlayerPosX";
    private const string PrefY = "PlayerPosY";

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        LoadSavedPosition();
    }
    void FixedUpdate()
    {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        rb.linearVelocity = movement * speed;

        anim.SetFloat("MoveX", rb.linearVelocity.x);
        anim.SetFloat("MoveY", rb.linearVelocity.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1 || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            anim.SetFloat("LastX", Input.GetAxisRaw("Horizontal"));
            anim.SetFloat("LastY", Input.GetAxisRaw("Vertical"));
        }
    }

    public void SavePosition()
    {
        Vector2 pos = transform.position;
        PlayerPrefs.SetFloat(PrefX, pos.x);
        PlayerPrefs.SetFloat(PrefY, pos.y);
        PlayerPrefs.Save();
    }

    private void LoadSavedPosition()
    {
        if (PlayerPrefs.HasKey(PrefX) && PlayerPrefs.HasKey(PrefY))
        {
            float x = PlayerPrefs.GetFloat(PrefX);
            float y = PlayerPrefs.GetFloat(PrefY);
            transform.position = new Vector2(x, y);
        }
    }

    public void ClearSavedPosition()
    {
        PlayerPrefs.DeleteKey(PrefX);
        PlayerPrefs.DeleteKey(PrefY);
        PlayerPrefs.Save();
    }
}
