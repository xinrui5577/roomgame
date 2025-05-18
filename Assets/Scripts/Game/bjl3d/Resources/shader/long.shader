// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:3354,x:32591,y:32794,varname:node_3354,prsc:2|emission-8538-OUT,alpha-7406-A;n:type:ShaderForge.SFN_Tex2d,id:7406,x:31887,y:32762,ptovrint:False,ptlb:node_5734,ptin:_node_5734,varname:node_5734,prsc:2,tex:9b7e4ec8c6e2a444bb2ea69cba8f1c23,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:2597,x:31789,y:33045,ptovrint:False,ptlb:node_9259,ptin:_node_9259,varname:node_9259,prsc:2,tex:a04f744b1c3e8a343b6048d17470c42a,ntxv:0,isnm:False|UVIN-141-UVOUT;n:type:ShaderForge.SFN_Panner,id:141,x:31579,y:33057,varname:node_141,prsc:2,spu:0,spv:-0.2|UVIN-6348-OUT;n:type:ShaderForge.SFN_Add,id:8538,x:32387,y:32918,varname:node_8538,prsc:2|A-2400-OUT,B-1196-OUT;n:type:ShaderForge.SFN_Multiply,id:1196,x:32076,y:32993,varname:node_1196,prsc:2|A-2597-RGB,B-9918-OUT;n:type:ShaderForge.SFN_Vector3,id:9918,x:31789,y:33296,varname:node_9918,prsc:2,v1:0.6397059,v2:1,v3:0.731643;n:type:ShaderForge.SFN_Tex2d,id:6597,x:31019,y:33112,ptovrint:False,ptlb:node_2806,ptin:_node_2806,varname:node_2806,prsc:2,tex:1c409aff5769b524eaee11aeab10f9fe,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2839,x:31215,y:33146,varname:node_2839,prsc:2|A-710-OUT,B-6597-R;n:type:ShaderForge.SFN_Slider,id:710,x:30930,y:32922,ptovrint:False,ptlb:node_1199,ptin:_node_1199,varname:node_1199,prsc:2,min:0,cur:0.1232023,max:1;n:type:ShaderForge.SFN_Add,id:6348,x:31386,y:33067,varname:node_6348,prsc:2|A-1955-UVOUT,B-2839-OUT;n:type:ShaderForge.SFN_TexCoord,id:1955,x:31253,y:32848,varname:node_1955,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:2400,x:32163,y:32767,varname:node_2400,prsc:2|A-4831-OUT,B-7406-RGB;n:type:ShaderForge.SFN_Vector1,id:4831,x:32026,y:32643,varname:node_4831,prsc:2,v1:0.8;proporder:7406-2597-6597-710;pass:END;sub:END;*/

Shader "Shader Forge/long" {
    Properties {
        _node_5734 ("node_5734", 2D) = "white" {}
        _node_9259 ("node_9259", 2D) = "white" {}
        _node_2806 ("node_2806", 2D) = "white" {}
        _node_1199 ("node_1199", Range(0, 1)) = 0.1232023
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
            uniform sampler2D _node_5734; uniform float4 _node_5734_ST;
            uniform sampler2D _node_9259; uniform float4 _node_9259_ST;
            uniform sampler2D _node_2806; uniform float4 _node_2806_ST;
            uniform float _node_1199;
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
                float4 _node_5734_var = tex2D(_node_5734,TRANSFORM_TEX(i.uv0, _node_5734));
                float4 node_4009 = _Time + _TimeEditor;
                float4 _node_2806_var = tex2D(_node_2806,TRANSFORM_TEX(i.uv0, _node_2806));
                float2 node_141 = ((i.uv0+(_node_1199*_node_2806_var.r))+node_4009.g*float2(0,-0.2));
                float4 _node_9259_var = tex2D(_node_9259,TRANSFORM_TEX(node_141, _node_9259));
                float3 emissive = ((0.8*_node_5734_var.rgb)+(_node_9259_var.rgb*float3(0.6397059,1,0.731643)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,_node_5734_var.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
