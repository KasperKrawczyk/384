// using System;
// using UnityEngine;
// using UnityEngine.EventSystems;
//
// public class ContainerUiToggle : MonoBehaviour, IPointerClickHandler
// {
//     public RectTransform containerTransform; 
//     public Vector2 minimizedSize = new Vector2(100, 20);
//     public Vector2 maximizedSize;
//
//     private void Awake()
//     {
//         maximizedSize = containerTransform.rect.size;
//     }
//
//     private bool isMinimized = false;
//
//     public void OnPointerClick(PointerEventData eventData)
//     {
//         if (eventData.clickCount == 2)
//         {
//             ToggleSize();
//         }
//     }
//
//     void ToggleSize()
//     {
//         containerTransform.sizeDelta = isMinimized ? maximizedSize : minimizedSize;
//         isMinimized = !isMinimized;
//     }
// }