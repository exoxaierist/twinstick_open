Shader "Custom/DitherLitCutoutShaded"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Brightness ("Brightness",Range(0,1)) = 0
        _StepCount ("StepCount",Range(1,20)) = 10
        _Cutoff ("Cutoff", Range(0,1)) = 0.5
        _Scale ("Scale", Range(0,5)) = 1.4
        _Offset ("Offset", Range(0,2)) = 0.05
        _Subtract ("Subtract", Range(0,2)) = 0.97
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout"  "Queue" = "Geometry+0"  }
        LOD 200
        Cull back

        CGPROGRAM
        #pragma surface surf CustomDither alphatest:_Cutoff fullforwardshadows
        #pragma target 3.0

        struct CustomSurfaceOutput
        {
            fixed3 Albedo;
            fixed3 Normal;
            half3 Emission;
            fixed Alpha;

            float3 worldPos;      
        };

        static const float dither4x4[16] = {
			 0, 8, 2,10,
            12, 4,14, 6,
             3,11, 1, 9,
            15, 7,13, 5
        };

        

        sampler2D _MainTex;
        float4 _Color;
        fixed _StepCount;
        fixed _Scale;
        fixed _Offset;
        fixed _Subtract;
        fixed _Brightness;

       half4 LightingCustomDither(CustomSurfaceOutput s,float3 lightDir,float atten){
            fixed4 c;
            fixed stepBrightness;
            fixed newBrightness;
            fixed stepSize = 1/_StepCount;
            fixed3 worldGrid = s.worldPos;
            fixed dither;

            c.r = atten*_LightColor0.a*(dot(s.Normal,lightDir)*(1-_Brightness)+_Brightness);
            c.a = s.Alpha;

            stepBrightness = c.r;
            newBrightness = c.r;
            stepBrightness = floor(c.r*_StepCount)*stepSize;

            worldGrid = (((ceil(worldGrid*16)*0.0625) - (floor(worldGrid*4)*0.25)))*16;
            dither = dither4x4[((worldGrid.x+((worldGrid.z*4)-4)-1)+worldGrid.y*4)%16]*0.0625+0.0625;

            newBrightness = lerp(stepBrightness,stepBrightness+stepSize,ceil(saturate((c.r-stepBrightness)-(dither*stepSize))));
            newBrightness = lerp(c.r,newBrightness,stepBrightness*_Scale+_Offset);

            c.rgb = ((UNITY_LIGHTMODEL_AMBIENT*(1-newBrightness-_Subtract))+_LightColor0.rgb*newBrightness)*s.Albedo;
            return c;
        }

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        void surf (Input IN, inout CustomSurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb*_Color;
            o.Alpha = c.a*_Color.a;
            o.worldPos = IN.worldPos;
        }
        ENDCG
        Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

			sampler2D _MainTex;
            fixed _Cutoff;

            struct v2f{
                V2F_SHADOW_CASTER;
                float2 uv : TEXCOORD1;
            };

            v2f vert(appdata_full v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv=v.texcoord;
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }

            float4 frag(v2f IN):COLOR
            {
                fixed4 c = tex2D(_MainTex,IN.uv);
                clip(c.a-_Cutoff);
                SHADOW_CASTER_FRAGMENT(IN);
            }

			ENDCG
		}
    }
    FallBack "Diffuse"
}