using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int selectedWeaponIndex;
    public List<WeaponSaveData> weaponInventory = new List<WeaponSaveData>();
}
