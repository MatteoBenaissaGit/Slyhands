using System;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    public UnityEngine.Camera CardCamera;
    private RaycastHit[] _hits = new RaycastHit[20];
    
    public GameObject CardSelected;

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

    private void Update()
    {
        if (CardSelected == null)
        {
            Ray r = CardCamera.ScreenPointToRay(Input.mousePosition);

            int hits = Physics.RaycastNonAlloc(r, _hits);
            
            for (int i = 0; i < hits; i++)
            {
                if (_hits[i].collider.TryGetComponent(out Card card) && _hits[i].collider.gameObject != CardSelected)
                {
                    Debug.Log("Card Detect !");
                    CardSelected = _hits[i].collider.gameObject;
                }
            }
        }
    }
}
