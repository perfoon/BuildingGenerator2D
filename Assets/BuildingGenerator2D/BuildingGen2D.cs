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
        private List<STSpriteInfo> m_GroundSprites = new List<STSpriteInfo>();
		private List<STSpriteInfo> m_RoofSprites = new List<STSpriteInfo>();
		private List<STSpriteInfo> m_WindowSprites = new List<STSpriteInfo>();
		private List<STSpriteInfo> m_LadderSprites = new List<STSpriteInfo>();
		private GameObject m_building;

		public string buildingName { get; set; }
		public float buildingX { get; set; }
		public float buildingY { get; set; }

		public int MinLength { get; set; }
		public int MaxLength { get; set; }

		public int MinHeight { get; set; }
		public int MaxHeight { get; set; }

		public int windowProbability { get; set; }

		public static float pixelsPerUnit = 100f;
		public float onePixelUnit = 1f / pixelsPerUnit;

		public const string LAYER_NAMEO = "BottomLayer";
		public int sortingOrderO = 0;

		public const string LAYER_NAME1 = "TopLayer";
		public int sortingOrder1 = 1;

		
		public List<STSpriteInfo> GroundSprites
        {
            get
            {
                return this.m_GroundSprites;
            }
        }

		public List<STSpriteInfo> RoofSprites
		{
			get
			{
				return this.m_RoofSprites;
			}
		}

		public List<STSpriteInfo> WindowSprites
		{
			get
			{
				return this.m_WindowSprites;
			}
		}

		public List<STSpriteInfo> LadderSprites
		{
			get
			{
				return this.m_LadderSprites;
			}
		}
		
		public List<STSpriteInfo> copyOfGroundSprites
		{
			get
            {
                List<STSpriteInfo> list = new List<STSpriteInfo>();
                foreach (STSpriteInfo i in this.m_GroundSprites)
                    list.Add(i);
                return list;
            }
        }

		public BuildingGen2D() {
			MinLength = 1;
			MaxLength = 1;
			MinHeight = 1;
			MaxHeight = 1;
		}

        public void RemoveGroundSprite(STSpriteInfo info)
        {
            m_GroundSprites.Remove(info);
        }

        public void AddGroundSprite(Object resource)
        {
            if (resource is Texture2D || resource is Sprite)
            {

                STSpriteInfo info = new STSpriteInfo();
                info.source = resource;
                if (resource is Sprite)
                {
                    info.target = resource as Sprite;
                }

                this.GroundSprites.Add(info);
            }
        }

        public void AddGroundSprites(Object[] resources)
        {
            foreach (Object resource in resources)
            {
                this.AddGroundSprite(resource);
            }
        }

		public void AddRectangleSprite(GameObject gameObject, Color32 color, Color32 borderColor, int borderWidth, int width, int height, string LAYER_NAME, int sortingOrder)
		{
			
			Texture2D texture = new Texture2D(width, height, TextureFormat.RGB24, false, true);
			texture.filterMode = FilterMode.Point;
			texture.wrapMode = TextureWrapMode.Clamp;
			Color32[] pixels = texture.GetPixels32();
			for (int i = 0; i < pixels.Length; i++) {
                if (i <= width * borderWidth || i >= width * height - width * borderWidth
                    || (i % width) < borderWidth || (i % width) >= width - borderWidth)
                {
                    pixels[i] = borderColor;
                }
                else
                {
                    pixels[i] = color;
                }
			}
			texture.SetPixels32(pixels);
			texture.Apply();

			Sprite newSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f),128f);

			gameObject.AddComponent<SpriteRenderer>();
			SpriteRenderer sprRenderer = gameObject.GetComponent<SpriteRenderer>();

			sprRenderer.sortingOrder = sortingOrder;
			sprRenderer.sortingLayerName = LAYER_NAME;
			
			sprRenderer.sprite = newSprite;
		}

		public void GenerateBuilding() {

			if (m_GroundSprites.Count != 0 && m_RoofSprites.Count != 0 && m_WindowSprites.Count != 0 && m_LadderSprites.Count != 0) {
				if (m_building == null || GameObject.Find (buildingName) == null) { //m_building.name
					//m_building = new GameObject ("Building");
					m_building = new GameObject (buildingName);
				} else {
					GameObject.DestroyImmediate (GameObject.Find (buildingName)); //m_building.name
					Debug.Log ("deleted gameobject ");
					//m_building = new GameObject ("Building");
					m_building = new GameObject (buildingName);
				}

				int random_length = Random.Range (MinLength, MaxLength + 1);
				int random_height = Random.Range (MinHeight, MaxHeight + 1);
				//Ground generation
				GenerateGround(random_length);

				//Roof generation
				GenerateRoof(random_length, random_height);

				//maja fassaad
				List<Blueprint> blueprints = new List<Blueprint>();
				Blueprint.windowCount = 0;
				int nextLadderPos = -1;
				for (int i = 0; i< random_height*random_length;i++) {

					if ( i == nextLadderPos) {
						Blueprint bp = new Blueprint("windows",m_WindowSprites, windowProbability, "ladders",m_LadderSprites);
						blueprints.Add(bp);
					} else {
						Blueprint aknad = new Blueprint("windows",m_WindowSprites, windowProbability);
						blueprints.Add(aknad);
					}
				}
				/*Blueprint aknad = new Blueprint("windows",m_WindowSprites);
				Blueprint aknad2 = new Blueprint("windows",m_WindowSprites);
				Blueprint aknad3 = new Blueprint("windows",m_WindowSprites);
				blueprints.Add(aknad);
				blueprints.Add(aknad2);
				blueprints.Add(aknad3);*/
				GenerateWalls( blueprints, random_length, random_height);


				//vundament
				if (Random.Range (0, 2) == 1) {
					GameObject foundation = new GameObject("Foundation");
					float h = random_height/48f;
					//float w = random_length + onePixelUnit * 2f;
					foundation.transform.position = new Vector3 (0, h, 0);
					foundation.transform.parent = m_building.transform;
					AddRectangleSprite(foundation, new Color32(107, 97, 84, 255), new Color32(53, 47, 45, 255), 1, random_length * 130, random_height * 128 / 24, LAYER_NAME1, sortingOrder1);
				}

				//Building wall generation
				GameObject go2 = new GameObject("Wall");
				go2.transform.position = new Vector3 (0, random_height/2.0f, 0);
				go2.transform.parent = m_building.transform;
                AddRectangleSprite(go2, new Color32(90, 84, 76, 255), new Color32(53, 47, 45, 255), 1, random_length * 128, random_height * 128, LAYER_NAMEO, sortingOrderO);


				m_building.transform.position = new Vector3 (buildingX, buildingY, 0);
				
			} else {
				Debug.Log ("No Sprites Added!! Try Again!");
			}
			Debug.Log ("Building thingy!");



		}

		public void GenerateGround(int random_length) {

			for (int i = 0; i < random_length; i++) {
				
				GameObject go = new GameObject ("Ground_" + i);
				float x_transformed_beginning = random_length /  -2.0f + 0.5f;
				//to flip it or not decision
				int posNeg = Random.Range (0, 2);
				if (posNeg == 0) {
					go.transform.localScale = new Vector3 (-1, 1, 1);
				} 
				
				go.transform.parent = m_building.transform;
				SpriteRenderer renderer = go.AddComponent<SpriteRenderer> ();
				renderer.sortingOrder = sortingOrder1;
				renderer.sortingLayerName = LAYER_NAME1;
				int random = Random.Range (0, m_GroundSprites.Count);
				go.transform.position = new Vector3 (x_transformed_beginning + i, (-m_GroundSprites [random].target.rect.height/2f +2)*onePixelUnit , 0);
				renderer.sprite = m_GroundSprites [random].target;
			}
		
		}

		public void GenerateRoof(int building_width, int building_height) {

			int random = Random.Range (0, m_RoofSprites.Count);
			Sprite roofSprite = m_RoofSprites [random].target;

			//creating new texture from the sprite(this texture is exactly the size of the image, not 128x128)
			Texture2D source = spriteToTexture (roofSprite);
			int sourceHeight = source.height;
			int sourceWidth = source.width;

			//right side roof edge
			//Important!! in Rect the values start form the bottom left corner
			Sprite leftRoofTip = Sprite.Create(source, new Rect(0f, 0f, 2f, sourceHeight), new Vector2(0f, 1f));//moving point is in the upper left corner (0,1)
			GameObject roof_left = new GameObject ("Roof_left_0");
			SpriteRenderer renderer_l = roof_left.AddComponent<SpriteRenderer> ();
			renderer_l.sortingOrder = sortingOrder1;
			renderer_l.sortingLayerName = LAYER_NAME1;
			renderer_l.sprite = leftRoofTip;
			roof_left.transform.position = new Vector3 (-building_width / 2f - 2 * onePixelUnit, building_height + onePixelUnit, 0);
			//roof_left.transform.localScale = new Vector3 (2, 2, 1);
			roof_left.transform.parent = m_building.transform;


			//greate texture for middlepart
			Sprite middlePart = Sprite.Create(source, new Rect(2f, 0f, sourceWidth - 4f, sourceHeight), new Vector2(0f, 1f));//moving point is in the upper left corner
			//Debug.Log ("-- testing   " + middlePart.rect.width);
			//creating new texture from the sprite, then can go 'around' the image from left to right as much is needed
			Texture2D middleRoof_texture = spriteToTexture (middlePart);

			//roof middle part generation
			Sprite correctLength_middlePart = Sprite.Create(middleRoof_texture, new Rect(0f, 0f, building_width * pixelsPerUnit, sourceHeight), new Vector2(0f, 1f));
			GameObject roof_mid = new GameObject ("Roof_mid_0");
			SpriteRenderer sr = roof_mid.AddComponent<SpriteRenderer> ();
			sr.sortingOrder = sortingOrder1;
			sr.sortingLayerName = LAYER_NAME1;
			sr.sprite = correctLength_middlePart;
			roof_mid.transform.position = new Vector3 (-building_width / 2f, building_height + onePixelUnit, 0);
			roof_mid.transform.parent = m_building.transform;


			//right side roof edge
			Sprite rightRoofTip = Sprite.Create(source, new Rect(sourceWidth - 2f, 0f, 2, sourceHeight), new Vector2(0f, 1f));//moving point is in the upper left corner (0,1)
			GameObject roof_right = new GameObject ("Roof_right_0");
			SpriteRenderer renderer_r = roof_right.AddComponent<SpriteRenderer> ();
			renderer_r.sortingOrder = sortingOrder1;
			renderer_r.sortingLayerName = LAYER_NAME1;
			renderer_r.sprite = rightRoofTip;
			roof_right.transform.position = new Vector3 (building_width / 2f, building_height + onePixelUnit, 0);
			roof_right.transform.parent = m_building.transform;
			
		}

		private Texture2D spriteToTexture (Sprite sprite) {
			//creating new texture from the sprite(this texture is exactly the size of the image, not 128x128)
			Texture2D source = new Texture2D((int)sprite.rect.width,(int)sprite.rect.height); 
			source.filterMode = FilterMode.Point; //makes it into pixels. Basically same as TextureFormat truecolor
			
			Color[] newColors = sprite.texture.GetPixels((int)sprite.rect.x, 
			                                                 (int)sprite.rect.y, 
			                                                 (int)sprite.rect.width, 
			                                                 (int)sprite.rect.height);
			
			source.SetPixels(newColors);
			source.Apply();
			return source;
		}

		public void GenerateWalls( List<Blueprint> blueprints,int building_width, int building_height) {
			//blueprint object points are located in the upper left corner
			int column = 0;
			int row = 0;
			for (int j = 0; j < blueprints.Count; j++) { 
				Blueprint bp = blueprints [j];
				List<BlueprintObject> Objects = bp.getBlueprintObjects();
				int objCount = Objects.Count;//bp.getBlueprintObjects.Count
				for (int i = 0; i <  objCount; i++) {
					Debug.Log ("-- testing " +Objects[i].Name +"   x " + Objects[i].X + " y " + Objects[i].Y);
					GameObject go = new GameObject (Objects[i].Name);
					SpriteRenderer renderer = go.AddComponent<SpriteRenderer> ();

					renderer.sortingOrder = sortingOrder1;
					renderer.sortingLayerName = LAYER_NAME1;

					renderer.sprite = Objects[i].Sprite;

					float leftUpperCornerX = -building_width /2f + column;
					float leftUpperCornerY = building_height - row;

					float objX = leftUpperCornerX + Objects[i].X*onePixelUnit; 
					float objY = leftUpperCornerY - Objects[i].Y*onePixelUnit;
					go.transform.position = new Vector3 (objX, objY, 0);
					go.transform.parent = m_building.transform;
				}

				if (building_width-2 < column) {
					row += 1;
					column = 0;
				} else {
					column += 1;
				}
			}
		}
    }
}
