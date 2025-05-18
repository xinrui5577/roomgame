using UnityEngine;

namespace Assets.Scripts.Game.jh.Modle
{
    public class JhSendObjAddOne : JhSendObjAdd {

        protected override void OnGetChip(object data)
        {
            var chipData = (GetChipData)data;
            if (chipData != null)
            {
                int chipValue = chipData.Gold;
                int chipCnt = 1;
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

        protected override void OnGetBeatMinAndMa(object data)
        {
            var beatData = (GetBeatMinAndMix)data;
            
            if (beatData.UserInfo.IsLook)
            {
                beatData.PlayerGold = beatData.AnteRate[beatData.AnteRate.Count - 1];
                beatData.SignleBet *= 2;
            }
            beatData.SendObj.PutInt("SignleBet", beatData.SignleBet);
            beatData.SendObj.PutLong("PlayerGold", beatData.PlayerGold);
        }
    }
}
