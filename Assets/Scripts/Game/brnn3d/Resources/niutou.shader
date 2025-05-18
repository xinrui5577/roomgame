// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7681,x:32990,y:32720,varname:node_7681,prsc:2|emission-4659-OUT,alpha-5734-A;n:type:ShaderForge.SFN_Tex2d,id:5734,x:32506,y:32693,ptovrint:False,ptlb:node_5734,ptin:_node_5734,varname:node_5734,prsc:2,tex:9aafd806346a44243843171fed1261ce,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9259,x:32172,y:32992,ptovrint:False,ptlb:node_9259,ptin:_node_9259,varname:node_9259,prsc:2,tex:ee94a97386fad2e4c9c21edf574604ee,ntxv:0,isnm:False|UVIN-1779-UVOUT;n:type:ShaderForge.SFN_Panner,id:1779,x:31926,y:33018,varname:node_1779,prsc:2,spu:0,spv:-0.5|UVIN-8664-OUT;n:type:ShaderForge.SFN_Add,id:4659,x:32723,y:32861,varname:node_4659,prsc:2|A-5734-RGB,B-3036-OUT;n:type:ShaderForge.SFN_Multiply,id:3036,x:32401,y:32957,varname:node_3036,prsc:2|A-9259-RGB,B-667-OUT;n:type:ShaderForge.SFN_Vector3,id:667,x:32172,y:33243,varname:node_667,prsc:2,v1:1,v2:0.7610548,v3:0.5441177;n:type:ShaderForge.SFN_Tex2d,id:2806,x:31402,y:33059,ptovrint:False,ptlb:node_2806,ptin:_node_2806,varname:node_2806,prsc:2,tex:3f7a3153736f09b4287f34949975e617,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9142,x:31621,y:33100,varname:node_9142,prsc:2|A-1199-OUT,B-2806-R;n:type:ShaderForge.SFN_Slider,id:1199,x:31313,y:32869,ptovrint:False,ptlb:node_1199,ptin:_node_1199,varname:node_1199,prsc:2,min:0,cur:0.1232023,max:1;n:type:ShaderForge.SFN_Add,id:8664,x:31769,y:33014,varname:node_8664,prsc:2|A-2426-UVOUT,B-9142-OUT;n:type:ShaderForge.SFN_TexCoord,id:2426,x:31636,y:32795,varname:node_2426,prsc:2,uv:0;proporder:5734-9259-2806-1199;pass:END;sub:END;*/

Shader "Shader Forge/niutou" {
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
                float4 node_4134 = _Time + _TimeEditor;
                float4 _node_2806_var = tex2D(_node_2806,TRANSFORM_TEX(i.uv0, _node_2806));
                float2 node_1779 = ((i.uv0+(_node_1199*_node_2806_var.r))+node_4134.g*float2(0,-0.5));
                float4 _node_9259_var = tex2D(_node_9259,TRANSFORM_TEX(node_1779, _node_9259));
                float3 emissive = (_node_5734_var.rgb+(_node_9259_var.rgb*float3(1,0.7610548,0.5441177)));
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
