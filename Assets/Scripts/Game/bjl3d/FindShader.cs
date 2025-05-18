using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.bjl3d
{
    public class FindShader : MonoBehaviour
    {
        /// <summary>
        /// 是图像
        /// </summary>
        public bool IsImage = true;
        /// <summary>
        /// 是粒子
        /// </summary>
        public bool IsParticle = false;
        public string ShaderName;

        protected void Awake()
        {

            if (IsImage)
            {
                var img = gameObject.GetComponent<Image>();
                img.material.shader = Shader.Find(ShaderName);

            }
            else if (IsParticle)
            {
                Renderer render = GetComponent<Renderer>();
                render.material.shader = Shader.Find(ShaderName);
            }
            else
            {
                Material mat = GetComponent<MeshRenderer>().material;
                mat.shader = Shader.Find(ShaderName);
            }

        }


    }
}
