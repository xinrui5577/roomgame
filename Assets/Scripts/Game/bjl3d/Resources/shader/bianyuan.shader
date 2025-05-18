// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:2,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:6953,x:33277,y:32691,varname:node_6953,prsc:2|diff-5448-OUT,spec-3089-OUT,gloss-5153-OUT,emission-1896-OUT;n:type:ShaderForge.SFN_Vector1,id:3089,x:32902,y:32578,varname:node_3089,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Slider,id:5153,x:32802,y:32809,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5153,prsc:2,min:0,cur:0.5470086,max:1;n:type:ShaderForge.SFN_Tex2d,id:8115,x:32284,y:32607,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_8115,prsc:2,tex:fee9517ac6a582b49a27cee766f27764,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Blend,id:1199,x:32507,y:32544,varname:node_1199,prsc:2,blmd:1,clmp:True|SRC-8115-RGB,DST-6518-RGB;n:type:ShaderForge.SFN_Multiply,id:5448,x:32644,y:32751,varname:node_5448,prsc:2|A-1199-OUT,B-2073-OUT;n:type:ShaderForge.SFN_Multiply,id:1896,x:32996,y:33005,varname:node_1896,prsc:2|A-5448-OUT,B-4233-OUT;n:type:ShaderForge.SFN_Vector1,id:2073,x:32476,y:32801,varname:node_2073,prsc:2,v1:0.6;n:type:ShaderForge.SFN_Color,id:6518,x:32193,y:32887,ptovrint:False,ptlb:Tint,ptin:_Tint,varname:node_6518,prsc:2,glob:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Power,id:4233,x:32868,y:33117,varname:node_4233,prsc:2|VAL-2580-OUT,EXP-7164-OUT;n:type:ShaderForge.SFN_Multiply,id:2580,x:32733,y:32992,varname:node_2580,prsc:2|A-3739-OUT,B-9932-OUT;n:type:ShaderForge.SFN_Slider,id:7164,x:32343,y:33219,ptovrint:False,ptlb:Bright,ptin:_Bright,varname:node_7164,prsc:2,min:-2,cur:0,max:1;n:type:ShaderForge.SFN_Fresnel,id:3739,x:32476,y:32920,varname:node_3739,prsc:2;n:type:ShaderForge.SFN_Slider,id:9932,x:32227,y:33063,ptovrint:False,ptlb:v,ptin:_v,varname:_Bright_copy,prsc:2,min:0,cur:2.391633,max:3;proporder:5153-6518-8115-7164-9932;pass:END;sub:END;*/

Shader "Shader Forge/bianyuan" {
    Properties {
        _Gloss ("Gloss", Range(0, 1)) = 0.5470086
        _Tint ("Tint", Color) = (1,1,1,1)
        _Texture ("Texture", 2D) = "white" {}
        _Bright ("Bright", Range(-2, 1)) = 0
        _v ("v", Range(0, 3)) = 2.391633
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
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform float _Gloss;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _Tint;
            uniform float _Bright;
            uniform float _v;
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
                UNITY_FOG_COORDS(5)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD6;
                #else
                    float3 shLight : TEXCOORD6;
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
                TRANSFER_VERTEX_TO_FRAGMENT(o)
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
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
///////// Gloss:
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float node_3089 = 0.4;
                float3 specularColor = float3(node_3089,node_3089,node_3089);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float3 node_5448 = (saturate((_Texture_var.rgb*_Tint.rgb))*0.6);
                float3 diffuse = (directDiffuse + indirectDiffuse) * node_5448;
////// Emissive:
                float3 emissive = (node_5448*pow(((1.0-max(0,dot(normalDirection, viewDirection)))*_v),_Bright));
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
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
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform float _Gloss;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _Tint;
            uniform float _Bright;
            uniform float _v;
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
                float gloss = _Gloss;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float node_3089 = 0.4;
                float3 specularColor = float3(node_3089,node_3089,node_3089);
                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow);
                float3 specular = directSpecular * specularColor;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float3 node_5448 = (saturate((_Texture_var.rgb*_Tint.rgb))*0.6);
                float3 diffuse = directDiffuse * node_5448;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
