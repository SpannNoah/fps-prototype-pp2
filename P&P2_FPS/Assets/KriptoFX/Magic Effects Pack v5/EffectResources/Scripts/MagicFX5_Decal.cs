using UnityEngine;

namespace MagicFX5
{
   // [ExecuteAlways]
    public class MagicFX5_Decal : MonoBehaviour
    {
        public bool  UsePropertyBlock = true;
        public bool  AutoRaycast      = false;
        public float RaycastLength    = 1;
        public float RaycastOffset    = 0.15f;

        private Renderer  _rend;

        private bool    _initialized;


        //I can't use awake initialistion because "domain reloading" and ExecuteAlways doesnt work together
        private void Initialize()
        {
            //transform.localPosition = Random.insideUnitSphere * 0.01f;
            _rend        = GetComponent<Renderer>();
            _initialized = true;
        }

        void OnEnable()
        {
            if (!_initialized) Initialize();

            if (UsePropertyBlock)
            {
                _rend.GetPropertyBlock(MagicFX5_CoreUtils.SharedMaterialPropertyBlock);
                _rend.SetPropertyBlock(MagicFX5_CoreUtils.SharedMaterialPropertyBlock);
            }
          

            if (AutoRaycast && Application.isPlaying)
            {
                transform.localPosition = Vector3.zero;
                var pos = transform.position;
                pos.y += RaycastLength;
                if (Physics.Raycast(transform.position, Vector3.down, out var hit, RaycastLength * 2))
                {
                    pos.y              = hit.point.y + RaycastOffset;
                    transform.position = pos;
                }
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
