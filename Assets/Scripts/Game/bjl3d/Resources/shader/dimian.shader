// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7077,x:32939,y:32698,varname:node_7077,prsc:2|emission-7743-OUT,alpha-5447-OUT;n:type:ShaderForge.SFN_Tex2d,id:4555,x:32093,y:32716,ptovrint:False,ptlb:node_4555,ptin:_node_4555,varname:node_4555,prsc:2,tex:bf1f8aa390a23924faa4d580d82667bf,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1253,x:32303,y:32974,ptovrint:False,ptlb:node_1253,ptin:_node_1253,varname:node_1253,prsc:2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:8908,x:32043,y:33231,varname:node_8908,prsc:2|A-7794-T,B-2626-OUT;n:type:ShaderForge.SFN_Time,id:7794,x:31701,y:33165,varname:node_7794,prsc:2;n:type:ShaderForge.SFN_Vector1,id:2626,x:31766,y:33394,varname:node_2626,prsc:2,v1:2;n:type:ShaderForge.SFN_Sin,id:8646,x:32232,y:33271,varname:node_8646,prsc:2|IN-8908-OUT;n:type:ShaderForge.SFN_Multiply,id:2698,x:32568,y:33154,varname:node_2698,prsc:2|A-1253-R,B-8646-OUT;n:type:ShaderForge.SFN_Add,id:5447,x:32754,y:33008,varname:node_5447,prsc:2|A-6316-OUT,B-2698-OUT;n:type:ShaderForge.SFN_Multiply,id:6316,x:32600,y:32938,varname:node_6316,prsc:2|A-604-OUT,B-1253-R;n:type:ShaderForge.SFN_Vector1,id:604,x:32449,y:32898,varname:node_604,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:7743,x:32634,y:32708,varname:node_7743,prsc:2|A-3947-OUT,B-4555-RGB;n:type:ShaderForge.SFN_Vector1,id:8727,x:32382,y:32672,varname:node_8727,prsc:2,v1:1.2;n:type:ShaderForge.SFN_Slider,id:3947,x:32282,y:32587,ptovrint:False,ptlb:node_3947,ptin:_node_3947,varname:node_3947,prsc:2,min:0,cur:0.5213675,max:1;proporder:4555-1253-3947;pass:END;sub:END;*/

Shader "Shader Forge/dimian" {
    Properties {
        _node_4555 ("node_4555", 2D) = "white" {}
        _node_1253 ("node_1253", 2D) = "white" {}
        _node_3947 ("node_3947", Range(0, 1)) = 0.5213675
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
            uniform sampler2D _node_4555; uniform float4 _node_4555_ST;
            uniform sampler2D _node_1253; uniform float4 _node_1253_ST;
            uniform float _node_3947;
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
                float4 _node_4555_var = tex2D(_node_4555,TRANSFORM_TEX(i.uv0, _node_4555));
                float3 emissive = (_node_3947*_node_4555_var.rgb);
                float3 finalColor = emissive;
                float4 _node_1253_var = tex2D(_node_1253,TRANSFORM_TEX(i.uv0, _node_1253));
                float4 node_7794 = _Time + _TimeEditor;
                fixed4 finalRGBA = fixed4(finalColor,((0.2*_node_1253_var.r)+(_node_1253_var.r*sin((node_7794.g*2.0)))));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
