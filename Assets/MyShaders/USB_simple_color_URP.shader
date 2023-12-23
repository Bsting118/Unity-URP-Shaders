Shader "Unlit/USB_simple_color_URP"
//Shader "InspectorPath/ShaderName"

/********************************************
*  File: USB_simple_color_URP.shader        *
*  Programmed By: Brendan Sting             *
*  CS-320, Instructed by Professor Reeves   *
*  Date: 2-8-2023                           *
*********************************************/

//HW (TODO): Add a simple void function for fake lighting from section 4.0.4 in online textbook; submit in Blackboard
//HW2 (TODO #2): Convert this script from Cg to HLSL using section 4.6 in your online textbook; submit in Blackboard
{
    Properties
    {
        //All shader properties go in this field
        _MainTex("Texture", 2D) = "white" {}
        //With this, can now change color of material in Inspector
        _Color("Texture Color", Color) = (1,1,1,1)
        //Use Alpha channel in RGBA to adjust transparency (adjust subshader tags accordingly)
    }
        SubShader
    {
        //Subshader configuration in this field (ADJUSTED PER HW 4.0.6)
        Tags {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalRenderPipeline"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            //FOR HW (4.0.6) HLSL CONVERSION:
            //===============================
            HLSLPROGRAM
            // programa Cg - HLSL in this field

            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            //#include "UnityCG.cginc"
            #include "HLSLSupport.cginc"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                //Can connect ShaderGraph to project in Built-in RP to fix shader issues and code in HLSL (not really supposed to do this, but works)
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            //Global pass variables (use anywhere in functions):
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color; //Connection variable

            //FOR HW (4.0.4) FAKELIGHT IMPLEMENTATION:
            //========================================
            void FakeLight_float(in float3 Normal, out float3 Out)
            {
                float3 operation = Normal;
                Out = operation;
            }

            v2f vert(appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            // 1. Declare your function so it can be used in later functions like 'frag (v2f i)'

            // 2. Use the fragment function
            half4 frag(v2f i) : SV_Target
            {
                //In-class demonstrations:
                // sample the texture
                half4 col = tex2D(_MainTex, i.uv);
                //Yes, you can have multiple return statements in functions; GPU reads top-down
                return col * _Color;
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;

                //FOR HW (4.0.4) FAKELIGHT IMPLEMENTATION:
                //========================================
                // declare normals.
                float3 n = i.normal;
                // declare the output.
                float3 col2 = 0;
                // pass both values as arguments.
                FakeLight_float(n, col2);

                return float4(col2.rgb, 1);
            }
            ENDHLSL
        }
    }
}