using UnityEngine;
using System.Collections;
using System;

namespace ST
{
	[System.Serializable]
    public class STSpriteInfo : IComparable<STSpriteInfo>
	{
		public UnityEngine.Object source;
		public Sprite target;

		public string getName()
		{
			if (this.target != null) {
				return this.target.name;
			} else if (this.source != null) {
				return this.source.name;
			}
			return string.Empty;
		}

        public int CompareTo(STSpriteInfo other)
        {
            return this.getName().CompareTo(other.getName());
        }

	}
}
