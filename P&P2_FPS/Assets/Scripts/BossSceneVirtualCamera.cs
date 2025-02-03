using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossSceneVirtualCamera : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera CutSceneCamera;

    public PlayableDirector BossSceneTrack;
    // Start is called before the first frame update
    void Start()
    {
        CutSceneCamera.Priority = 10;
        BossSceneTrack.Play();
        BossSceneTrack.stopped += BossCutScene;

    }

    private void BossCutScene(PlayableDirector director)
    {
        CutSceneCamera.Priority = 0;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
