// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1917,x:33161,y:32755,varname:node_1917,prsc:2|emission-98-OUT,alpha-5995-OUT;n:type:ShaderForge.SFN_Tex2d,id:8066,x:32025,y:32775,ptovrint:False,ptlb:node_8066,ptin:_node_8066,varname:node_8066,prsc:2,tex:0d5aaaa846cfb0649a3eff9a7b9b9681,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5995,x:32844,y:33099,varname:node_5995,prsc:2|A-8066-A,B-9437-OUT;n:type:ShaderForge.SFN_Time,id:6489,x:32097,y:33233,varname:node_6489,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9816,x:32395,y:33263,varname:node_9816,prsc:2|A-6489-T,B-7960-OUT;n:type:ShaderForge.SFN_Vector1,id:7960,x:32173,y:33419,varname:node_7960,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Sin,id:2315,x:32606,y:33335,varname:node_2315,prsc:2|IN-9816-OUT;n:type:ShaderForge.SFN_Abs,id:9437,x:32738,y:33291,varname:node_9437,prsc:2|IN-2315-OUT;n:type:ShaderForge.SFN_Desaturate,id:3134,x:32221,y:32746,varname:node_3134,prsc:2|COL-8066-RGB;n:type:ShaderForge.SFN_Multiply,id:1442,x:32538,y:32823,varname:node_1442,prsc:2|A-8168-OUT,B-3134-OUT;n:type:ShaderForge.SFN_Vector3,id:8168,x:32329,y:32601,varname:node_8168,prsc:2,v1:1,v2:0.9768763,v3:0.1617647;n:type:ShaderForge.SFN_Multiply,id:98,x:32900,y:32611,varname:node_98,prsc:2|A-374-OUT,B-1442-OUT;n:type:ShaderForge.SFN_Vector1,id:374,x:32661,y:32550,varname:node_374,prsc:2,v1:1.5;proporder:8066;pass:END;sub:END;*/

Shader "Shader Forge/gou_g2" {
    Properties {
        _node_8066 ("node_8066", 2D) = "white" {}
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
            uniform sampler2D _node_8066; uniform float4 _node_8066_ST;
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
                float4 _node_8066_var = tex2D(_node_8066,TRANSFORM_TEX(i.uv0, _node_8066));
                float3 emissive = (1.5*(float3(1,0.9768763,0.1617647)*dot(_node_8066_var.rgb,float3(0.3,0.59,0.11))));
                float3 finalColor = emissive;
                float4 node_6489 = _Time + _TimeEditor;
                fixed4 finalRGBA = fixed4(finalColor,(_node_8066_var.a*abs(sin((node_6489.g*1.5)))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
