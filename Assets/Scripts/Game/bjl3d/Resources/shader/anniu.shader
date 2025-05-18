// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:1,culm:0,dpts:2,wrdp:True,dith:2,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:7969,x:32808,y:32489,varname:node_7969,prsc:2|diff-3032-RGB,emission-8225-OUT;n:type:ShaderForge.SFN_Tex2d,id:3032,x:31806,y:32410,ptovrint:False,ptlb:node_3032,ptin:_node_3032,varname:node_3032,prsc:2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3281,x:31975,y:32918,varname:node_3281,prsc:2|A-4453-OUT,B-6917-RGB;n:type:ShaderForge.SFN_Slider,id:583,x:32358,y:33301,ptovrint:False,ptlb:mmm,ptin:_mmm,varname:node_583,prsc:2,min:0,cur:0.4861612,max:2;n:type:ShaderForge.SFN_Tex2d,id:6917,x:31710,y:32973,ptovrint:False,ptlb:node_6917,ptin:_node_6917,varname:node_6917,prsc:2,tex:1727988d196bf2d4da0b7b61c0e41dbb,ntxv:0,isnm:False|UVIN-694-UVOUT;n:type:ShaderForge.SFN_Panner,id:694,x:31491,y:32843,varname:node_694,prsc:2,spu:0.06,spv:0|UVIN-7863-OUT;n:type:ShaderForge.SFN_Tex2d,id:7081,x:30814,y:32816,ptovrint:False,ptlb:node_7081,ptin:_node_7081,varname:node_7081,prsc:2,tex:e65ae57b61a1a204aa405493f91b9c2e,ntxv:0,isnm:False|UVIN-1396-UVOUT;n:type:ShaderForge.SFN_Panner,id:2661,x:30315,y:33005,varname:node_2661,prsc:2,spu:-0.1,spv:0;n:type:ShaderForge.SFN_Multiply,id:2155,x:31039,y:32816,varname:node_2155,prsc:2|A-9220-OUT,B-7081-R;n:type:ShaderForge.SFN_Slider,id:9220,x:30731,y:32671,ptovrint:False,ptlb:node_9220,ptin:_node_9220,varname:node_9220,prsc:2,min:0,cur:0.07343207,max:1;n:type:ShaderForge.SFN_Add,id:7863,x:31288,y:32843,varname:node_7863,prsc:2|A-8507-UVOUT,B-2155-OUT;n:type:ShaderForge.SFN_TexCoord,id:8507,x:31079,y:32653,varname:node_8507,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector3,id:4453,x:31797,y:32813,varname:node_4453,prsc:2,v1:0.9926471,v2:0.7386461,v3:0.3576449;n:type:ShaderForge.SFN_Multiply,id:8225,x:32570,y:32960,varname:node_8225,prsc:2|A-2602-OUT,B-583-OUT;n:type:ShaderForge.SFN_Add,id:2602,x:32462,y:32676,varname:node_2602,prsc:2|A-364-OUT,B-4783-OUT;n:type:ShaderForge.SFN_Multiply,id:364,x:32244,y:32507,varname:node_364,prsc:2|A-3032-RGB,B-1006-OUT;n:type:ShaderForge.SFN_Vector1,id:1006,x:32057,y:32646,varname:node_1006,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Multiply,id:4783,x:32351,y:32902,varname:node_4783,prsc:2|A-4593-OUT,B-708-OUT;n:type:ShaderForge.SFN_Slider,id:708,x:32157,y:33201,ptovrint:False,ptlb:node_708,ptin:_node_708,varname:node_708,prsc:2,min:0,cur:1.712689,max:2;n:type:ShaderForge.SFN_Multiply,id:4403,x:32008,y:33356,varname:node_4403,prsc:2|A-7121-OUT,B-6033-RGB;n:type:ShaderForge.SFN_Tex2d,id:6033,x:31750,y:33412,ptovrint:False,ptlb:node_6917_copy,ptin:_node_6917_copy,varname:_node_6917_copy,prsc:2,tex:39504a790595f1944a660c4516d9931b,ntxv:0,isnm:False|UVIN-7191-UVOUT;n:type:ShaderForge.SFN_Panner,id:7191,x:31521,y:33369,varname:node_7191,prsc:2,spu:-0.06,spv:0|UVIN-2463-OUT;n:type:ShaderForge.SFN_Tex2d,id:6777,x:30854,y:33255,ptovrint:False,ptlb:node_7081_copy,ptin:_node_7081_copy,varname:_node_7081_copy,prsc:2,tex:e65ae57b61a1a204aa405493f91b9c2e,ntxv:0,isnm:False|UVIN-6620-UVOUT;n:type:ShaderForge.SFN_Panner,id:8703,x:30589,y:33140,varname:node_8703,prsc:2,spu:0.1,spv:0;n:type:ShaderForge.SFN_Multiply,id:5119,x:31079,y:33255,varname:node_5119,prsc:2|A-6470-OUT,B-6777-R;n:type:ShaderForge.SFN_Slider,id:6470,x:30771,y:33110,ptovrint:False,ptlb:node_9220_copy,ptin:_node_9220_copy,varname:_node_9220_copy,prsc:2,min:0,cur:0.07343207,max:1;n:type:ShaderForge.SFN_Add,id:2463,x:31328,y:33282,varname:node_2463,prsc:2|A-7486-UVOUT,B-5119-OUT;n:type:ShaderForge.SFN_TexCoord,id:7486,x:31119,y:33092,varname:node_7486,prsc:2,uv:0;n:type:ShaderForge.SFN_Vector3,id:7121,x:31837,y:33252,varname:node_7121,prsc:2,v1:0.9926471,v2:0.7386461,v3:0.3576449;n:type:ShaderForge.SFN_Add,id:4593,x:32095,y:33050,varname:node_4593,prsc:2|A-3281-OUT,B-4403-OUT;n:type:ShaderForge.SFN_Rotator,id:6529,x:31457,y:33007,varname:node_6529,prsc:2|UVIN-7863-OUT,SPD-9922-OUT;n:type:ShaderForge.SFN_Vector1,id:9922,x:31328,y:33165,varname:node_9922,prsc:2,v1:-0.1;n:type:ShaderForge.SFN_Rotator,id:6790,x:31609,y:33195,varname:node_6790,prsc:2|UVIN-2463-OUT,SPD-9922-OUT;n:type:ShaderForge.SFN_Rotator,id:1396,x:30527,y:32791,varname:node_1396,prsc:2|UVIN-2767-UVOUT,SPD-5442-OUT;n:type:ShaderForge.SFN_Vector1,id:5442,x:30295,y:32877,varname:node_5442,prsc:2,v1:-0.05;n:type:ShaderForge.SFN_TexCoord,id:2767,x:30325,y:32707,varname:node_2767,prsc:2,uv:0;n:type:ShaderForge.SFN_Rotator,id:6620,x:30605,y:33322,varname:node_6620,prsc:2|UVIN-8694-UVOUT,SPD-8397-OUT;n:type:ShaderForge.SFN_Vector1,id:8397,x:30346,y:33392,varname:node_8397,prsc:2,v1:0.05;n:type:ShaderForge.SFN_TexCoord,id:8694,x:30379,y:33249,varname:node_8694,prsc:2,uv:0;proporder:3032-583-6917-7081-9220-708-6033-6777-6470;pass:END;sub:END;*/

