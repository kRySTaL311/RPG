using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogController : MonoBehaviour
{
    [Header("Player Info")]
    public Sprite playerAvatar;
    public string playerName;

    [Header("UI Elements")]
    public TextMeshProUGUI dialogText;
    public TextMeshProUGUI leftName;
    public TextMeshProUGUI rightName;
    public GameObject rightNameBox;
    public GameObject leftNameBox;
    public GameObject dialogBox;
    public Image rightIcon;
    public Image leftIcon;

    [Header("Dialog Settings")]
    public Color npcTextColor = Color.green;
    public Color playerTextColor = Color.white;
    public Color iconColorGrey = Color.gray;
    public Color iconColorNormal = Color.white;
    public KeyCode advanceKey = KeyCode.E;
    public float typeSpeed = 0.05f;

    [Header("Debug")]
    public DialogEntry[] dialog;

    private int currentLine;
    private bool justStarted;
    private AudioSource audioSource;
    private Coroutine typingCoroutine;

    public static DialogController Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!dialogBox.activeInHierarchy) return;

        if (Input.GetKeyUp(advanceKey))
        {
            if (justStarted)
            {
                justStarted = false;
                return;
            }

            currentLine++;

            if (currentLine >= dialog.Length)
            {
                CloseDialog();
            }
            else
            {
                DisplayCurrentLine();
            }
        }
    }

    public void ShowDialog(DialogEntry[] dialogLines, Sprite npcSprite, string npcName)
    {
        if (dialogLines == null || dialogLines.Length == 0) return;

        Time.timeScale = 0;
        dialog = dialogLines;
        currentLine = 0;

        rightName.text = npcName;
        rightIcon.sprite = npcSprite;
        leftIcon.sprite = playerAvatar;
        leftName.text = playerName;

        dialogBox.SetActive(true);
        justStarted = true;

        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        if (currentLine >= dialog.Length) return;

        SetupSpeakerUI(dialog[currentLine]);

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeDialog(dialog[currentLine].message));

        PlayDialogSound(dialog[currentLine]);
    }

    private IEnumerator TypeDialog(string sentence)
    {
        dialogText.text = "";

        foreach (char letter in sentence)
        {
            dialogText.text += letter;
            yield return new WaitForSecondsRealtime(typeSpeed);
        }
    }

    private void SetupSpeakerUI(DialogEntry entry)
    {
        bool isPlayer = entry.speaker == Speaker.Player;

        leftNameBox.SetActive(isPlayer);
        rightNameBox.SetActive(!isPlayer);

        leftIcon.color = isPlayer ? iconColorNormal : iconColorGrey;
        rightIcon.color = !isPlayer ? iconColorNormal : iconColorGrey;

        dialogText.color = isPlayer ? playerTextColor : npcTextColor;
    }

    private void PlayDialogSound(DialogEntry entry)
    {
        if (entry.hasSound && entry.soundClip != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(entry.soundClip);
        }
    }

    private void CloseDialog()
    {
        dialogBox.SetActive(false);
        Time.timeScale = 1;
    }
}

public enum Speaker
{
    Player,
    NPC
}

[System.Serializable]
public class DialogEntry
{
    public Speaker speaker;

    [TextArea(3, 10)]
    public string message;

    public bool hasSound;
    public AudioClip soundClip;
}
