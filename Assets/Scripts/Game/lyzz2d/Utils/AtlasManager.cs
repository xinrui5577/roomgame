/** 
 *文件名称:     AltasManager.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-04-17 
 *描述:         需要切换的图集管理中心，目前是来处理动态表情
 *历史记录: 
*/

using System.Collections.Generic;
using Assets.Scripts.Game.lyzz2d.Utils.Single;
using com.yxixia.utile.YxDebug;

namespace Assets.Scripts.Game.lyzz2d.Utils
{
    public class AtlasManager : MonoSingleton<AtlasManager>
    {
        public List<UIAtlas> Atlas = new List<UIAtlas>();

        public override void Awake()
        {
            base.Awake();
        }

        public UIAtlas GetAtlaByName(string name)
        {
            if (Atlas == null || Atlas.Count == 0)
            {
                YxDebug.LogError("AtlasManager is null or empty,Please ensure your resourse");
                return null;
            }
            var getAtla = Atlas.Find(atla => atla.name == string.Format("Phiz_{0}", name));
            if (getAtla != null)
            {
                return getAtla;
            }
            return null;
        }
    }
}