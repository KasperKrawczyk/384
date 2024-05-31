// using System;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
//
// public class ContainerButtonManager : MonoBehaviour, IPointerClickHandler
// {
//
//     private ContainerItem _c;
//
//     [SerializeField] private Sprite full;
//     [SerializeField] private Sprite empty;
//
//     private void Awake()
//     {
//         _c = GetComponent<ContainerItem>();
//     }
//
//     public ContainerItem ContainerItem
//     {
//         get
//         {
//             return _c;
//         }
//     
//         set
//         {
//             if (value != null)
//             {
//                 GetComponent<Image>().sprite = full;
//             }
//             else
//             {
//                 GetComponent<Image>().sprite = empty;
//             }
//         }
//     }
//
//     public void OnPointerClick(PointerEventData eventData)
//     {
//         // if (_c != null)
//         // {
//         //     Debug.Log("OnPointerClick in " + name + " of " + _c.name);
//         //     _c.ToggleOpenClose();
//         // }
//     }
// }
