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
    }
}
