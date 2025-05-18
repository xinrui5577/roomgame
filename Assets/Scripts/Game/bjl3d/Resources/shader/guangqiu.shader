// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:2,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:803,x:33564,y:32617,varname:node_803,prsc:2|emission-3959-OUT;n:type:ShaderForge.SFN_Tex2d,id:8221,x:32516,y:32811,ptovrint:False,ptlb:node_8221,ptin:_node_8221,varname:node_8221,prsc:2,tex:8a7270dd70d76d3438b2f7ae176a1723,ntxv:0,isnm:False|UVIN-4111-UVOUT;n:type:ShaderForge.SFN_Panner,id:4111,x:32302,y:32711,varname:node_4111,prsc:2,spu:0.2,spv:0.2|UVIN-8387-OUT;n:type:ShaderForge.SFN_Tex2d,id:6460,x:31373,y:32850,ptovrint:False,ptlb:node_6460,ptin:_node_6460,varname:node_6460,prsc:2,tex:2c16a09d03ea42246a762b498a674a54,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:9727,x:31703,y:32783,varname:node_9727,prsc:2|A-6481-OUT,B-6460-R;n:type:ShaderForge.SFN_Slider,id:6481,x:31343,y:32637,ptovrint:False,ptlb:node_6481,ptin:_node_6481,varname:node_6481,prsc:2,min:0,cur:0.01743832,max:1;n:type:ShaderForge.SFN_Add,id:8387,x:32001,y:32624,varname:node_8387,prsc:2|A-7787-UVOUT,B-9727-OUT;n:type:ShaderForge.SFN_TexCoord,id:7787,x:31762,y:32429,varname:node_7787,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:9465,x:32227,y:33106,ptovrint:False,ptlb:node_9465,ptin:_node_9465,varname:node_9465,prsc:2,tex:8a7270dd70d76d3438b2f7ae176a1723,ntxv:0,isnm:False|UVIN-8397-UVOUT;n:type:ShaderForge.SFN_Panner,id:8397,x:32067,y:33091,varname:node_8397,prsc:2,spu:-0.2,spv:-0.2|UVIN-8387-OUT;n:type:ShaderForge.SFN_Add,id:3959,x:33111,y:32928,varname:node_3959,prsc:2|A-6972-OUT,B-3444-OUT;n:type:ShaderForge.SFN_Desaturate,id:6766,x:32453,y:33106,varname:node_6766,prsc:2|COL-9465-RGB;n:type:ShaderForge.SFN_Multiply,id:8957,x:32717,y:33221,varname:node_8957,prsc:2|A-6766-OUT,B-7968-OUT;n:type:ShaderForge.SFN_Vector3,id:7968,x:32427,y:33323,varname:node_7968,prsc:2,v1:0.5063905,v2:0.2262651,v3:0.9926471;n:type:ShaderForge.SFN_Desaturate,id:7506,x:32585,y:32609,varname:node_7506,prsc:2|COL-8221-RGB;n:type:ShaderForge.SFN_Multiply,id:2266,x:32881,y:32669,varname:node_2266,prsc:2|A-4850-OUT,B-7506-OUT;n:type:ShaderForge.SFN_Vector3,id:4850,x:32675,y:32464,varname:node_4850,prsc:2,v1:0.2481618,v2:0.3149085,v3:0.9926471;n:type:ShaderForge.SFN_Multiply,id:3444,x:32919,y:33141,varname:node_3444,prsc:2|A-9335-OUT,B-8957-OUT;n:type:ShaderForge.SFN_Vector1,id:9335,x:32688,y:33052,varname:node_9335,prsc:2,v1:3;n:type:ShaderForge.SFN_Multiply,id:6972,x:33041,y:32684,varname:node_6972,prsc:2|A-2266-OUT,B-9335-OUT;proporder:8221-6460-6481-9465;pass:END;sub:END;*/

Shader "Shader Forge/guangqiu" {
    Properties {
        _node_8221 ("node_8221", 2D) = "white" {}
        _node_6460 ("node_6460", 2D) = "white" {}
        _node_6481 ("node_6481", Range(0, 1)) = 0.01743832
        _node_9465 ("node_9465", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _node_8221; uniform float4 _node_8221_ST;
            uniform sampler2D _node_6460; uniform float4 _node_6460_ST;
            uniform float _node_6481;
            uniform sampler2D _node_9465; uniform float4 _node_9465_ST;
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
                float4 node_33 = _Time + _TimeEditor;
                float4 _node_6460_var = tex2D(_node_6460,TRANSFORM_TEX(i.uv0, _node_6460));
                float2 node_8387 = (i.uv0+(_node_6481*_node_6460_var.r));
                float2 node_4111 = (node_8387+node_33.g*float2(0.2,0.2));
                float4 _node_8221_var = tex2D(_node_8221,TRANSFORM_TEX(node_4111, _node_8221));
                float node_9335 = 3.0;
                float2 node_8397 = (node_8387+node_33.g*float2(-0.2,-0.2));
                float4 _node_9465_var = tex2D(_node_9465,TRANSFORM_TEX(node_8397, _node_9465));
                float3 emissive = (((float3(0.2481618,0.3149085,0.9926471)*dot(_node_8221_var.rgb,float3(0.3,0.59,0.11)))*node_9335)+(node_9335*(dot(_node_9465_var.rgb,float3(0.3,0.59,0.11))*float3(0.5063905,0.2262651,0.9926471))));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
