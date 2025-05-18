using Assets.Scripts.Common.Utils;
using Sfs2X.Entities.Data;
using UnityEngine;
using YxFramwork.Tool;

namespace Assets.Scripts.Game.nn41
{
    public class GameResult : MonoBehaviour
    {
        private UILabel _goldLabel;
        [Tooltip("结算胜利特效")]
        public GameObject winEffect;
        protected void Awake()
        {
            _goldLabel = transform.FindChild("GoldLabel").GetComponent<UILabel>();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
	
        public void SetGameResult(ISFSObject data)
        {
	        gameObject.SetActive(true);
            var win = data.GetInt(InteractParameter.Gold);
            if (_goldLabel==null)return;
            var winState = win > 0;
            _goldLabel.text = (winState ? "+":"")+YxUtiles.ReduceNumber(win);
            winEffect.TrySetComponentValue(winState);
        }
	
    }
}
