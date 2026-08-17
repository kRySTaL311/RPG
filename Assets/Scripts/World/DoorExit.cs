using UnityEngine;

public class DoorExit : MonoBehaviour
{
    public string transitionName;
    private string doorName;
    void Start()
    {

        if (PlayerPrefs.HasKey("Area"))
        {
            doorName = PlayerPrefs.GetString("Area");
        }
        if (transitionName == doorName)
        {
            PlayerController.instance.transform.position = gameObject.transform.position;
        }
    }
}
