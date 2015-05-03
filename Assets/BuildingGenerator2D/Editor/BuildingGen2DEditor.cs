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
