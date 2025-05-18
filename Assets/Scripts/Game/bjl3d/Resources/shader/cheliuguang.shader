// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:2256,x:33131,y:32701,varname:node_2256,prsc:2|emission-8896-OUT,alpha-320-A;n:type:ShaderForge.SFN_Tex2d,id:320,x:32337,y:32679,ptovrint:False,ptlb:node_320,ptin:_node_320,varname:node_320,prsc:2,tex:c663a2e11e8b7da4fbc29e3971988726,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1047,x:32178,y:32975,ptovrint:False,ptlb:node_1047,ptin:_node_1047,varname:node_1047,prsc:2,tex:b4bb0fd810222ba46a53179d9ab26f12,ntxv:0,isnm:False|UVIN-7842-UVOUT;n:type:ShaderForge.SFN_Panner,id:7842,x:31969,y:32982,varname:node_7842,prsc:2,spu:0,spv:-5;n:type:ShaderForge.SFN_Add,id:8896,x:32727,y:32849,varname:node_8896,prsc:2|A-320-RGB,B-326-OUT;n:type:ShaderForge.SFN_Multiply,id:6913,x:32349,y:33040,varname:node_6913,prsc:2|A-1047-RGB,B-6733-OUT;n:type:ShaderForge.SFN_Vector3,id:6733,x:32144,y:33196,varname:node_6733,prsc:2,v1:0.7132353,v2:0.9288032,v3:1;n:type:ShaderForge.SFN_Multiply,id:326,x:32529,y:33061,varname:node_326,prsc:2|A-6913-OUT,B-9515-OUT;n:type:ShaderForge.SFN_Vector1,id:9515,x:32431,y:33244,varname:node_9515,prsc:2,v1:0.1;proporder:320-1047;pass:END;sub:END;*/

Shader "Shader Forge/cheliuguang" {
    Properties {
        _node_320 ("node_320", 2D) = "white" {}
        _node_1047 ("node_1047", 2D) = "white" {}
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
            uniform sampler2D _node_320; uniform float4 _node_320_ST;
            uniform sampler2D _node_1047; uniform float4 _node_1047_ST;
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
                float4 _node_320_var = tex2D(_node_320,TRANSFORM_TEX(i.uv0, _node_320));
                float4 node_2091 = _Time + _TimeEditor;
                float2 node_7842 = (i.uv0+node_2091.g*float2(0,-5));
                float4 _node_1047_var = tex2D(_node_1047,TRANSFORM_TEX(node_7842, _node_1047));
                float3 emissive = (_node_320_var.rgb+((_node_1047_var.rgb*float3(0.7132353,0.9288032,1))*0.1));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,_node_320_var.a);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
