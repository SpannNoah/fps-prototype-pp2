using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelData
{
    public int levelNumber;

    public LevelData(Portal currentLevel)
    {
        levelNumber = currentLevel.currentLevel;
    }
}
