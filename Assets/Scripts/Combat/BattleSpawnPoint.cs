using UnityEngine;

public class BattleSpawnPoint : MonoBehaviour
{
  public Character Spawn(Character character)
    {
        Character characterToSPawn = Instantiate<Character>(character, this.transform);
        return characterToSPawn;
    }
}
