// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:5664,x:32973,y:32765,varname:node_5664,prsc:2|emission-5845-OUT,alpha-9710-OUT;n:type:ShaderForge.SFN_Tex2d,id:9705,x:32198,y:32924,ptovrint:False,ptlb:node_9705,ptin:_node_9705,varname:node_9705,prsc:2,tex:9e8117220bb32cc459da7bb0226344cd,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1717,x:32524,y:32839,varname:node_1717,prsc:2|A-8844-OUT,B-9705-RGB;n:type:ShaderForge.SFN_Vector3,id:8844,x:32297,y:32721,varname:node_8844,prsc:2,v1:0.9698365,v2:1.014706,v3:0.201449;n:type:ShaderForge.SFN_Multiply,id:9710,x:32758,y:33103,varname:node_9710,prsc:2|A-9705-G,B-3141-OUT;n:type:ShaderForge.SFN_Multiply,id:3025,x:32122,y:33489,varname:node_3025,prsc:2|A-9664-T,B-3589-OUT;n:type:ShaderForge.SFN_Time,id:9664,x:31796,y:33478,varname:node_9664,prsc:2;n:type:ShaderForge.SFN_Vector1,id:3589,x:31944,y:33683,varname:node_3589,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Sin,id:9797,x:32420,y:33505,varname:node_9797,prsc:2|IN-3025-OUT;n:type:ShaderForge.SFN_Abs,id:3141,x:32607,y:33466,varname:node_3141,prsc:2|IN-9797-OUT;n:type:ShaderForge.SFN_Multiply,id:5845,x:32733,y:32867,varname:node_5845,prsc:2|A-478-OUT,B-1717-OUT;n:type:ShaderForge.SFN_Vector1,id:478,x:32587,y:32760,varname:node_478,prsc:2,v1:0.5;proporder:9705;pass:END;sub:END;*/

Shader "Shader Forge/gou_g1" {
    Properties {
        _node_9705 ("node_9705", 2D) = "white" {}
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
            uniform sampler2D _node_9705; uniform float4 _node_9705_ST;
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
                float4 _node_9705_var = tex2D(_node_9705,TRANSFORM_TEX(i.uv0, _node_9705));
                float3 emissive = (0.5*(float3(0.9698365,1.014706,0.201449)*_node_9705_var.rgb));
                float3 finalColor = emissive;
                float4 node_9664 = _Time + _TimeEditor;
                fixed4 finalRGBA = fixed4(finalColor,(_node_9705_var.g*abs(sin((node_9664.g*1.5)))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
