using UnityEngine;
using System.Collections.Generic;


namespace Assets.Scripts.Game.fillpit.Mgr
{
    public class AtlasMgr : MonoBehaviour
    {
        public List<UIAtlas> Atlas = new List<UIAtlas>();

        /// <summary>
        /// 通过图集的名字找到图集
        /// </summary>
        /// <param name="atlasName">图集名字</param>
        /// <returns></returns>
        public UIAtlas GetAtlasByName(string atlasName)
        {
            if (Atlas == null || Atlas.Count == 0)
            {
                com.yxixia.utile.YxDebug.YxDebug.LogError(" The atlas list is null or empty,please check the list");
                return null;
            }

            UIAtlas atl = Atlas.Find(atla => atla.name == atlasName);

            return atl;
        }
    }
}