using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.Game.Mahjong3D.Standard
{
    [AddComponentMenu("UI/Effects/Gradient")]
    public class Gradient : BaseMeshEffect
    {
        [SerializeField]
        private Color32 topColor = Color.white;
        [SerializeField]
        private Color32 bottomColor = Color.black;
        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
            {
                return;
            }
            var vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);
            int count = vertexList.Count;
            ApplyGradient(vertexList, 0, count);
            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
        }

        private void ApplyGradient(List<UIVertex> vertexList, int start, int end)
        {
            if (vertexList.Count <= 0)
                return;
            float bottomY = vertexList[0].position.y;
            float topY = vertexList[0].position.y;
            for (int i = start; i < end; ++i)
            {
                float y = vertexList[i].position.y;
                if (y > topY)
                {
                    topY = y;
                }
                else if (y < bottomY)
                {
                    bottomY = y;
                }
            }
            float uiElementHeight = topY - bottomY;
            for (int i = start; i < end; ++i)
            {
                UIVertex uiVertex = vertexList[i];
                uiVertex.color = Color32.Lerp(bottomColor, topColor, (uiVertex.position.y - bottomY) / uiElementHeight);
                vertexList[i] = uiVertex;
            }
        }
    }
}