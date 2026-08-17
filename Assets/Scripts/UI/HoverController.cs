using UnityEngine;
using UnityEngine.InputSystem;

public class HoverController : MonoBehaviour
{
    private Character lastHoveredCharacter;
    public static HoverController instance;

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        ProcessMouseInput();
    }
    private void ProcessMouseInput()
    {
        if (Mouse.current == null || Camera.main == null)
        {
            return;
        }

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hitInfo = Physics2D.Raycast(ray.origin, ray.direction);
        if (hitInfo.collider != null && hitInfo.collider.CompareTag("Character"))
        {
            Character character = hitInfo.collider.GetComponent<Character>();
            if(character != lastHoveredCharacter)
            {
                DeselectLastHovered();
                character.ShowHoverIndicator(true);
                lastHoveredCharacter =character;
            }
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                BattleController.instance.SelectCharacter(character);

            }

        }
        else
        {
            DeselectLastHovered();
        }
    }

    private void DeselectLastHovered()
    {
        if (lastHoveredCharacter != null)
        {
            lastHoveredCharacter.ShowHoverIndicator(false);
            lastHoveredCharacter=null;
        }
    }
}
