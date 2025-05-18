// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:6876,x:33032,y:32794,varname:node_6876,prsc:2|diff-1763-RGB,emission-4754-OUT,alpha-1763-A;n:type:ShaderForge.SFN_Tex2d,id:1763,x:32562,y:32713,ptovrint:False,ptlb:node_1763,ptin:_node_1763,varname:node_1763,prsc:2,tex:ddd8f048c4dd5bb499ffe3fdd648e047,ntxv:0,isnm:False|UVIN-2509-OUT;n:type:ShaderForge.SFN_Tex2d,id:802,x:31950,y:32789,ptovrint:False,ptlb:node_802,ptin:_node_802,varname:node_802,prsc:2,ntxv:0,isnm:False|UVIN-7044-UVOUT;n:type:ShaderForge.SFN_Panner,id:7044,x:31630,y:32773,varname:node_7044,prsc:2,spu:0.2,spv:0;n:type:ShaderForge.SFN_Multiply,id:8728,x:32222,y:32817,varname:node_8728,prsc:2|A-8576-OUT,B-802-R;n:type:ShaderForge.SFN_Slider,id:8576,x:31984,y:32525,ptovrint:False,ptlb:node_8576,ptin:_node_8576,varname:node_8576,prsc:2,min:0,cur:0.04609354,max:1;n:type:ShaderForge.SFN_Add,id:2509,x:32369,y:32730,varname:node_2509,prsc:2|A-7226-UVOUT,B-8728-OUT;n:type:ShaderForge.SFN_TexCoord,id:7226,x:32399,y:32494,varname:node_7226,prsc:2,uv:0;n:type:ShaderForge.SFN_Tex2d,id:5035,x:32332,y:33005,ptovrint:False,ptlb:node_5035,ptin:_node_5035,varname:node_5035,prsc:2,tex:c5d495978210f26428b18166f375e9dd,ntxv:0,isnm:False|UVIN-6269-UVOUT;n:type:ShaderForge.SFN_Panner,id:6269,x:32069,y:33055,varname:node_6269,prsc:2,spu:-1,spv:0;n:type:ShaderForge.SFN_Add,id:4754,x:32739,y:32882,varname:node_4754,prsc:2|A-1763-RGB,B-5035-RGB;proporder:1763-802-8576-5035;pass:END;sub:END;*/

Shader "Shader Forge/y1" {
    Properties {
        _node_1763 ("node_1763", 2D) = "white" {}
        _node_802 ("node_802", 2D) = "white" {}
        _node_8576 ("node_8576", Range(0, 1)) = 0.04609354
        _node_5035 ("node_5035", 2D) = "white" {}
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
            uniform sampler2D _node_1763; uniform float4 _node_1763_ST;
            uniform sampler2D _node_802; uniform float4 _node_802_ST;
            uniform float _node_8576;
            uniform sampler2D _node_5035; uniform float4 _node_5035_ST;
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
                float4 node_4283 = _Time + _TimeEditor;
                float2 node_7044 = (i.uv0+node_4283.g*float2(0.2,0));
                float4 _node_802_var = tex2D(_node_802,TRANSFORM_TEX(node_7044, _node_802));
                float2 node_2509 = (i.uv0+(_node_8576*_node_802_var.r));
                float4 _node_1763_var = tex2D(_node_1763,TRANSFORM_TEX(node_2509, _node_1763));
                float3 diffuse = (directDiffuse + indirectDiffuse) * _node_1763_var.rgb;
////// Emissive:
                float2 node_6269 = (i.uv0+node_4283.g*float2(-1,0));
                float4 _node_5035_var = tex2D(_node_5035,TRANSFORM_TEX(node_6269, _node_5035));
                float3 emissive = (_node_1763_var.rgb+_node_5035_var.rgb);
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,_node_1763_var.a);
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
            uniform sampler2D _node_1763; uniform float4 _node_1763_ST;
            uniform sampler2D _node_802; uniform float4 _node_802_ST;
            uniform float _node_8576;
            uniform sampler2D _node_5035; uniform float4 _node_5035_ST;
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
                float4 node_5349 = _Time + _TimeEditor;
                float2 node_7044 = (i.uv0+node_5349.g*float2(0.2,0));
                float4 _node_802_var = tex2D(_node_802,TRANSFORM_TEX(node_7044, _node_802));
                float2 node_2509 = (i.uv0+(_node_8576*_node_802_var.r));
                float4 _node_1763_var = tex2D(_node_1763,TRANSFORM_TEX(node_2509, _node_1763));
                float3 diffuse = directDiffuse * _node_1763_var.rgb;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * _node_1763_var.a,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
