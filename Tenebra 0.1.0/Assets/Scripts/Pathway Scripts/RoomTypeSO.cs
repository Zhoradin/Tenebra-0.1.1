using UnityEngine;

[CreateAssetMenu(fileName = "RoomType", menuName = "Map/RoomType")]
public class RoomTypeSO : ScriptableObject
{

    

    public RoomType roomType; // Chosen in unity inspector
    public string description; // A detailed description for what that type of room contains

    //public string TypeName;    // Name of the room type
    
}

public enum RoomType
    {
        None,
        Monster,
        Event,
        EliteMonster,
        RestSite,
        Merchant,
        Treasure,
        Boss
    }