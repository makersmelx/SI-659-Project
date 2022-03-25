Shader "494/underwater_shaking_effect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Waviness ("Waviness", float) = 100
        _Amplitude("Amplitude", float) = 0.2
        _Frequency("Frequency", float) = 100

        _Color("Color", float) = 0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _Waviness;
            float _Amplitude;
            float _Frequency;
            float _Color;



            fixed4 frag (v2f i) : SV_Target
            {
                float2 custom_uv = i.uv + float2(_Amplitude * sin(_Time.x* _Frequency + i.uv.y* _Waviness), 0);
                fixed4 scene_color = tex2D(_MainTex, custom_uv);

                fixed4 deep_blue_underwater_color = fixed4 (0, 0.3 , 0.447, 1);
                fixed4 final_color = lerp(scene_color, deep_blue_underwater_color,1/(1+exp(-(1 - (0.1 - _Color)*15 * i.uv.y)))*0.7);

                return final_color;
            }
            ENDCG
        }
    }
}
