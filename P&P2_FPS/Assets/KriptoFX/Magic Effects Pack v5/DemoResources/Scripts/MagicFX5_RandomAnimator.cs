using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicFX5_RandomAnimator : MonoBehaviour
{
    public Vector2 RandomRange    = new Vector2(0.8f, 1.2f);
    public string  FloatParameter = "Speed";


    void OnEnable()
    {
        GetComponent<Animator>().SetFloat(FloatParameter, Random.Range(RandomRange.x, RandomRange.y));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
