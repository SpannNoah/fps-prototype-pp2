using UnityEngine;


namespace MagicFX5
{
    [ExecuteAlways]
    public class MagicFX5_CommandBufferDistortion : MonoBehaviour
    {

        void OnEnable()
        {
            MagicFX5_GlobalUpdate.CreateInstanceIfRequired();
            MagicFX5_GlobalUpdate.DistortionInstances.Add(this);

        }

        void OnDisable()
        {
            MagicFX5_GlobalUpdate.DistortionInstances.Remove(this);
        }
    }
}