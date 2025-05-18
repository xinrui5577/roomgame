using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    public class MahjongTable : MahjongTablePart, ISceneInitCycle, IReconnectedCycle
    {
        public bool IsCustom;
        public MeshRenderer TableMesh;
        public Transform TableLift;
        public float LiftOffset;
        public MeshRenderer[] Lifts;

        public void OnSceneInitCycle()
        {
            SwitchTableSkin();
        }

        public override void OnInitalization()
        {
            GameCenter.RegisterCycle(this);
        }

        public void OnReconnectedCycle()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// 切换麻将桌子皮肤
        /// </summary>
        public void SwitchTableSkin()
        {
            if (IsCustom) return;
            var assetsName = "TableSkin_" + MahjongUtility.MahjongTableColor;
            var texture = GameUtils.GetAssets<Texture>(assetsName);
            if (texture != null)
            {
                TableMesh.material.mainTexture = texture;
                for (int i = 0; i < Lifts.Length; i++)
                {
                    Lifts[i].material.mainTexture = texture;
                }
            }
        }

        /// <summary>
        /// 麻将机下移动画
        /// </summary>
        public void TableDownAnimation(float time)
        {
            var currPos = TableLift.transform.localPosition;
            var offsetY = new Vector3(currPos.x, LiftOffset, currPos.z);
            StartCoroutine(MoveTo(offsetY, time));
        }

        /// <summary>
        /// 麻将机上升动画
        /// </summary>
        public void TableUpAnimation(float time)
        {
            var currPos = TableLift.transform.localPosition;
            var offsetY = new Vector3(currPos.x, 0, currPos.z);
            StartCoroutine(MoveTo(offsetY, time));
        }

        private IEnumerator MoveTo(Vector3 moveTo, float time)
        {
            float val = 0;
            float bTime = Time.time;
            var fpos = TableLift.localPosition;
            var tpos = moveTo;
            while (val < time)
            {
                val = Time.time - bTime;
                float smoothval = val / time;
                TableLift.localPosition = Vector3.Lerp(fpos, tpos, smoothval);
                yield return 2;
            }
        }
    }
}