using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace ST
{
    public class STSpriteListSelector : Editor
    {
        private List<STSpriteInfo> m_Sprites;

        private static Color spriteBoxNormalColor = new Color(0.897f, 0.897f, 0.897f, 1f);
        //private static Color spriteBoxHighlightColor = new Color(0.798f, 0.926f, 0.978f, 1f);

        private GUIStyle boxStyle;
        private GUIStyle paddingStyle;

        private Vector2 scrollViewOffset = Vector2.zero;
        private bool expandSprites = false;

        public System.String Name { get; set; }

        public void Init(System.String name, List<STSpriteInfo> sprites)
        {
            m_Sprites = sprites;
            Name = name;

            this.boxStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector).box);
            this.paddingStyle = new GUIStyle();
            //this.serializedObject.Update();
        }

        public void RenderEditor()
        {
            Rect controlRect = EditorGUILayout.GetControlRect();
            GUI.Label(controlRect, Name + " (" + this.m_Sprites.Count.ToString() + ")", EditorStyles.boldLabel);

            GUI.Label(new Rect(controlRect.width - 50f, controlRect.y, 50f, 20f), "Expand");
            GUI.changed = false;
            bool toggleRes = GUI.Toggle(new Rect(controlRect.width - 2f, controlRect.y + 1f, 20f, 20f), expandSprites, " ");
            if (GUI.changed)
            {
                expandSprites = toggleRes;
            }

            float scrollHeight = expandSprites ? 250f : 75f;

            //Sprites area
            Rect outerRect = EditorGUILayout.BeginVertical(this.boxStyle);

            this.scrollViewOffset = EditorGUILayout.BeginScrollView(this.scrollViewOffset, GUILayout.Height(scrollHeight));

            this.DrawSpritesWithThumbs();
            this.DragSprites(outerRect);

            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        public void DragSprites(Rect outerRect)
        {
            Event evt = Event.current;
            //Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
            //GUI.Box(drop_area, "Add Sprite (Drop Here)", this.boxStyle);

            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    {
                        /*if (!outerRect.Contains(evt.mousePosition))
                            return;
                        */
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (evt.type == EventType.DragPerform)
                        {
                            DragAndDrop.AcceptDrag();
                            Object[] filtered = ST.STTools.FilterResourcesForAtlasImport(DragAndDrop.objectReferences);

                            for (int i = 0; i < filtered.Length; i++)
                            {
                                if (this.m_Sprites.Find(s => s.source == filtered[i]) != null)
                                {
                                    Debug.LogWarning("A sprite with source \"" + STTools.GetAssetPath(filtered[i]) + "\" already exists in the atlas");
                                    System.Array.Clear(filtered, i, 1);
                                }
                            }

                            this.AddSprites(filtered);
                        }
                        break;
                    }
            }
        }

        private void DrawSpritesWithThumbs()
        {
            List<STSpriteInfo> sprites = this.copyOfSprites;
            sprites.Sort();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(3f);
            EditorGUILayout.BeginVertical();

            float thumbnailMaxHeight = 40f;
            float labelHeight = 20f;

            RectOffset padding = this.paddingStyle.padding;
            RectOffset thumbnailPadding = new RectOffset(6, 6, 6, 3);

            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.fontStyle = FontStyle.Normal;
            labelStyle.alignment = TextAnchor.MiddleCenter;

            foreach (STSpriteInfo sprite in sprites)
            {
                Color boxColor = STSpriteListSelector.spriteBoxNormalColor;
                GUILayout.Space(3f);
                EditorGUILayout.BeginHorizontal(this.paddingStyle);

                if (sprite != null && sprite.target != null)
                {
                    float thumbnailHeight = 0f;
                    float thumbnailHeightWithPadding = 0f;
                    if (expandSprites)
                    {
                        thumbnailHeight = (sprite.target.rect.height > thumbnailMaxHeight) ? thumbnailMaxHeight : sprite.target.rect.height;
                        thumbnailHeightWithPadding = thumbnailHeight + 6f + 6f;
                    }

                    Rect controlRect = GUILayoutUtility.GetRect(0.0f, (thumbnailHeightWithPadding + labelHeight), GUILayout.ExpandWidth(true));
                    Rect clickRect = new Rect(controlRect.x, controlRect.y, controlRect.width, controlRect.height);

                    GUI.color = boxColor;
                    GUI.Box(new Rect(controlRect.x - padding.left, controlRect.y - padding.top, controlRect.width + (padding.left + padding.right), controlRect.height + (padding.top + padding.bottom)), "", this.boxStyle);
                    GUI.color = Color.white;

                    if (sprite.target.texture != null && expandSprites)
                        this.DrawThumbnail(sprite.target, thumbnailHeight, controlRect, thumbnailPadding);

                    GUI.Label(new Rect(controlRect.x, (controlRect.y + thumbnailHeightWithPadding + 1f), controlRect.width, labelHeight), sprite.target.name + " (" + sprite.target.rect.width + "x" + sprite.target.rect.height + ")", labelStyle);

                    if (GUI.Button(new Rect((controlRect.width - 17f), (controlRect.y), 20f, 20f), "X"))
                    {
                        this.RemoveSprite(sprite);
                    }


                    else if (Event.current.type == EventType.MouseUp && clickRect.Contains(Event.current.mousePosition))
                    {
                        EditorGUIUtility.PingObject(sprite.target);

                        // Remove the focus of the focused control
                        GUI.FocusControl("");

                        // Set as selected
                        //if (!isSelected) this.SetSelected(info.GetHashCode());
                    }

                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(6f);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawThumbnail(Sprite sprite, float height, Rect controlRect, RectOffset padding)
        {
            // Calculate the sprite rect inside the texture
            Rect spriteRect = new Rect(sprite.textureRect.x / sprite.texture.width,
                                       sprite.textureRect.y / sprite.texture.height,
                                       sprite.textureRect.width / sprite.texture.width,
                                       sprite.textureRect.height / sprite.texture.height);

            // Get the original sprite size
            Vector2 spriteSize = new Vector2(sprite.rect.width, sprite.rect.height);

            // Determine the max size of the thumb
            Vector2 thumbMaxSize = new Vector2((controlRect.width - (padding.left + padding.right)), height);

            // Clamp the sprite size based on max width and height of the control
            if (spriteSize.x > thumbMaxSize.x)
            {
                spriteSize *= thumbMaxSize.x / spriteSize.x;
            }
            if (spriteSize.y > thumbMaxSize.y)
            {
                spriteSize *= thumbMaxSize.y / spriteSize.y;
            }

            // Prepare the rect for the texture draw
            Rect thumbRect = new Rect(0f, 0f, spriteSize.x, spriteSize.y);

            // Position in the middle of the control rect
            thumbRect.x = controlRect.x + ((controlRect.width - spriteSize.x) / 2f);
            thumbRect.y = controlRect.y + padding.top + ((height - spriteSize.y) / 2f);

            // Draw the thumbnail
            GUI.DrawTextureWithTexCoords(thumbRect, sprite.texture, spriteRect, true);
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

        public void RemoveSprite(STSpriteInfo sprite)
        {
            m_Sprites.Remove(sprite);
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

                this.m_Sprites.Add(info);
            }
        }

        public void AddSprites(Object[] resources)
        {
            foreach (Object resource in resources)
            {
                this.AddSprite(resource);
            }
        }

    }
}
