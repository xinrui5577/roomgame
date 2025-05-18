using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game.brnn3d
{
    public class FindShader : MonoBehaviour
    {
        public bool IsImage = true;
        public bool IsParticle = false;
        public string ShaderName;

        void Awake()
        {
            if (IsImage)
            {
                Image img = this.gameObject.GetComponent<Image>();
                img.material.shader = Shader.Find(ShaderName);

            }
            else if (IsParticle)
            {
                Renderer render = GetComponent<Renderer>();
                render.material.shader = Shader.Find(ShaderName);
            }
            else
            {
                Material mat = this.GetComponent<MeshRenderer>().material;
                mat.shader = Shader.Find(ShaderName);
            }

        }


    }
}
