using UnityEngine;

public class ClimbableManager : MonoBehaviour
{
    [SerializeField] GameObject startCollider, endCollider;
    [SerializeField] float ClimbingSpeed = 0.5f;

    // At the Start of the Game send to the Colliders the start and end point of the Climbable
    public void SetUpPlayer(GameObject Player, bool isDown)
    {
        Character playerMovement = Player.GetComponent<Character>();
        playerMovement.SetClimbableData(true, startCollider.transform.position, endCollider.transform.position, isDown, ClimbingSpeed);
    }

    public void PlayerOffClimbable(GameObject Player)
    {
        Character playerMovement = Player.GetComponent<Character>();
        playerMovement.OffClimbable();
    }
}