using UnityEngine;
using System.Collections;
using UnityEditor;
using ST;

namespace BuildingGen2D
{
    [CustomEditor(typeof(BuildingGen2D))]
    public class BuildingGen2DEditor : Editor
    {
        private BuildingGen2D m_BuildingGen;

        //Editor components
        private STSpriteListSelector spriteListSelector;

        protected void OnEnable()
        {
            m_BuildingGen = this.target as BuildingGen2D;

            spriteListSelector = ScriptableObject.CreateInstance(typeof(STSpriteListSelector)) as STSpriteListSelector;
            spriteListSelector.Init("Ground sprites", m_BuildingGen.Sprites);
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Texture"), new GUIContent("Atlas Texture"));
            //EditorGUILayout.Space();
            spriteListSelector.RenderEditor();

			m_BuildingGen.MinLength = EditorGUILayout.IntSlider("Min Length", m_BuildingGen.MinLength, 1, 10);
			m_BuildingGen.MaxLength = EditorGUILayout.IntSlider("Max Length", m_BuildingGen.MaxLength, 1, 10);

			if (m_BuildingGen.MaxLength < m_BuildingGen.MinLength)
				m_BuildingGen.MaxLength = m_BuildingGen.MinLength;

			if(GUILayout.Button("Generate building"))
			{
				m_BuildingGen.GenerateBuilding();
			}
        }

        [MenuItem("Assets/Create/Building Generator 2D")]
        public static void CreateInstance()
        {
            STTools.CreateAsset<BuildingGen2D>("BuildingGen2D", "BuildingGenerator2D");
        }
    }
}
