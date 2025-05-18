// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:6075,x:32496,y:32678,varname:node_6075,prsc:2|diff-2597-RGB,spec-9601-OUT,gloss-1678-OUT,emission-5655-OUT,alpha-236-A;n:type:ShaderForge.SFN_Tex2d,id:2597,x:31855,y:32486,ptovrint:False,ptlb:node_2597,ptin:_node_2597,varname:node_2597,prsc:2,tex:370f5a4df50fcd641bda4900630fb068,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:6135,x:31714,y:32816,ptovrint:False,ptlb:node_6135,ptin:_node_6135,varname:node_6135,prsc:2,tex:39504a790595f1944a660c4516d9931b,ntxv:0,isnm:False|UVIN-5770-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1782,x:31032,y:32802,ptovrint:False,ptlb:node_1782,ptin:_node_1782,varname:node_1782,prsc:2,tex:41282416516956e4b9cc70b9fc10ffd5,ntxv:0,isnm:False|UVIN-7225-UVOUT;n:type:ShaderForge.SFN_Panner,id:5770,x:31513,y:32794,varname:node_5770,prsc:2,spu:0.1,spv:-0.1|UVIN-5149-OUT;n:type:ShaderForge.SFN_Multiply,id:2699,x:31901,y:32816,varname:node_2699,prsc:2|A-4559-OUT,B-6135-RGB;n:type:ShaderForge.SFN_Vector3,id:4559,x:31736,y:32643,varname:node_4559,prsc:2,v1:1,v2:0.5425964,v3:0.1911765;n:type:ShaderForge.SFN_Multiply,id:8307,x:31190,y:32819,varname:node_8307,prsc:2|A-8579-OUT,B-1782-R;n:type:ShaderForge.SFN_Slider,id:8579,x:30932,y:32644,ptovrint:False,ptlb:node_8579,ptin:_node_8579,varname:node_8579,prsc:2,min:0,cur:0.1794874,max:1;n:type:ShaderForge.SFN_Add,id:5149,x:31361,y:32774,varname:node_5149,prsc:2|A-3891-UVOUT,B-8307-OUT;n:type:ShaderForge.SFN_TexCoord,id:3891,x:31303,y:32618,varname:node_3891,prsc:2,uv:0;n:type:ShaderForge.SFN_Panner,id:7225,x:30772,y:32802,varname:node_7225,prsc:2,spu:-0.03,spv:0.03;n:type:ShaderForge.SFN_Tex2d,id:236,x:31569,y:33041,ptovrint:False,ptlb:node_236,ptin:_node_236,varname:node_236,prsc:2,tex:e4e747c26da90984f95a22f3ec4bd42b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:5655,x:32207,y:32922,varname:node_5655,prsc:2|A-3375-OUT,B-188-OUT;n:type:ShaderForge.SFN_Vector1,id:5595,x:31966,y:32746,varname:node_5595,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Multiply,id:9601,x:32249,y:32689,varname:node_9601,prsc:2|A-4559-OUT,B-5595-OUT;n:type:ShaderForge.SFN_Vector1,id:1678,x:32249,y:32868,varname:node_1678,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:3375,x:32067,y:33047,varname:node_3375,prsc:2|A-2699-OUT,B-4922-OUT;n:type:ShaderForge.SFN_Vector1,id:4922,x:32029,y:33181,varname:node_4922,prsc:2,v1:3;n:type:ShaderForge.SFN_Multiply,id:188,x:31864,y:32992,varname:node_188,prsc:2|A-236-RGB,B-3971-OUT;n:type:ShaderForge.SFN_Vector1,id:3971,x:31701,y:33228,varname:node_3971,prsc:2,v1:0.35;proporder:2597-6135-1782-8579-236;pass:END;sub:END;*/

Shader "Shader Forge/hongbaoshi" {
    Properties {
        _node_2597 ("node_2597", 2D) = "white" {}
        _node_6135 ("node_6135", 2D) = "white" {}
        _node_1782 ("node_1782", 2D) = "white" {}
        _node_8579 ("node_8579", Range(0, 1)) = 0.1794874
        _node_236 ("node_236", 2D) = "white" {}
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
            uniform sampler2D _node_2597; uniform float4 _node_2597_ST;
            uniform sampler2D _node_6135; uniform float4 _node_6135_ST;
            uniform sampler2D _node_1782; uniform float4 _node_1782_ST;
            uniform float _node_8579;
            uniform sampler2D _node_236; uniform float4 _node_236_ST;
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 node_4559 = float3(1,0.5425964,0.1911765);
                float3 specularColor = (node_4559*1.5);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _node_2597_var = tex2D(_node_2597,TRANSFORM_TEX(i.uv0, _node_2597));
                float3 diffuse = (directDiffuse + indirectDiffuse) * _node_2597_var.rgb;
////// Emissive:
                float4 node_6530 = _Time + _TimeEditor;
                float2 node_7225 = (i.uv0+node_6530.g*float2(-0.03,0.03));
                float4 _node_1782_var = tex2D(_node_1782,TRANSFORM_TEX(node_7225, _node_1782));
                float2 node_5770 = ((i.uv0+(_node_8579*_node_1782_var.r))+node_6530.g*float2(0.1,-0.1));
                float4 _node_6135_var = tex2D(_node_6135,TRANSFORM_TEX(node_5770, _node_6135));
                float4 _node_236_var = tex2D(_node_236,TRANSFORM_TEX(i.uv0, _node_236));
                float3 emissive = (((node_4559*_node_6135_var.rgb)*3.0)*(_node_236_var.rgb*0.35));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                fixed4 finalRGBA = fixed4(finalColor,_node_236_var.a);
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
            uniform sampler2D _node_2597; uniform float4 _node_2597_ST;
            uniform sampler2D _node_6135; uniform float4 _node_6135_ST;
            uniform sampler2D _node_1782; uniform float4 _node_1782_ST;
            uniform float _node_8579;
            uniform sampler2D _node_236; uniform float4 _node_236_ST;
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
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float3 node_4559 = float3(1,0.5425964,0.1911765);
                float3 specularColor = (node_4559*1.5);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _node_2597_var = tex2D(_node_2597,TRANSFORM_TEX(i.uv0, _node_2597));
                float3 diffuse = directDiffuse * _node_2597_var.rgb;
/// Final Color:
                float3 finalColor = diffuse + specular;
                float4 _node_236_var = tex2D(_node_236,TRANSFORM_TEX(i.uv0, _node_236));
                return fixed4(finalColor * _node_236_var.a,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
