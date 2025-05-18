using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongCpgItem : MonoBehaviour
    {
        public MahjongSign MahjongSign;

        private MahSignItem mSignItem;

        public MahSignItem SignItem
        {
            get
            {
                if (mSignItem == null) mSignItem = MahjongSign.VectorSign(true) as MahSignItem;
                return mSignItem;
            }
        }

        public List<MahjongContainer> ItemStore { get; private set; }
        public float OffsetX { get; private set; }
        public CpgModel Model { get; private set; }

        public virtual void OnInit(CpgModel model)
        {
            Model = model;
            ItemStore = GameCenter.Scene.MahjongCtrl.PopMahjong(model.Cards);
            for (int i = 0; i < ItemStore.Count; i++)
            {
                ItemStore[i].ExSetParent(transform);
            }

            if (model.Hide) SetHideMahjong();
            NormalMahjongLayout(ItemStore);
        }

        protected virtual void SetSignPosition()
        {
            var pos = ItemStore[1].transform.localPosition;
            if (ItemStore.Count == 4)
            {
                pos.x = pos.x + DefaultUtils.MahjongSize.x * 0.5f;
            }
            pos.z = -0.3f;
            SignItem.transform.localPosition = pos;

            switch (Model.RelativeSeat)
            {
                case RelativeSeat.Front: SignItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90)); break;
                case RelativeSeat.Behind: SignItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90)); break;
                case RelativeSeat.Opposite: SignItem.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 180)); break;
            }
        }

        /// <summary>
        /// 麻将牌扣着
        /// </summary>
        protected virtual void SetHideMahjong()
        {
            for (int i = 0; i < ItemStore.Count; i++)
            {
                ItemStore[i].transform.localRotation = Quaternion.Euler(0, 180, 0);
            }

            bool flag = Model.OpChair == 0
                || GameCenter.DataCenter.Config.ShowAnGang
                || GameCenter.Instance.GameType == GameType.Replay;

            if (flag)
            {
                var index = ItemStore.Count - 1;
                ItemStore[index].SetMjRota(0, 0, 0);
            }
        }

        public virtual void NormalMahjongLayout(IList<MahjongContainer> list)
        {
            var size = DefaultUtils.MahjongSize;
            for (int i = 0; i < list.Count; i++)
            {
                var mahjong = list[i];
                OffsetX += size.x * 0.5f;
                mahjong.transform.localPosition = new Vector3(OffsetX, -size.y * (0.5f), -size.z * (0.5f));
                OffsetX += size.x * 0.5f;
            }

            if (Model.RelativeSeat != RelativeSeat.None)
            {
                SetSignPosition();
            }
        }

        public virtual void SetZhuaGangLayout(int card)
        {
            var mahjong = GameCenter.Scene.MahjongCtrl.PopMahjong(card);
            mahjong.ExSetParent(transform);
            ItemStore.Add(mahjong);

            var offsetZ = DefaultUtils.MahjongSize.z;
            var pos = ItemStore[1].transform.localPosition + new Vector3(0, 0, -offsetZ);
            mahjong.transform.localPosition = pos;

            if (Model.RelativeSeat != RelativeSeat.None)
            {
                var signV3 = SignItem.transform.localPosition;
                signV3.z = -0.45f;
                SignItem.transform.localPosition = signV3;
            }
        }

        public void OnReset()
        {
            for (int i = 0; i < ItemStore.Count; i++)
            {
                GameCenter.Scene.MahjongCtrl.PushMahjongToPool(ItemStore[i]);
            }
            ItemStore.Clear();
        }
    }
}