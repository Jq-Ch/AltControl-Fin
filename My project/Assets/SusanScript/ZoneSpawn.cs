using UnityEngine;

public class ZoneSpawn : MonoBehaviour
{
    public Transform spawnPoint;   // 指向地面上用于生成的位置
    public string zoneTag;         // Zone 名字，如 "ZoneA"
    public bool isOccupied = false;
}
