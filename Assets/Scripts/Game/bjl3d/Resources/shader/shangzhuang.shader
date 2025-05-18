// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7672,x:33778,y:32657,varname:node_7672,prsc:2|emission-9329-OUT,alpha-6046-OUT;n:type:ShaderForge.SFN_Tex2d,id:5170,x:32498,y:32828,ptovrint:False,ptlb:node_5170,ptin:_node_5170,varname:node_5170,prsc:2,tex:7ed77bed8e65fb340bddd267729e748b,ntxv:0,isnm:False|UVIN-3199-UVOUT;n:type:ShaderForge.SFN_Panner,id:3199,x:32230,y:32861,varname:node_3199,prsc:2,spu:0,spv:-0.3;n:type:ShaderForge.SFN_Tex2d,id:1887,x:32583,y:32550,ptovrint:False,ptlb:node_1887,ptin:_node_1887,varname:node_1887,prsc:2,tex:e2bed9d71841331469c2a4e1aca5d408,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3694,x:33024,y:32966,varname:node_3694,prsc:2|A-1887-A,B-5170-A;n:type:ShaderForge.SFN_Multiply,id:6046,x:33360,y:33149,varname:node_6046,prsc:2|A-3694-OUT,B-3351-R;n:type:ShaderForge.SFN_Multiply,id:9329,x:33320,y:32729,varname:node_9329,prsc:2|A-3506-OUT,B-5170-RGB;n:type:ShaderForge.SFN_Slider,id:3506,x:32965,y:32595,ptovrint:False,ptlb:node_3506,ptin:_node_3506,varname:node_3506,prsc:2,min:0,cur:0.676089,max:1;n:type:ShaderForge.SFN_Tex2d,id:3351,x:32859,y:33379,ptovrint:False,ptlb:node_3351,ptin:_node_3351,varname:node_3351,prsc:2,tex:912f6e27cf58ae248b31edc7b53edcc4,ntxv:0,isnm:False;proporder:5170-1887-3506-3351;pass:END;sub:END;*/

Shader "Shader Forge/shangzhuang" {
    Properties {
        _node_5170 ("node_5170", 2D) = "white" {}
        _node_1887 ("node_1887", 2D) = "white" {}
        _node_3506 ("node_3506", Range(0, 1)) = 0.676089
        _node_3351 ("node_3351", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _node_5170; uniform float4 _node_5170_ST;
            uniform sampler2D _node_1887; uniform float4 _node_1887_ST;
            uniform float _node_3506;
            uniform sampler2D _node_3351; uniform float4 _node_3351_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD2;
                #else
                    float3 shLight : TEXCOORD2;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
/////// Vectors:
////// Lighting:
////// Emissive:
                float4 node_4118 = _Time + _TimeEditor;
                float2 node_3199 = (i.uv0+node_4118.g*float2(0,-0.3));
                float4 _node_5170_var = tex2D(_node_5170,TRANSFORM_TEX(node_3199, _node_5170));
                float3 emissive = (_node_3506*_node_5170_var.rgb);
                float3 finalColor = emissive;
                float4 _node_1887_var = tex2D(_node_1887,TRANSFORM_TEX(i.uv0, _node_1887));
                float4 _node_3351_var = tex2D(_node_3351,TRANSFORM_TEX(i.uv0, _node_3351));
                fixed4 finalRGBA = fixed4(finalColor,((_node_1887_var.a*_node_5170_var.a)*_node_3351_var.r));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
