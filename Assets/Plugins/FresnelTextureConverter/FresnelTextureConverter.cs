using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditor.Rendering.LookDev;

namespace FemtoChanTools.FresnelTextureConverter
{
    [CreateAssetMenu(fileName = "FresnelTextureConverter", menuName = "FemtoChanTools/FresnelTextureConverter")]
    public class FresnelTextureConverter : ScriptableObject
    {
        [SerializeField] private ComputeShader computeShader;
        [SerializeField] private Texture2D sourceTexture;
        [SerializeField] private float edge = 0;
        [SerializeField] private int textureSize = 0;

        private const string KERNEL_NAME = "CSMain";

        [SerializeField]
        private RenderTexture renderTextureAsset;

        public Texture2D Execute()
        {
            RenderTexture renderTexture;

            renderTexture = new RenderTexture(textureSize, textureSize, 0);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();

            int kernelIndex = computeShader.FindKernel(KERNEL_NAME);
            int IdEdge = Shader.PropertyToID("Edge");
            int IdTextureSize = Shader.PropertyToID("TextureSize");
            int IdRenderTexture = Shader.PropertyToID("RenderTexture");
            int IdSourceTexture = Shader.PropertyToID("SourceTexture");

            computeShader.SetFloat(IdEdge, edge);
            computeShader.SetFloat(IdTextureSize, textureSize);
            computeShader.SetTexture(kernelIndex, IdRenderTexture, renderTexture);
            computeShader.SetTexture(kernelIndex, IdSourceTexture, sourceTexture);

            computeShader.Dispatch(kernelIndex, textureSize, textureSize, 1);



            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();

            Graphics.Blit(renderTexture, renderTextureAsset);

            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            //Write to a file in the project folder
            string path = $"{AssetDatabase.GetAssetPath(this).Replace($"{this.name}.asset", string.Empty)}outputTexture.png";
            File.WriteAllBytes(path, bytes);
            Texture2D created = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            AssetDatabase.ImportAsset(path);

            RenderTexture.active = null;
            renderTexture.Release();
            DestroyImmediate(renderTexture);

            return created;
        }
    }

    [CustomEditor(typeof(FresnelTextureConverter))]
    public class FresnelTextureConverterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var _target = target as FresnelTextureConverter;

            if (GUILayout.Button("Execute"))
            {
                var context = _target.Execute();
                Debug.Log("complete.", context);
            }
        }
    }
}
