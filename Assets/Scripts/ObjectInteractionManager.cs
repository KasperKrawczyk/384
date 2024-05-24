using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ObjectInteractionManager : MonoBehaviour, IClickable
{

    public event Action<InputAction.CallbackContext> OnInteractableClick;


    public void OnClick(InputAction.CallbackContext context)
    {
        Debug.Log("OnClick in ObjectInteractionManager of " + gameObject.name);
        OnInteractableClick?.Invoke(context);

    }


}