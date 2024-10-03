using UnityEngine;

public enum CardStatus
{
    InHand = 0,
    InHandHovered = 1,
    InHandMinor = 2,
    Dragged = 3,
    Discarded = 4
}

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    
    [field: SerializeField] public GameplayDeckManager GameplayDeckManager { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already another Card Manager in this scene !");
        }
    }
}