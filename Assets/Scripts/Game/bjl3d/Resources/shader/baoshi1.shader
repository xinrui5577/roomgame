// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,dith:2,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:2629,x:33628,y:32655,varname:node_2629,prsc:2|diff-2743-RGB,emission-2659-OUT,alpha-3858-OUT;n:type:ShaderForge.SFN_Tex2d,id:2743,x:32472,y:32736,ptovrint:False,ptlb:node_2743,ptin:_node_2743,varname:node_2743,prsc:2,tex:41366e56ce01bd74e90399d99922993c,ntxv:0,isnm:False|UVIN-8554-OUT;n:type:ShaderForge.SFN_TexCoord,id:6750,x:32064,y:32762,varname:node_6750,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:8554,x:32246,y:32877,varname:node_8554,prsc:2|A-6750-UVOUT,B-6942-OUT;n:type:ShaderForge.SFN_Vector1,id:6942,x:32106,y:33059,varname:node_6942,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6843,x:33240,y:33122,varname:node_6843,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Multiply,id:8622,x:32702,y:32787,varname:node_8622,prsc:2|A-8302-OUT,B-2743-RGB;n:type:ShaderForge.SFN_Vector1,id:8302,x:32579,y:32613,varname:node_8302,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Multiply,id:3858,x:32963,y:32858,varname:node_3858,prsc:2|A-2743-A,B-3319-OUT;n:type:ShaderForge.SFN_Slider,id:3319,x:32380,y:32999,ptovrint:False,ptlb:node_3319,ptin:_node_3319,varname:node_3319,prsc:2,min:0,cur:1.310792,max:2;n:type:ShaderForge.SFN_Tex2d,id:8421,x:31944,y:33272,ptovrint:False,ptlb:node_8421,ptin:_node_8421,varname:node_8421,prsc:2,tex:51b93b25da97ad04fbf6c40bbf5ce971,ntxv:0,isnm:False|UVIN-2558-UVOUT;n:type:ShaderForge.SFN_Panner,id:2558,x:31735,y:33299,varname:node_2558,prsc:2,spu:-0.25,spv:0;n:type:ShaderForge.SFN_Multiply,id:1279,x:32277,y:33195,varname:node_1279,prsc:2|A-5145-OUT,B-8421-RGB;n:type:ShaderForge.SFN_Vector1,id:5145,x:32044,y:33165,varname:node_5145,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Add,id:8081,x:32674,y:33139,varname:node_8081,prsc:2|A-8622-OUT,B-1279-OUT;n:type:ShaderForge.SFN_Tex2d,id:8189,x:32684,y:33618,ptovrint:False,ptlb:node_8421_copy_copy,ptin:_node_8421_copy_copy,varname:_node_8421_copy_copy,prsc:2,tex:acaa8904c5cb1cb41a40459e701abf6f,ntxv:0,isnm:False|UVIN-8783-UVOUT;n:type:ShaderForge.SFN_Panner,id:8783,x:32475,y:33645,varname:node_8783,prsc:2,spu:0.25,spv:0;n:type:ShaderForge.SFN_Multiply,id:187,x:32925,y:33604,varname:node_187,prsc:2|A-8701-OUT,B-8189-RGB;n:type:ShaderForge.SFN_Vector1,id:8701,x:32784,y:33511,varname:node_8701,prsc:2,v1:0.7;n:type:ShaderForge.SFN_Add,id:1773,x:33029,y:33338,varname:node_1773,prsc:2|A-8622-OUT,B-187-OUT;n:type:ShaderForge.SFN_Add,id:2951,x:33094,y:33009,varname:node_2951,prsc:2|A-8081-OUT,B-1773-OUT;n:type:ShaderForge.SFN_Multiply,id:2659,x:33386,y:32951,varname:node_2659,prsc:2|A-2951-OUT,B-6843-OUT;proporder:2743-3319-8421-8189;pass:END;sub:END;*/

Shader "Shader Forge/baoshi1" {
    Properties {
        _node_2743 ("node_2743", 2D) = "white" {}
        _node_3319 ("node_3319", Range(0, 2)) = 1.310792
        _node_8421 ("node_8421", 2D) = "white" {}
        _node_8421_copy_copy ("node_8421_copy_copy", 2D) = "white" {}
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
            uniform sampler2D _node_2743; uniform float4 _node_2743_ST;
            uniform float _node_3319;
            uniform sampler2D _node_8421; uniform float4 _node_8421_ST;
            uniform sampler2D _node_8421_copy_copy; uniform float4 _node_8421_copy_copy_ST;
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
                float2 node_8554 = (i.uv0*1.0);
                float4 _node_2743_var = tex2D(_node_2743,TRANSFORM_TEX(node_8554, _node_2743));
                float3 diffuse = (directDiffuse + indirectDiffuse) * _node_2743_var.rgb;
////// Emissive:
                float3 node_8622 = (0.3*_node_2743_var.rgb);
                float4 node_331 = _Time + _TimeEditor;
                float2 node_2558 = (i.uv0+node_331.g*float2(-0.25,0));
                float4 _node_8421_var = tex2D(_node_8421,TRANSFORM_TEX(node_2558, _node_8421));
                float2 node_8783 = (i.uv0+node_331.g*float2(0.25,0));
                float4 _node_8421_copy_copy_var = tex2D(_node_8421_copy_copy,TRANSFORM_TEX(node_8783, _node_8421_copy_copy));
                float3 emissive = (((node_8622+(0.7*_node_8421_var.rgb))+(node_8622+(0.7*_node_8421_copy_copy_var.rgb)))*0.7);
/// Final Color:
                float3 finalColor = diffuse + emissive;
                fixed4 finalRGBA = fixed4(finalColor,(_node_2743_var.a*_node_3319));
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
            uniform sampler2D _node_2743; uniform float4 _node_2743_ST;
            uniform float _node_3319;
            uniform sampler2D _node_8421; uniform float4 _node_8421_ST;
            uniform sampler2D _node_8421_copy_copy; uniform float4 _node_8421_copy_copy_ST;
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
                float2 node_8554 = (i.uv0*1.0);
                float4 _node_2743_var = tex2D(_node_2743,TRANSFORM_TEX(node_8554, _node_2743));
                float3 diffuse = directDiffuse * _node_2743_var.rgb;
/// Final Color:
                float3 finalColor = diffuse;
                return fixed4(finalColor * (_node_2743_var.a*_node_3319),0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
