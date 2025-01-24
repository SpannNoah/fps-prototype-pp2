using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace MagicFX5
{
    public class MagicFX5_TrailRendererFeatures : MagicFX5_IScriptInstance
    {
        public bool                 UseWorldSpaceUV = true;
        public MagicFX5_ShaderColorCurve ApplyColorCurveWhenStoped;
        public bool                 AutomaticEmittingStop = false;

        private Vector2  _uvOffset;
        private Vector3  _lastPos;
        private Renderer _renderer;

        private LineRenderer  _lineRenderer;
        private TrailRenderer _trailRenderer;

        int _shaderID = Shader.PropertyToID("_TrailWorldUvOffset");

        private       int _emittingStopCurrentFrames;
        private const int EmittingStopFramesThreshold = 10;

        internal override void OnEnableExtended()
        {
            if (_renderer      == null) _renderer      = GetComponent<Renderer>();
            if (_lineRenderer  == null) _lineRenderer  = GetComponent<LineRenderer>();
            if (_trailRenderer == null) _trailRenderer = GetComponent<TrailRenderer>();

            _uvOffset = Vector2.zero;
            _lastPos  = transform.position;

            if (ApplyColorCurveWhenStoped != null) ApplyColorCurveWhenStoped.enabled = false;

            if (AutomaticEmittingStop && _trailRenderer != null)
            {
                _emittingStopCurrentFrames   = 0;
                _trailRenderer.Clear();
                _trailRenderer.emitting      = true;
            }
        }

        internal override void OnDisableExtended()
        {
            

        }

        internal override void ManualUpdate()
        {
            var currentPos = transform.position;
            if (_lineRenderer != null && _lineRenderer.positionCount > 1)
            {
                currentPos = _lineRenderer.GetPosition(0);
            }

            var distance = Vector3.Distance(currentPos, _lastPos);
            _lastPos = currentPos;
           
            if (UseWorldSpaceUV)
            {
                _uvOffset.x -= distance;
                _renderer.SetVectorPropertyBlock(_shaderID, _uvOffset);
            }

            if (ApplyColorCurveWhenStoped != null) { ApplyColorCurveWhenStoped.enabled = distance < 0.001f; }

            if (AutomaticEmittingStop && _trailRenderer != null && distance < 0.001f)
            {
                _emittingStopCurrentFrames++;
                if (_emittingStopCurrentFrames > EmittingStopFramesThreshold) _trailRenderer.emitting = false;
            }
        }

      
    }
}
