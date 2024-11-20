Shader "Shader Graphs/Sprite-Unlit-Rewrite"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AlphaClip ("Alpha Clip", Range(0.0, 1.0)) = 0.5
        [PerRendererData] _Solidity ("Solidity", Range(0.0, 1.0)) = 0
        [PerRendererData] _Grayscale ("Grayscale", Range(0.0,1.0)) = 0
        _ColorFrom ("Color From", Color) = (0,0,0,1)
        _ColorTo ("Color To", Color) = (0,0,0,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        AlphaTest GEqual [_AlphaClip]

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _ColorFrom;
            fixed4 _ColorTo;
            fixed _AlphaClip;
            fixed _Solidity;
            fixed _Grayscale;
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);

                //color swap
                fixed diff = 1-ceil(dot(abs(col.rgb-_ColorFrom.rgb),fixed3(0.3,0.3,0.3)));
                col.rgb = lerp(col.rgb,_ColorTo.rgb,diff);

                //make solid
                col = lerp(col,fixed4(1,1,1,col.a),_Solidity) * _Color * i.color;

                //make grayscale
                fixed gray = dot(col.rgb, half3(0.299, 0.587, 0.114))*0.2;
                col = lerp(col,fixed4(gray,gray,gray,col.a),_Grayscale);

                if (col.a < _AlphaClip)
                    discard;

                return col;
            }
            ENDCG
        }
    }
}
