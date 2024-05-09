using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private static InventoryManager _instance;
    
    // debug
    [SerializeField] private BaseItem[] items; 
    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<InventoryManager>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        // Debug.Log("items[0] = " + items[0]);
        // ContainerItem bag = (ContainerItem) Instantiate(items[0]);
        // bag.Initialise();
    }
}
