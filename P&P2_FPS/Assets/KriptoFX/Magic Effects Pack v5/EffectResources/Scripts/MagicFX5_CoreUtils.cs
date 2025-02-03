using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace MagicFX5
{
    public static class MagicFX5_CoreUtils
    {
        public static MaterialPropertyBlock SharedMaterialPropertyBlock = new MaterialPropertyBlock();
        static        List<Material>        _materialsDummy             = new List<Material>();

        public static void SetFloatPropertyBlock(this Renderer rend, int shaderNameID, float value)
        {
            rend.GetPropertyBlock(SharedMaterialPropertyBlock);
            SharedMaterialPropertyBlock.SetFloat(shaderNameID, value);
            rend.SetPropertyBlock(SharedMaterialPropertyBlock);
        }

        public static void SetColorPropertyBlock(this Renderer rend, int shaderNameID, Color value)
        {
            rend.GetPropertyBlock(SharedMaterialPropertyBlock);
            SharedMaterialPropertyBlock.SetVector(shaderNameID, value); //set color doesnt work with hdr, wtf?
            rend.SetPropertyBlock(SharedMaterialPropertyBlock);
        }

        public static void SetColorPropertyBlock(this Renderer rend, int shaderNameID, Color value, int materialIndex)
        {
            rend.GetPropertyBlock(SharedMaterialPropertyBlock, materialIndex);
            SharedMaterialPropertyBlock.SetVector(shaderNameID, value); //set color doesnt work with hdr, wtf?
            rend.SetPropertyBlock(SharedMaterialPropertyBlock, materialIndex);
        }

        public static void SetVectorPropertyBlock(this Renderer rend, int shaderNameID, Vector4 value)
        {
            rend.GetPropertyBlock(SharedMaterialPropertyBlock);
            SharedMaterialPropertyBlock.SetVector(shaderNameID, value);
            rend.SetPropertyBlock(SharedMaterialPropertyBlock);
        }

        public static void SetMatrixPropertyBlock(this Renderer rend, int shaderNameID, Matrix4x4 value)
        {
            rend.GetPropertyBlock(SharedMaterialPropertyBlock);
            SharedMaterialPropertyBlock.SetMatrix(shaderNameID, value);
            rend.SetPropertyBlock(SharedMaterialPropertyBlock);
        }

        public static GraphicsFormat GetGraphicsFormatHDR()
        {
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Render)) return GraphicsFormat.B10G11R11_UFloatPack32;
            else return GraphicsFormat.R16G16B16A16_SFloat;
        }

        public static void AddMaterialInstance(this Renderer rend, Material newInstance)
        {
            _materialsDummy.Clear();
            rend.GetSharedMaterials(_materialsDummy);
            _materialsDummy.Add(newInstance);
            rend.sharedMaterials = _materialsDummy.ToArray();
        }

        public static void RemoveMaterialInstance(this Renderer rend, Material newInstance)
        {
            _materialsDummy.Clear();
            rend.GetSharedMaterials(_materialsDummy);
            _materialsDummy.Remove(newInstance);
            rend.sharedMaterials = _materialsDummy.ToArray();
        }
    }
}