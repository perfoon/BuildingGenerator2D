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

        public void AddRectangleSprite(GameObject gameObject, Color32 color, Color32 borderColor, int borderWidth, int width, int height)
        {
			
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false, true);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
			Color32[] pixels = texture.GetPixels32();
			for (int i=0; i<pixels.Length; i++) {
				pixels[i] = color;
			}
			texture.SetPixels32(pixels);
			texture.Apply();

			Sprite newSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f),128f);

			gameObject.AddComponent<SpriteRenderer>();
			SpriteRenderer sprRenderer = gameObject.GetComponent<SpriteRenderer>();
			sprRenderer.sprite = newSprite;
		}

		public void GenerateBuilding() {

			if (m_Sprites.Count != 0) {
				if (m_building == null || GameObject.Find (m_building.name) == null) {
					m_building = new GameObject ("Building");
				} else {
					GameObject.DestroyImmediate (GameObject.Find (m_building.name));
					Debug.Log ("deleted gameobject ");
					m_building = new GameObject ("Building");
				}
				int random_length = Random.Range (MinLength, MaxLength + 1);
				//Ground generation
				GenerateGround(random_length);

				//Building wall generation
				GameObject go2 = new GameObject("Wall");
				go2.transform.position = new Vector3 (0, 1, 0);
				go2.transform.parent = m_building.transform;
				AddRectangleSprite(go2, new Color32(135, 128, 128, 255), random_length * 128 , 256);
				
			} else {
				Debug.Log ("No Sprites Added!! Try Again!");
			}
			Debug.Log ("Building thingy!");

			GameObject go2 = new GameObject("Wall");
			AddRectangleSprite(go2, new Color32(135, 128, 128, 255), new Color32(100, 90, 90, 255), 1, 256, 256);
		}

		public void GenerateGround(int random_length) {

			for (int i = 0; i < random_length; i++) {
				
				GameObject go = new GameObject ("Ground_" + i);
				float x_transformed_beginning = random_length /  -2.0f + 0.5f;
				go.transform.position = new Vector3 (x_transformed_beginning + i, 0, 0);
				//to flip it or not decision
				int posNeg = Random.Range (0, 2);
				if (posNeg == 0) {
					go.transform.localScale = new Vector3 (-1, 1, 1);
				} 
				
				go.transform.parent = m_building.transform;
				SpriteRenderer renderer = go.AddComponent<SpriteRenderer> ();
				int random = Random.Range (0, m_Sprites.Count);
				renderer.sprite = m_Sprites [random].target;
			}
		
		}
    }
}
