// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:2,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:8688,x:32981,y:32698,varname:node_8688,prsc:2|emission-8689-OUT;n:type:ShaderForge.SFN_Tex2d,id:9352,x:32436,y:32670,ptovrint:False,ptlb:node_9352,ptin:_node_9352,varname:node_9352,prsc:2,tex:82c1a0513537b6b409b83dcbe74f5497,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:9724,x:32266,y:32933,ptovrint:False,ptlb:node_9724,ptin:_node_9724,varname:node_9724,prsc:2,tex:0eb28d5171cde0844873b16e78602261,ntxv:0,isnm:False|UVIN-1253-UVOUT;n:type:ShaderForge.SFN_Panner,id:1253,x:32011,y:32929,varname:node_1253,prsc:2,spu:0,spv:-0.2|UVIN-4989-OUT;n:type:ShaderForge.SFN_Tex2d,id:6694,x:31315,y:32902,ptovrint:False,ptlb:node_6694,ptin:_node_6694,varname:node_6694,prsc:2,tex:f3eef8828c51c6a4b95d607377ed8d28,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:684,x:31638,y:32943,varname:node_684,prsc:2|A-1274-OUT,B-6694-R;n:type:ShaderForge.SFN_Slider,id:1274,x:31427,y:32702,ptovrint:False,ptlb:node_1274,ptin:_node_1274,varname:node_1274,prsc:2,min:0,cur:0.1003048,max:1;n:type:ShaderForge.SFN_Add,id:4989,x:31838,y:32979,varname:node_4989,prsc:2|A-2982-UVOUT,B-684-OUT;n:type:ShaderForge.SFN_TexCoord,id:2982,x:31847,y:32647,varname:node_2982,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:8518,x:32477,y:33060,varname:node_8518,prsc:2|A-9724-RGB,B-3391-OUT;n:type:ShaderForge.SFN_Vector3,id:3391,x:32339,y:33250,varname:node_3391,prsc:2,v1:1,v2:0.837931,v3:0.3088235;n:type:ShaderForge.SFN_Add,id:8689,x:32681,y:32777,varname:node_8689,prsc:2|A-9352-RGB,B-1568-OUT;n:type:ShaderForge.SFN_Multiply,id:1568,x:32659,y:33195,varname:node_1568,prsc:2|A-8518-OUT,B-3850-OUT;n:type:ShaderForge.SFN_Vector1,id:3850,x:32499,y:33303,varname:node_3850,prsc:2,v1:1;proporder:9352-9724-6694-1274;pass:END;sub:END;*/

Shader "Shader Forge/HundredBeef_SZ" {
    Properties {
        _node_9352 ("node_9352", 2D) = "white" {}
        _node_9724 ("node_9724", 2D) = "white" {}
        _node_6694 ("node_6694", 2D) = "white" {}
        _node_1274 ("node_1274", Range(0, 1)) = 0.1003048
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
            uniform sampler2D _node_9352; uniform float4 _node_9352_ST;
            uniform sampler2D _node_9724; uniform float4 _node_9724_ST;
            uniform sampler2D _node_6694; uniform float4 _node_6694_ST;
            uniform float _node_1274;
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
                float4 _node_9352_var = tex2D(_node_9352,TRANSFORM_TEX(i.uv0, _node_9352));
                float4 node_8325 = _Time + _TimeEditor;
                float4 _node_6694_var = tex2D(_node_6694,TRANSFORM_TEX(i.uv0, _node_6694));
                float2 node_1253 = ((i.uv0+(_node_1274*_node_6694_var.r))+node_8325.g*float2(0,-0.2));
                float4 _node_9724_var = tex2D(_node_9724,TRANSFORM_TEX(node_1253, _node_9724));
                float3 emissive = (_node_9352_var.rgb+((_node_9724_var.rgb*float3(1,0.837931,0.3088235))*1.0));
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