Shader "Shader Forge/anniu" {
    Properties {
        _node_3032 ("node_3032", 2D) = "white" {}
        _mmm ("mmm", Range(0, 2)) = 0.4861612
        _node_6917 ("node_6917", 2D) = "white" {}
        _node_7081 ("node_7081", 2D) = "white" {}
        _node_9220 ("node_9220", Range(0, 1)) = 0.07343207
        _node_708 ("node_708", Range(0, 2)) = 1.712689
        _node_6917_copy ("node_6917_copy", 2D) = "white" {}
        _node_7081_copy ("node_7081_copy", 2D) = "white" {}
        _node_9220_copy ("node_9220_copy", Range(0, 1)) = 0.07343207
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
            uniform float4 _TimeEditor;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _node_3032; uniform float4 _node_3032_ST;
            uniform float _mmm;
            uniform sampler2D _node_6917; uniform float4 _node_6917_ST;
            uniform sampler2D _node_7081; uniform float4 _node_7081_ST;
            uniform float _node_9220;
            uniform float _node_708;
            uniform sampler2D _node_6917_copy; uniform float4 _node_6917_copy_ST;
            uniform sampler2D _node_7081_copy; uniform float4 _node_7081_copy_ST;
            uniform float _node_9220_copy;
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
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float4 _node_3032_var = tex2D(_node_3032,TRANSFORM_TEX(i.uv0, _node_3032));
                float3 diffuse = (directDiffuse + indirectDiffuse) * _node_3032_var.rgb;
////// Emissive:
                float4 node_1879 = _Time + _TimeEditor;
                float node_1396_ang = node_1879.g;
                float node_1396_spd = (-0.05);
                float node_1396_cos = cos(node_1396_spd*node_1396_ang);
                float node_1396_sin = sin(node_1396_spd*node_1396_ang);
                float2 node_1396_piv = float2(0.5,0.5);
                float2 node_1396 = (mul(i.uv0-node_1396_piv,float2x2( node_1396_cos, -node_1396_sin, node_1396_sin, node_1396_cos))+node_1396_piv);
                float4 _node_7081_var = tex2D(_node_7081,TRANSFORM_TEX(node_1396, _node_7081));
                float2 node_7863 = (i.uv0+(_node_9220*_node_7081_var.r));
                float2 node_694 = (node_7863+node_1879.g*float2(0.06,0));
                float4 _node_6917_var = tex2D(_node_6917,TRANSFORM_TEX(node_694, _node_6917));
                float node_6620_ang = node_1879.g;
                float node_6620_spd = 0.05;
                float node_6620_cos = cos(node_6620_spd*node_6620_ang);
                float node_6620_sin = sin(node_6620_spd*node_6620_ang);
                float2 node_6620_piv = float2(0.5,0.5);
                float2 node_6620 = (mul(i.uv0-node_6620_piv,float2x2( node_6620_cos, -node_6620_sin, node_6620_sin, node_6620_cos))+node_6620_piv);
                float4 _node_7081_copy_var = tex2D(_node_7081_copy,TRANSFORM_TEX(node_6620, _node_7081_copy));
                float2 node_2463 = (i.uv0+(_node_9220_copy*_node_7081_copy_var.r));
                float2 node_7191 = (node_2463+node_1879.g*float2(-0.06,0));
                float4 _node_6917_copy_var = tex2D(_node_6917_copy,TRANSFORM_TEX(node_7191, _node_6917_copy));
                float3 emissive = (((_node_3032_var.rgb*0.2)+(((float3(0.9926471,0.7386461,0.3576449)*_node_6917_var.rgb)+(float3(0.9926471,0.7386461,0.3576449)*_node_6917_copy_var.rgb))*_node_708))*_mmm);
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
            uniform float4 _TimeEditor;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform sampler2D _node_3032; uniform float4 _node_3032_ST;
            uniform float _mmm;
            uniform sampler2D _node_6917; uniform float4 _node_6917_ST;
            uniform sampler2D _node_7081; uniform float4 _node_7081_ST;
            uniform float _node_9220;
            uniform float _node_708;
            uniform sampler2D _node_6917_copy; uniform float4 _node_6917_copy_ST;
            uniform sampler2D _node_7081_copy; uniform float4 _node_7081_copy_ST;
            uniform float _node_9220_copy;
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
                float4 _node_3032_var = tex2D(_node_3032,TRANSFORM_TEX(i.uv0, _node_3032));
                float3 diffuse = directDiffuse * _node_3032_var.rgb;
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
