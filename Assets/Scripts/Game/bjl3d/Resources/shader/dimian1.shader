// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9385,x:33162,y:32899,varname:node_9385,prsc:2|diff-5135-OUT,emission-4285-OUT,alpha-5699-OUT;n:type:ShaderForge.SFN_Tex2d,id:3995,x:31412,y:32990,ptovrint:False,ptlb:dd,ptin:_dd,varname:node_3995,prsc:2,tex:8d3ef445d49ab3f4995215e3f93b1ef5,ntxv:0,isnm:False|UVIN-9103-UVOUT;n:type:ShaderForge.SFN_Vector3,id:5135,x:32102,y:32590,varname:node_5135,prsc:2,v1:0,v2:0,v3:0;n:type:ShaderForge.SFN_Desaturate,id:6226,x:31596,y:32911,varname:node_6226,prsc:2|COL-3995-RGB;n:type:ShaderForge.SFN_Multiply,id:7610,x:31902,y:32852,varname:node_7610,prsc:2|A-5539-OUT,B-6226-OUT;n:type:ShaderForge.SFN_Vector3,id:5539,x:31646,y:32719,varname:node_5539,prsc:2,v1:0,v2:0.2965517,v3:1;n:type:ShaderForge.SFN_Rotator,id:9103,x:31199,y:32894,varname:node_9103,prsc:2|UVIN-1475-UVOUT;n:type:ShaderForge.SFN_Multiply,id:8227,x:32176,y:32813,varname:node_8227,prsc:2|A-2599-OUT,B-7610-OUT;n:type:ShaderForge.SFN_Vector1,id:2599,x:31859,y:32704,varname:node_2599,prsc:2,v1:0.6;n:type:ShaderForge.SFN_Tex2d,id:8509,x:31808,y:33114,ptovrint:False,ptlb:node_8509,ptin:_node_8509,varname:node_8509,prsc:2,tex:f7167e36f8a661b45b656bbf5d9cf7e9,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4285,x:32358,y:32952,varname:node_4285,prsc:2|A-8227-OUT,B-8509-RGB;n:type:ShaderForge.SFN_Multiply,id:2603,x:31977,y:33257,varname:node_2603,prsc:2|A-3995-A,B-8509-A;n:type:ShaderForge.SFN_Add,id:1501,x:32155,y:33092,varname:node_1501,prsc:2|A-3995-B,B-8509-A;n:type:ShaderForge.SFN_TexCoord,id:1475,x:31002,y:32758,varname:node_1475,prsc:2,uv:0;n:type:ShaderForge.SFN_Time,id:9577,x:31740,y:33546,varname:node_9577,prsc:2;n:type:ShaderForge.SFN_Multiply,id:3197,x:32029,y:33527,varname:node_3197,prsc:2|A-865-OUT,B-9577-T;n:type:ShaderForge.SFN_Vector1,id:865,x:31836,y:33475,varname:node_865,prsc:2,v1:2;n:type:ShaderForge.SFN_Add,id:4484,x:32469,y:33154,varname:node_4484,prsc:2|A-1501-OUT,B-4506-A;n:type:ShaderForge.SFN_Tex2d,id:4506,x:32253,y:33235,ptovrint:False,ptlb:node_4506,ptin:_node_4506,varname:node_4506,prsc:2,tex:f7167e36f8a661b45b656bbf5d9cf7e9,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:5699,x:32952,y:33369,varname:node_5699,prsc:2|A-4484-OUT,B-931-OUT;n:type:ShaderForge.SFN_Sin,id:3566,x:32228,y:33527,varname:node_3566,prsc:2|IN-3197-OUT;n:type:ShaderForge.SFN_Multiply,id:931,x:32674,y:33372,varname:node_931,prsc:2|A-4484-OUT,B-3566-OUT;n:type:ShaderForge.SFN_Cos,id:2514,x:32424,y:33554,varname:node_2514,prsc:2|IN-3566-OUT;proporder:3995-8509-4506;pass:END;sub:END;*/

Shader "Shader Forge/dimian1" {
    Properties {
        _dd ("dd", 2D) = "white" {}
        _node_8509 ("node_8509", 2D) = "white" {}
        _node_4506 ("node_4506", 2D) = "white" {}
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
            uniform sampler2D _dd; uniform float4 _dd_ST;
            uniform sampler2D _node_8509; uniform float4 _node_8509_ST;
            uniform sampler2D _node_4506; uniform float4 _node_4506_ST;
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
                float3 diffuse = (directDiffuse + indirectDiffuse) * float3(0,0,0);
////// Emissive:
                float4 node_5194 = _Time + _TimeEditor;
                float node_9103_ang = node_5194.g;
                float node_9103_spd = 1.0;
                float node_9103_cos = cos(node_9103_spd*node_9103_ang);
                float node_9103_sin = sin(node_9103_spd*node_9103_ang);
                float2 node_9103_piv = float2(0.5,0.5);
                float2 node_9103 = (mul(i.uv0-node_9103_piv,float2x2( node_9103_cos, -node_9103_sin, node_9103_sin, node_9103_cos))+node_9103_piv);
                float4 _dd_var = tex2D(_dd,TRANSFORM_TEX(node_9103, _dd));
                float4 _node_8509_var = tex2D(_node_8509,TRANSFORM_TEX(i.uv0, _node_8509));
                float3 emissive = ((0.6*(float3(0,0.2965517,1)*dot(_dd_var.rgb,float3(0.3,0.59,0.11))))*_node_8509_var.rgb);
/// Final Color:
                float3 finalColor = diffuse + emissive;
                float4 _node_4506_var = tex2D(_node_4506,TRANSFORM_TEX(i.uv0, _node_4506));
                float node_4484 = ((_dd_var.b+_node_8509_var.a)+_node_4506_var.a);
                float4 node_9577 = _Time + _TimeEditor;
                float node_3566 = sin((2.0*node_9577.g));
                fixed4 finalRGBA = fixed4(finalColor,(node_4484+(node_4484*node_3566)));
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
            uniform sampler2D _dd; uniform float4 _dd_ST;
            uniform sampler2D _node_8509; uniform float4 _node_8509_ST;
            uniform sampler2D _node_4506; uniform float4 _node_4506_ST;
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
                float3 diffuse = directDiffuse * float3(0,0,0);
/// Final Color:
                float3 finalColor = diffuse;
                float4 node_8584 = _Time + _TimeEditor;
                float node_9103_ang = node_8584.g;
                float node_9103_spd = 1.0;
                float node_9103_cos = cos(node_9103_spd*node_9103_ang);
                float node_9103_sin = sin(node_9103_spd*node_9103_ang);
                float2 node_9103_piv = float2(0.5,0.5);
                float2 node_9103 = (mul(i.uv0-node_9103_piv,float2x2( node_9103_cos, -node_9103_sin, node_9103_sin, node_9103_cos))+node_9103_piv);
                float4 _dd_var = tex2D(_dd,TRANSFORM_TEX(node_9103, _dd));
                float4 _node_8509_var = tex2D(_node_8509,TRANSFORM_TEX(i.uv0, _node_8509));
                float4 _node_4506_var = tex2D(_node_4506,TRANSFORM_TEX(i.uv0, _node_4506));
                float node_4484 = ((_dd_var.b+_node_8509_var.a)+_node_4506_var.a);
                float4 node_9577 = _Time + _TimeEditor;
                float node_3566 = sin((2.0*node_9577.g));
                return fixed4(finalColor * (node_4484+(node_4484*node_3566)),0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
