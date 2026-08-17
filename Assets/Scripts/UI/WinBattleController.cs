using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinBattleController : MonoBehaviour
{
    public static WinBattleController instance;
    public Image rewardItemImage;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI goldText;
    private void Awake()
    {
        instance = this;
    }
    public void ShowReward(ItemData item, int exp, int gold)
    {
        if (item != null)
        {
            rewardItemImage.sprite = item.icon;
        }
        else
        {
            rewardItemImage.enabled = false;
        }

        if (expText != null)
            expText.text = $"EXP Gained: +{exp}";
        if (goldText != null)
            goldText.text = $"Gold Gained: +{gold}";
    }
}
