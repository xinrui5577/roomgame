using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class BbmjMahjongPlayerHand : MahjongPlayerHand
    {
        protected override void Start()
        {
            base.Start();
            AddActionToDic(HandcardStateTyps.Youjin, SwitchYoujinState);
        }
        
        private void SwitchYoujinState(params object[] args)
        {
            MahjongContainer item;
            List<int> tingList = args[0] as List<int>;
            if (tingList == null || tingList.Count == 0) return;
            var list = MahjongList;
            for (int i = 0; i < list.Count; i++)
            {
                item = list[i];
                item.ResetPos();
                if (!tingList.Contains(item.Value))
                {
                    item.Lock = true;
                    item.RemoveMahjongScript();
                }
                else
                {
                    item.SetMahjongScript();
                    item.SetThowOutCall(YoujinClickEvent);
                }
            }
        }

        /// <summary>
        /// Daigu click event
        /// </summary>      
        private void YoujinClickEvent(Transform transf)
        {
            MahjongContainer item;
            var Mj = transf.GetComponent<MahjongContainer>();
            if (!Mj.Lock)
            {
                HasToken = false;
                GameCenter.EventHandle.Dispatch<C2STingArgs>((int)EventKeys.C2STing, (args) =>
                {
                    args.Card = Mj.Value;
                    args.Prol = NetworkProls.Youjin;
                });
                Mj.ResetPos();
                var list = MahjongList;
                for (int i = 0; i < list.Count; i++)
                {
                    item = list[i];
                    item.SetMahjongScript();
                    item.SetThowOutCall(ThrowCardClickEvent);
                }
            }
        }
    }
}