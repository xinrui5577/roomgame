/** 
 *文件名称:     GuoDanSelect.cs 
 *作者:         AndeLee 
 *版本:         1.0 
 *Unity版本：   5.4.0f3 
 *创建时间:     2017-06-29 
 *描述:         过蛋选择界面。op:1024的特殊处理
 *历史记录: 
*/

using Assets.Scripts.Game.Mahjong2D.CommonFile.Tools.Singleton;
using UnityEngine;
using YxFramwork.Common;

namespace Assets.Scripts.Game.Mahjong2D.Game.GameCtrl
{
    public class GuoDanSelect : MonoSingleton<GuoDanSelect>
    {
        /// <summary>
        /// 显示父级
        /// </summary>
        [SerializeField]
        private GameObject _showPanrent;
        [HideInInspector]
        public bool DanSelect
        {
            set; get;
        }

        Mahjong2DGameServer Mahjong2DGame
        {
            get
            {
                return App.GetRServer<Mahjong2DGameServer>();
            }
        }

        public override void Awake()
        {
            base.Awake();
            if (_showPanrent!=null)
            {
                _showPanrent.SetActive(false);
                DanSelect = false;
            }
        }

        public void ShowDanSelect()
        {
            _showPanrent.SetActive(true);
            DanSelect = true;
        }

        public void OnClickSelectBtn(GameObject obj)
        {
            int num = int.Parse(obj.name);
            Mahjong2DGame.RequestGuoDan(num);
            _showPanrent.SetActive(false);
            DanSelect = false;
        }
    }
}
