Shader "Unlit/UnlitGroundTexture"
{
    Properties
    {
		_LookupTex("Texture", 2D) = "white" {}
		_LookupColor("Lookup Color", Color) = (1,1,1,1)
		_GroundTex("Ground Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _LookupTex;
            float4 _LookupTex_ST;
			fixed4 _LookupColor;
			sampler2D _GroundTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _LookupTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				fixed4 c = tex2D(_LookupTex, i.uv);
				half match = 0;
				if (all(c == _LookupColor))
					match = 1;
				c = tex2D(_GroundTex, i.uv * 5) * match;
				c.a = match;

				return c;
            }
            ENDCG
        }
    }
}
