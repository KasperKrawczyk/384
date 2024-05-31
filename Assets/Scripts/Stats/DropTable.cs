using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "DropTable")]
public class DropTable : ScriptableObject
{
    public List<DropStatInfo> dropStatInfoList = new List<DropStatInfo>();

}


[System.Serializable]
public class DropStatInfo
{
    public string itemId;
    public float prob;
    public int qty;
    public DropStatInfo inside;
}
