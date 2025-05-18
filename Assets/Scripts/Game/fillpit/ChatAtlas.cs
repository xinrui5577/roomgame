using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.fillpit
{
    public class ChatAtlas : MonoBehaviour
    {
        public List<UIAtlas> AtlasList = new List<UIAtlas>();
        public List<string> CommonList = new List<string>();
       
        private UIAtlas _getAtla;
        public UIAtlas GetAtlaByName(string atlasName)
        {
            if (AtlasList == null || AtlasList.Count == 0)
            {
                com.yxixia.utile.YxDebug.YxDebug.LogError("AtlasManager is null or empty,Please ensure your resourse");
                return null;
            }
            if (YxFramwork.Common.App.GameKey.Equals("ykmj"))
                _getAtla = AtlasList.Find(atla => atla.name == string.Format("YkPhiz_{0}", atlasName));
            else
                _getAtla = AtlasList.Find(atla => atla.name == string.Format("Phiz_{0}", atlasName));
            if (_getAtla != null)
            {
                return _getAtla;
            }
            return null;
        }

        public UIAtlas GetAtlasByIndex(int index)
        {
            return AtlasList[index];
        }

        public string GetCommonByIndex(int index)
        {
            return CommonList[index];
        }
  
    }
}