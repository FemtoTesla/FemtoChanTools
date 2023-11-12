using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace FemtoChanTools.RenderTextureConverter
{
    [CreateAssetMenu(fileName = "RenderTextureConverter", menuName = "FemtoChanTools/RenderTextureConverter")]
    public class RenderTextureConverter : ScriptableObject
    {
        [SerializeField]
        private RenderTexture renderTexture;

        public void Execute()
        {
            Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex.Apply();

            Color[] _colors = tex.GetPixels();
            for (int _i = 0; _i < _colors.Length; _i++)
            {
                float a = (_colors[_i].r + _colors[_i].g + _colors[_i].b) / 3.0f;
                _colors[_i] = new Color(1, 1, 1, a);
            }
            tex.SetPixels(_colors);

            // Encode texture into PNG
            byte[] bytes = tex.EncodeToPNG();
            Object.DestroyImmediate(tex);

            //Write to a file in the project folder
            string path = $"{AssetDatabase.GetAssetPath(this).Replace($"{this.name}.asset", string.Empty)}outputTexture.png";
            File.WriteAllBytes(path, bytes);
            Texture2D created = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            AssetDatabase.ImportAsset(path);

            RenderTexture.active = null;
        }
    }

    [CustomEditor(typeof(RenderTextureConverter))]
    public class RenderTextureConverterEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var _target = target as RenderTextureConverter;

            if (GUILayout.Button("Execute"))
            {
                _target.Execute();
                Debug.Log("complete.");
            }
        }
    }
}
