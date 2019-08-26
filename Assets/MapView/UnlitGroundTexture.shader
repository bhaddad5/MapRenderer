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
			float4 _LookupTex_TexelSize;
			fixed4 _LookupColor;
			sampler2D _GroundTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _LookupTex);
                return o;
            }

			half GetAlphaComponent(fixed2 baseUv, fixed2 texelOffset)
			{
				fixed2 lookupUv = baseUv + (texelOffset * fixed2(_LookupTex_TexelSize.x, _LookupTex_TexelSize.y));
				fixed4 c = tex2D(_LookupTex, lookupUv);
				return (half)all(c == _LookupColor);
			}

            fixed4 frag (v2f i) : SV_Target
            {
				half match = (GetAlphaComponent(i.uv, fixed2(0, 0)) + 
							GetAlphaComponent(i.uv, fixed2(1, 0)) +
							GetAlphaComponent(i.uv, fixed2(0, 1)) +
							GetAlphaComponent(i.uv, fixed2(1, 1)) +
							GetAlphaComponent(i.uv, fixed2(-1, -1)) +
							GetAlphaComponent(i.uv, fixed2(1, -1)) +
							GetAlphaComponent(i.uv, fixed2(-1, 1)) +
							GetAlphaComponent(i.uv, fixed2(-1, 0)) +
							GetAlphaComponent(i.uv, fixed2(0, -1)) +

					 		GetAlphaComponent(i.uv, fixed2(2, -2)) +
							GetAlphaComponent(i.uv, fixed2(2, -1)) +
							GetAlphaComponent(i.uv, fixed2(2, 0)) +
							GetAlphaComponent(i.uv, fixed2(2, 1)) +
							GetAlphaComponent(i.uv, fixed2(2, 2)) +
							GetAlphaComponent(i.uv, fixed2(-2, -2)) +
							GetAlphaComponent(i.uv, fixed2(-2, -1)) +
							GetAlphaComponent(i.uv, fixed2(-2, 0)) +
							GetAlphaComponent(i.uv, fixed2(-2, 1)) +
							GetAlphaComponent(i.uv, fixed2(-2, 2)) +
							GetAlphaComponent(i.uv, fixed2(1, 2)) +
							GetAlphaComponent(i.uv, fixed2(0, 2)) +
							GetAlphaComponent(i.uv, fixed2(-1, 2)) +
							GetAlphaComponent(i.uv, fixed2(1, -2)) +
							GetAlphaComponent(i.uv, fixed2(0, -2)) +
							GetAlphaComponent(i.uv, fixed2(-1, -2))
					) / 25;
				fixed4 c = tex2D(_GroundTex, i.uv * 20);
				c.a = match;

				return c;
            }
			
            ENDCG
        }
    }
	Fallback "VertexLit"
}
