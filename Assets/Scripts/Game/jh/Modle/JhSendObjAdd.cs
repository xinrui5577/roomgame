using System.Collections.Generic;
using Assets.Scripts.Game.jh.EventII;
using Sfs2X.Entities.Data;
using UnityEngine;

namespace Assets.Scripts.Game.jh.Modle
{
    public class JhSendObjAdd : MonoBehaviour
    {
        public class GetChipData
        {
            public JhUserInfo UserInfo;
            public int Gold;
            public List<int> AnteRate;
            public ISFSObject SendObj;
        }

        public class GetBeatMinAndMix
        {
            public JhUserInfo UserInfo;
            public int SignleBet;
            public long PlayerGold;
            public List<int> AnteRate;
            public ISFSObject SendObj;

        }

        public EventObject EventObj;

        public void OnRecieve(EventData data)
        {
            if (data.Name.Equals("GetChip"))
            {
                OnGetChip(data.Data);
            }
            else if (data.Name.Equals("GetBeatMinAndMax"))
            {
                OnGetBeatMinAndMa(data.Data);
            }
        }

        protected virtual void OnGetChip(object data)
        {
            var chipData = (GetChipData) data;
            if(chipData!=null)
            {
                int chipValue = chipData.Gold;
                int chipCnt = 1;
                if (chipData.UserInfo.IsLook)
                {
                    chipValue /= 2;
                    chipCnt += 1;
                }
                int chipIndex = chipData.AnteRate.IndexOf(chipValue);
                if (chipIndex < 0)
                {
                    chipIndex = 0;
                }
                chipData.SendObj.PutInt("ChipValue", chipValue);
                chipData.SendObj.PutInt("ChipIndex", chipIndex);
                chipData.SendObj.PutInt("ChipCnt", chipCnt);
            }
        }

        protected virtual void OnGetBeatMinAndMa(object data)
        {
            var beatData = (GetBeatMinAndMix) data;
            beatData.SendObj.PutInt("SignleBet", beatData.SignleBet);
            beatData.SendObj.PutLong("PlayerGold", beatData.UserInfo.IsLook ? beatData.PlayerGold / 2 : beatData.PlayerGold);
        }
    }
}
