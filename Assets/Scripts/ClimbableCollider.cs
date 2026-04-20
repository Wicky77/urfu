using UnityEngine;

public class ClimbableCollider : MonoBehaviour
{
    [SerializeField] bool isDown;

    public GameObject W;
    public GameObject S;

    ClimbableManager climbableManager;

    void Awake()
    {
        climbableManager = GetComponentInParent<ClimbableManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            climbableManager.SetUpPlayer(other.gameObject, isDown);

            if (isDown)
                W.SetActive(true);
            else
                S.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            climbableManager.PlayerOffClimbable(other.gameObject);
        }

        if (isDown)
            W.SetActive(false);
        else
            S.SetActive(false);
    }
}