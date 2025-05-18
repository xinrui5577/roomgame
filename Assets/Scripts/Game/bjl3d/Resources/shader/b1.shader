// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:555,x:32961,y:32734,varname:node_555,prsc:2|diff-5266-RGB,emission-7807-OUT,alpha-5266-A;n:type:ShaderForge.SFN_Tex2d,id:5266,x:32465,y:32698,ptovrint:False,ptlb:node_5266,ptin:_node_5266,varname:node_5266,prsc:2,tex:232eabb61090fbf4dbb0f2eb4c49adfa,ntxv:0,isnm:False|UVIN-9278-OUT;n:type:ShaderForge.SFN_Multiply,id:7807,x:32758,y:32739,varname:node_7807,prsc:2|A-1851-OUT,B-5266-RGB;n:type:ShaderForge.SFN_Vector1,id:1851,x:32574,y:32537,varname:node_1851,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Tex2d,id:9593,x:31895,y:32846,ptovrint:False,ptlb:node_9593,ptin:_node_9593,varname:node_9593,prsc:2,tex:c4d21005f46759a48aae84cb6356b194,ntxv:0,isnm:False|UVIN-43-UVOUT;n:type:ShaderForge.SFN_Panner,id:43,x:31586,y:32784,varname:node_43,prsc:2,spu:-0.1,spv:0;n:type:ShaderForge.SFN_Multiply,id:9147,x:32131,y:32875,varname:node_9147,prsc:2|A-4953-OUT,B-9593-R;n:type:ShaderForge.SFN_Slider,id:4953,x:31844,y:32511,ptovrint:False,ptlb:node_4953,ptin:_node_4953,varname:node_4953,prsc:2,min:0,cur:0.04975982,max:1;n:type:ShaderForge.SFN_Add,id:9278,x:32288,y:32684,varname:node_9278,prsc:2|A-2080-UVOUT,B-9147-OUT;n:type:ShaderForge.SFN_TexCoord,id:2080,x:32225,y:32517,varname:node_2080,prsc:2,uv:0;proporder:5266-9593-4953;pass:END;sub:END;*/

Shader "Shader Forge/b1" {
    Properties {
        _node_5266 ("node_5266", 2D) = "white" {}
        _node_9593 ("node_9593", 2D) = "white" {}
        _node_4953 ("node_4953", Range(0, 1)) = 0.04975982
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
            uniform float4 _LightColor0;
            uniform float4 _TimeEditor;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _node_5266; uniform float4 _node_5266_ST;
            uniform sampler2D _node_9593; uniform float4 _node_9593_ST;
            uniform float _node_4953;
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
                float4 node_2757 = _Time + _TimeEditor;
                float2 node_43 = (i.uv0+node_2757.g*float2(-0.1,0));
                float4 _node_9593_var = tex2D(_node_9593,TRANSFORM_TEX(node_43, _node_9593));
                float2 node_9278 = (i.uv0+(_node_4953*_node_9593_var.r));
                float4 _node_5266_var = tex2D(_node_5266,TRANSFORM_TEX(node_9278, _node_5266));
                float3 diffuse = (directDiffuse + indirectDiffuse) * _node_5266_var.rgb;
////// Emissive:
                float3 emissive = (0.2*_node_5266_var.rgb);
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,_node_5266_var.a);
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
            uniform sampler2D _node_5266; uniform float4 _node_5266_ST;
            uniform sampler2D _node_9593; uniform float4 _node_9593_ST;
            uniform float _node_4953;
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
                float4 node_4580 = _Time + _TimeEditor;
                float2 node_43 = (i.uv0+node_4580.g*float2(-0.1,0));
                float4 _node_9593_var = tex2D(_node_9593,TRANSFORM_TEX(node_43, _node_9593));
                float2 node_9278 = (i.uv0+(_node_4953*_node_9593_var.r));
                float4 _node_5266_var = tex2D(_node_5266,TRANSFORM_TEX(node_9278, _node_5266));
                float3 diffuse = directDiffuse * _node_5266_var.rgb;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * _node_5266_var.a,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
