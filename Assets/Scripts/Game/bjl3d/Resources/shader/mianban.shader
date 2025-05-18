// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:8319,x:33739,y:32379,varname:node_8319,prsc:2|diff-926-OUT,emission-9939-OUT;n:type:ShaderForge.SFN_Vector3,id:8462,x:32686,y:32358,varname:node_8462,prsc:2,v1:0.2145329,v2:0.3924471,v3:0.9117647;n:type:ShaderForge.SFN_Multiply,id:9939,x:32885,y:32196,varname:node_9939,prsc:2|A-1629-RGB,B-8462-OUT;n:type:ShaderForge.SFN_Tex2d,id:1629,x:32593,y:32156,ptovrint:False,ptlb:node_1629,ptin:_node_1629,varname:node_1629,prsc:2,tex:8075b9ea47c5c6f44b0502b3122cf20a,ntxv:0,isnm:False|UVIN-4896-OUT;n:type:ShaderForge.SFN_Tex2d,id:4908,x:31710,y:32095,ptovrint:False,ptlb:node_4908,ptin:_node_4908,varname:node_4908,prsc:2,tex:8075b9ea47c5c6f44b0502b3122cf20a,ntxv:0,isnm:False|UVIN-8071-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:7462,x:31710,y:32284,ptovrint:False,ptlb:node_7462,ptin:_node_7462,varname:node_7462,prsc:2,tex:41282416516956e4b9cc70b9fc10ffd5,ntxv:0,isnm:False|UVIN-8694-UVOUT;n:type:ShaderForge.SFN_Add,id:9851,x:31962,y:32218,varname:node_9851,prsc:2|A-4908-R,B-7462-R;n:type:ShaderForge.SFN_Panner,id:8071,x:31379,y:32114,varname:node_8071,prsc:2,spu:-0.04,spv:0.05;n:type:ShaderForge.SFN_Panner,id:8694,x:31403,y:32301,varname:node_8694,prsc:2,spu:0.04,spv:-0.05;n:type:ShaderForge.SFN_Multiply,id:9304,x:32173,y:32188,varname:node_9304,prsc:2|A-9553-OUT,B-9851-OUT;n:type:ShaderForge.SFN_Slider,id:9553,x:31901,y:31994,ptovrint:False,ptlb:node_9553,ptin:_node_9553,varname:node_9553,prsc:2,min:0,cur:0.1045537,max:1;n:type:ShaderForge.SFN_Add,id:4896,x:32320,y:32216,varname:node_4896,prsc:2|A-2502-UVOUT,B-9304-OUT;n:type:ShaderForge.SFN_TexCoord,id:2502,x:32267,y:32023,varname:node_2502,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector3,id:926,x:33082,y:32493,varname:node_926,prsc:2,v1:1,v2:0.623428,v3:0.4191176;proporder:1629-4908-7462-9553;pass:END;sub:END;*/

Shader "Shader Forge/mianban" {
    Properties {
        _node_1629 ("node_1629", 2D) = "white" {}
        _node_4908 ("node_4908", 2D) = "white" {}
        _node_7462 ("node_7462", 2D) = "white" {}
        _node_9553 ("node_9553", Range(0, 1)) = 0.1045537
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
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _node_1629; uniform float4 _node_1629_ST;
            uniform sampler2D _node_4908; uniform float4 _node_4908_ST;
            uniform sampler2D _node_7462; uniform float4 _node_7462_ST;
            uniform float _node_9553;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD4;
                #else
                    float3 shLight : TEXCOORD4;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * float3(1,0.623428,0.4191176);
////// Emissive:
                float4 node_3685 = _Time + _TimeEditor;
                float2 node_8071 = (i.uv0+node_3685.g*float2(-0.04,0.05));
                float4 _node_4908_var = tex2D(_node_4908,TRANSFORM_TEX(node_8071, _node_4908));
                float2 node_8694 = (i.uv0+node_3685.g*float2(0.04,-0.05));
                float4 _node_7462_var = tex2D(_node_7462,TRANSFORM_TEX(node_8694, _node_7462));
                float2 node_4896 = (i.uv0+(_node_9553*(_node_4908_var.r+_node_7462_var.r)));
                float4 _node_1629_var = tex2D(_node_1629,TRANSFORM_TEX(node_4896, _node_1629));
                float3 emissive = (_node_1629_var.rgb*float3(0.2145329,0.3924471,0.9117647));
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _node_1629; uniform float4 _node_1629_ST;
            uniform sampler2D _node_4908; uniform float4 _node_4908_ST;
            uniform sampler2D _node_7462; uniform float4 _node_7462_ST;
            uniform float _node_9553;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD5;
                #else
                    float3 shLight : TEXCOORD5;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuse = directDiffuse * float3(1,0.623428,0.4191176);
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
