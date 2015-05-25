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
        private STSpriteListSelector groundSpriteListSelector;
		private STSpriteListSelector roofSpriteListSelector;
		private STSpriteListSelector windowSpriteListSelector;
		private STSpriteListSelector ladderSpriteListSelector;


        protected void OnEnable()
        {
			m_BuildingGen = this.target as BuildingGen2D;

            groundSpriteListSelector = ScriptableObject.CreateInstance(typeof(STSpriteListSelector)) as STSpriteListSelector;
            groundSpriteListSelector.Init("Ground sprites", m_BuildingGen.GroundSprites);

			roofSpriteListSelector = ScriptableObject.CreateInstance(typeof(STSpriteListSelector)) as STSpriteListSelector;
			roofSpriteListSelector.Init("Roof sprites", m_BuildingGen.RoofSprites);

			windowSpriteListSelector = ScriptableObject.CreateInstance(typeof(STSpriteListSelector)) as STSpriteListSelector;
			windowSpriteListSelector.Init("Window sprites", m_BuildingGen.WindowSprites);

			ladderSpriteListSelector = ScriptableObject.CreateInstance(typeof(STSpriteListSelector)) as STSpriteListSelector;
			ladderSpriteListSelector.Init("Ladder sprites", m_BuildingGen.LadderSprites);

        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();
            //EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Texture"), new GUIContent("Atlas Texture"));
            //EditorGUILayout.Space();
			GUILayout.Label ("Building name:", EditorStyles.boldLabel);
			//m_BuildingGen.buildingName = "Building_A";
			m_BuildingGen.buildingName = EditorGUILayout.TextField ( m_BuildingGen.buildingName);

			GUILayout.Label ("Building location:", EditorStyles.boldLabel);
			m_BuildingGen.buildingX = EditorGUILayout.FloatField("x: ", m_BuildingGen.buildingX);
			m_BuildingGen.buildingY = EditorGUILayout.FloatField("y: ", m_BuildingGen.buildingY);


            groundSpriteListSelector.RenderEditor();
			roofSpriteListSelector.RenderEditor();
			windowSpriteListSelector.RenderEditor();
			//m_BuildingGen.windowProbability = 100;//if not changed the value will be this
			m_BuildingGen.windowProbability = EditorGUILayout.IntSlider("Window probability", m_BuildingGen.windowProbability, 0, 100);

			ladderSpriteListSelector.RenderEditor();

			EditorGUILayout.Separator ();
			EditorGUILayout.LabelField ("Building size parameters", EditorStyles.boldLabel);

			m_BuildingGen.MinLength = EditorGUILayout.IntSlider("Min Width", m_BuildingGen.MinLength, 1, 10);
			m_BuildingGen.MaxLength = EditorGUILayout.IntSlider("Max Width", m_BuildingGen.MaxLength, 1, 10);

			if (m_BuildingGen.MaxLength < m_BuildingGen.MinLength)
				m_BuildingGen.MaxLength = m_BuildingGen.MinLength;

			m_BuildingGen.MinHeight = EditorGUILayout.IntSlider("Min Heigth", m_BuildingGen.MinHeight, 1, 10);
			m_BuildingGen.MaxHeight = EditorGUILayout.IntSlider("Max Heigth", m_BuildingGen.MaxHeight, 1, 10);
			
			if (m_BuildingGen.MaxHeight < m_BuildingGen.MinHeight)
				m_BuildingGen.MaxHeight = m_BuildingGen.MinHeight;

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
