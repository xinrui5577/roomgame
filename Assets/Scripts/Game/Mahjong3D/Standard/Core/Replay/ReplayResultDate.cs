using System.Collections.Generic;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class ReplayPlayerDate
    {       
        public List<int> HuCards;
        public List<int> HardCards;
        public List<CpgModel> CpgModels;

        public ReplayUserData UserData;

        public ReplayPlayerDate(int index)
        {
            //玩家信息
            var datas = GameCenter.Replay.ReplayData;
            UserData = datas.GetUserData(index);
        }
    }

    public class ReplayResultDate
    {
        public Dictionary<int, ReplayPlayerDate> ResultDic = new Dictionary<int, ReplayPlayerDate>();

        public void SetHandCard(List<int> cards, int index)
        {
            var data = GetPlayerDate(index);
            data.HardCards = cards;
        }

        public void SetCpgModels(List<CpgModel> models, int index)
        {
            var data = GetPlayerDate(index);
            data.CpgModels = models;
        }

        public void SetHucardList(List<int> cards, int index)
        {
            var data = GetPlayerDate(index);
            data.HuCards = cards;
        }

        private ReplayPlayerDate GetPlayerDate(int index)
        {
            ReplayPlayerDate data;
            if (!ResultDic.TryGetValue(index, out data))
            {
                data = new ReplayPlayerDate(index);
                ResultDic[index] = data;
            }
            return data;
        }
    }
}
