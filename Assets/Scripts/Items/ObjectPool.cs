using System.Collections.Generic;
using UnityEngine;

public class ObjectPool {
    private readonly Stack<GameObject> objects = new Stack<GameObject>();
    private readonly GameObject prefab;
    private readonly Transform parentTrasform;

    public ObjectPool(GameObject prefab) {
        this.prefab = prefab;
    }

    public ObjectPool(GameObject prefab, Transform parentTransform) {
        this.prefab = prefab;
        this.parentTrasform = parentTransform;
    }
    
    public GameObject Get() {
        if (objects.Count > 0) {
            GameObject obj = objects.Pop();
            obj.SetActive(true);
            return obj;
        } else {
            if (parentTrasform != null) {
                return GameObject.Instantiate(prefab, parentTrasform);
            }

            return GameObject.Instantiate(prefab);
        }
    }

    public void Return(GameObject obj) {
        obj.gameObject.SetActive(false);
        objects.Push(obj);
    }
}