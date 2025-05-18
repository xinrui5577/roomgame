// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:33171,y:32623,varname:node_3138,prsc:2|normal-6919-OUT,alpha-3390-OUT,refract-6806-OUT;n:type:ShaderForge.SFN_Slider,id:3390,x:32664,y:32608,ptovrint:False,ptlb:touming,ptin:_touming,varname:node_1544,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:6919,x:32613,y:32717,varname:node_6919,prsc:2|A-5156-OUT,B-1237-RGB,T-8151-A;n:type:ShaderForge.SFN_Multiply,id:6806,x:32709,y:33008,varname:node_6806,prsc:2|A-8653-OUT,B-5056-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8653,x:32415,y:32927,varname:node_8653,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1237-RGB;n:type:ShaderForge.SFN_Multiply,id:5056,x:32498,y:33168,varname:node_5056,prsc:2|A-8151-A,B-130-OUT;n:type:ShaderForge.SFN_Vector3,id:5156,x:32337,y:32510,varname:node_5156,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Tex2d,id:1237,x:32181,y:32654,ptovrint:False,ptlb:node_6930,ptin:_node_6930,varname:_node_6930,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:8e8aa0f21d5a2c1479646df62d7d9f69,ntxv:3,isnm:False|UVIN-9193-UVOUT,MIP-1520-OUT;n:type:ShaderForge.SFN_TexCoord,id:9193,x:31868,y:32577,varname:node_9193,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:1520,x:31811,y:32898,ptovrint:False,ptlb:niuquqiangdu,ptin:_niuquqiangdu,varname:_node_8387,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.160604,max:6;n:type:ShaderForge.SFN_VertexColor,id:8151,x:31790,y:33013,varname:node_8151,prsc:2;n:type:ShaderForge.SFN_Slider,id:130,x:31907,y:33292,ptovrint:False,ptlb:fanshefanwei,ptin:_fanshefanwei,varname:_node_6810,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;proporder:1237-1520-130-3390;pass:END;sub:END;*/

Shader "Shader Forge/niuqu-tiaoxing" {
    Properties {
        _node_6930 ("node_6930", 2D) = "bump" {}
        _niuquqiangdu ("niuquqiangdu", Range(0, 6)) = 1.160604
        _fanshefanwei ("fanshefanwei", Range(0, 1)) = 0.2
        _touming ("touming", Range(0, 1)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        GrabPass{ }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _GrabTexture;
            uniform float _touming;
            uniform sampler2D _node_6930; uniform float4 _node_6930_ST;
            uniform float _niuquqiangdu;
            uniform float _fanshefanwei;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float3 tangentDir : TEXCOORD2;
                float3 bitangentDir : TEXCOORD3;
                float4 screenPos : TEXCOORD4;
                float4 vertexColor : COLOR;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.normalDir = normalize(i.normalDir);
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float4 _node_6930_var = tex2Dlod(_node_6930,float4(TRANSFORM_TEX(i.uv0, _node_6930),0.0,_niuquqiangdu));
                float3 normalLocal = lerp(float3(0,0,1),_node_6930_var.rgb,i.vertexColor.a);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_node_6930_var.rgb.rg*(i.vertexColor.a*_fanshefanwei));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                return fixed4(lerp(sceneColor.rgb, finalColor,_touming),1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
