using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FloatingDamageText : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private Vector3 floatDirection = Vector3.up;

    private TextMeshProUGUI textMesh;
    private Color currentColor;
    private float timePassed;

    private void Awake()
    {
        textMesh = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        HandleMovement();
        HandleFadeOut();
    }

    public void Initialize(string text,Color color)
    {
        SetFormattedText(text);
        SetTextColor(color);
    }
    private void SetFormattedText(string text)
    {
        if (text.StartsWith("CRITICAL!"))
        {
            string[] lines = text.Split('\n');
            if (lines.Length > 1)
            {
                textMesh.text = $"<size=130%><b>{lines[0]}</b></size>\n{lines[1]}";
            }
            else
            {
                textMesh.text = $"<b>{text}</b>";
            }     
        }
        else
        {
            textMesh.text = text;
        }
    }

    private void SetTextColor(Color color)
    {
        currentColor = color;
        textMesh.color=currentColor;
    }

    private void HandleMovement()
    {
        transform.position += floatDirection * floatSpeed * Time.deltaTime;
    }
    private void HandleFadeOut()
    {
        timePassed += Time.deltaTime;
        currentColor.a = Mathf.Clamp01(1 - (timePassed * fadeSpeed));
        textMesh.color = currentColor;

        if (currentColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
