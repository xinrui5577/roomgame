using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongThrowAdpOffset : MonoBehaviour
    {
        public Vector3 PositionOffset;
        public int RowCnt;

        public void OnInitalization()
        {
            var gorup = GetComponent<MahjongThrow>();
            if (GameCenter.DataCenter.ConfigData.MaxPlayerCount == 2)
            {
                gorup.RowCnt = RowCnt;
                var v3 = transform.localPosition;
                transform.localPosition = PositionOffset;
            }
        }
    }
}