using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DropTable", menuName = "DropTable")]
public class DropTable : ScriptableObject
{
    public List<DropStatInfo> dropStatInfoList = new List<DropStatInfo>();

}