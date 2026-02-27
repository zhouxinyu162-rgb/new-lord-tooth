Shader "Custom/Stamp"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Center ("Center", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 0.1
        _Strength ("Strength", Float) = 0.1
    }

    SubShader
    {
        Pass
        {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Center;
            float _Radius;
            float _Strength;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float current = tex2D(_MainTex, i.uv).r;
                float d = distance(i.uv, _Center.xy);

                if (d < _Radius)
                {
                    current = saturate(current + _Strength); // 每次只加一点点
                }

                return float4(current, current, current, 1);
            }
            ENDCG
        }
    }
}