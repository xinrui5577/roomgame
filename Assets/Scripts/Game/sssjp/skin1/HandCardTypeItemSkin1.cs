using UnityEngine;

namespace Assets.Scripts.Game.sssjp.skin1
{
    public class HandCardTypeItem : MonoBehaviour
    {
        public UISprite TypeSprite;

        public UILabel NormalLabel;
         
         
        public void ShowType(string typeName)
        {
            TypeSprite.spriteName = typeName;

        }

        public void HideType()
        {
          
        }

        public void Reset()
        {
            
        }
    }
}
