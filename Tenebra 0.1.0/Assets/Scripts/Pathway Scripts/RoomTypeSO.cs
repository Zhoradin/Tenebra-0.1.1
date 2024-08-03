using UnityEngine;

[CreateAssetMenu(fileName = "RoomType", menuName = "Map/RoomType")]
public class RoomTypeSO : ScriptableObject
{
    public string TypeName;    // Name of the room type
    public Color RoomColor;    // Color used for visualization
}
