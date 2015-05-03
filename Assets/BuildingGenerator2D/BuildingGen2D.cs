using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ST;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BuildingGen2D
{
    public class BuildingGen2D : ScriptableObject
    {
        [SerializeField]
        private List<STSpriteInfo> m_Sprites = new List<STSpriteInfo>();
		private GameObject m_building;

		public int MinLength { get; set; }
		public int MaxLength { get; set; }

        public List<STSpriteInfo> Sprites
        {
            get
            {
                return this.m_Sprites;
            }
        }

        public List<STSpriteInfo> copyOfSprites
        {
            get
            {
                List<STSpriteInfo> list = new List<STSpriteInfo>();
                foreach (STSpriteInfo i in this.m_Sprites)
                    list.Add(i);
                return list;
            }
        }

		public BuildingGen2D() {
			MinLength = 1;
			MaxLength = 1;
		}

        public void RemoveSprite(STSpriteInfo info)
        {
            m_Sprites.Remove(info);
        }

        public void AddSprite(Object resource)
        {
            if (resource is Texture2D || resource is Sprite)
            {

                STSpriteInfo info = new STSpriteInfo();
                info.source = resource;
                if (resource is Sprite)
                {
                    info.target = resource as Sprite;
                }

                this.Sprites.Add(info);
            }
        }

        public void AddSprites(Object[] resources)
        {
            foreach (Object resource in resources)
            {
                this.AddSprite(resource);
            }
        }

		public void GenerateBuilding() {

			if (m_building == null || GameObject.Find(m_building.name) == null) {
				m_building = new GameObject("Building");
			}

			GameObject go = new GameObject("Test");

			go.transform.parent = m_building.transform;
			SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();
			int random = Random.Range (0, m_Sprites.Count);
			renderer.sprite = m_Sprites[random].target;
			//Instantiate(m_Sprites[0].target);
			Debug.Log ("Building thingy!");
		}
    }
}
