// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.28 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.28;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:1,spmd:0,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:7,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:4013,x:32944,y:32596,varname:node_4013,prsc:2|normal-7218-OUT,alpha-1544-OUT,refract-7533-OUT;n:type:ShaderForge.SFN_Tex2d,id:6930,x:32119,y:32589,ptovrint:False,ptlb:node_6930,ptin:_node_6930,varname:_node_6930,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:c27df28103fbab74c9375f0cc60ffddc,ntxv:2,isnm:False|UVIN-5428-UVOUT,MIP-8387-OUT;n:type:ShaderForge.SFN_VertexColor,id:8684,x:31726,y:32949,varname:node_8684,prsc:2;n:type:ShaderForge.SFN_ComponentMask,id:8696,x:32351,y:32863,varname:node_8696,prsc:2,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-6930-RGB;n:type:ShaderForge.SFN_Multiply,id:7533,x:32645,y:32944,varname:node_7533,prsc:2|A-8696-OUT,B-752-OUT;n:type:ShaderForge.SFN_Multiply,id:752,x:32434,y:33104,varname:node_752,prsc:2|A-8684-A,B-6810-OUT;n:type:ShaderForge.SFN_TexCoord,id:5428,x:31806,y:32512,varname:node_5428,prsc:2,uv:0;n:type:ShaderForge.SFN_Slider,id:8387,x:31749,y:32833,ptovrint:False,ptlb:niuquqiangdu,ptin:_niuquqiangdu,varname:_node_8387,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1.160604,max:6;n:type:ShaderForge.SFN_Slider,id:6810,x:31843,y:33228,ptovrint:False,ptlb:fanshefanwei,ptin:_fanshefanwei,varname:_node_6810,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2,max:1;n:type:ShaderForge.SFN_Lerp,id:7218,x:32549,y:32653,varname:node_7218,prsc:2|A-3163-OUT,B-6930-RGB,T-8684-A;n:type:ShaderForge.SFN_Vector3,id:3163,x:32275,y:32445,varname:node_3163,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:1544,x:32600,y:32544,ptovrint:False,ptlb:touming,ptin:_touming,varname:node_1544,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;proporder:6930-8387-6810-1544;pass:END;sub:END;*/

Shader "Shader Forge/niuqu" {
    Properties {
        _node_6930 ("node_6930", 2D) = "black" {}
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
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            #pragma glsl
            uniform sampler2D _GrabTexture;
            uniform sampler2D _node_6930; uniform float4 _node_6930_ST;
            uniform float _niuquqiangdu;
            uniform float _fanshefanwei;
            uniform float _touming;
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
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.screenPos = o.pos;
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                #if UNITY_UV_STARTS_AT_TOP
                    float grabSign = -_ProjectionParams.x;
                #else
                    float grabSign = _ProjectionParams.x;
                #endif
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float4 _node_6930_var = tex2Dlod(_node_6930,float4(TRANSFORM_TEX(i.uv0, _node_6930),0.0,_niuquqiangdu));
                float3 normalLocal = lerp(float3(0,0,1),_node_6930_var.rgb,i.vertexColor.a);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float2 sceneUVs = float2(1,grabSign)*i.screenPos.xy*0.5+0.5 + (_node_6930_var.rgb.rg*(i.vertexColor.a*_fanshefanwei));
                float4 sceneColor = tex2D(_GrabTexture, sceneUVs);
////// Lighting:
                float3 finalColor = 0;
                fixed4 finalRGBA = fixed4(lerp(sceneColor.rgb, finalColor,_touming),1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
