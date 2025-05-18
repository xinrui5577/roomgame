using UnityEngine;
using DG.Tweening;

namespace Assets.Scripts.Game.brnn3d
{
    public class DownUILeftBg : MonoBehaviour
    {
        public LuziInfoUIMgr TheLuziInfoUIMgr;
        //显示下面历史记录的背景
        public void ShowDownUILeftBg()
        {
            transform.DOLocalMoveX(-250, 0.5f);
        }

        //隐藏下面历史记录的背景
        public void HideDownUILeftBg()
        {
            transform.DOLocalMoveX(-780, 0.5f);
        }
    }
}

