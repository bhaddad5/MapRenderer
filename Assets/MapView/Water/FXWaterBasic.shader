// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "FX/Water (Basic)" {
Properties {
	_MaskTex("Mask", 2D) = "white" {}
	_horizonColor ("Horizon color", COLOR)  = ( .172 , .463 , .435 , 0)
	_WaveScale ("Wave scale", Range (0.02,0.15)) = .07
	[NoScaleOffset] _ColorControl ("Reflective color (RGB) fresnel (A) ", 2D) = "" { }
	[NoScaleOffset] _BumpMap ("Waves Normalmap ", 2D) = "" { }
	WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
	}

CGINCLUDE

#include "UnityCG.cginc"

uniform float4 _horizonColor;

sampler2D _MaskTex;
float4 _MaskTex_TexelSize;

uniform float4 WaveSpeed;
uniform float _WaveScale;
uniform float4 _WaveOffset;

struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	float4 texcoord : TEXCOORD0;
};

struct v2f {
	float4 pos : SV_POSITION;
	float2 bumpuv[2] : TEXCOORD0;
	float4 uv : TEXCOORD3;
	float3 viewDir : TEXCOORD2;
	UNITY_FOG_COORDS(3)
};

v2f vert(appdata v)
{
	v2f o;
	float4 s;

	o.pos = UnityObjectToClipPos(v.vertex);

	// scroll bump waves
	float4 temp;
	float4 wpos = mul (unity_ObjectToWorld, v.vertex);
	temp.xyzw = wpos.xzxz * _WaveScale + _WaveOffset;
	o.bumpuv[0] = temp.xy * float2(.4, .45);
	o.bumpuv[1] = temp.wz;

	o.uv = float4(v.texcoord.xy, 0, 0);

	// object space view direction
	o.viewDir.xzy = normalize( WorldSpaceViewDir(v.vertex) );

	UNITY_TRANSFER_FOG(o,o.pos);
	return o;
}

ENDCG


Subshader {
	Tags{ "Queue" = "Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	ColorMask RGB
	Pass {

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fog

sampler2D _BumpMap;
sampler2D _ColorControl;

half GetAlphaComponent(fixed2 baseUv, fixed2 texelOffset)
{
	fixed2 lookupUv = baseUv + (texelOffset * fixed2(_MaskTex_TexelSize.x, _MaskTex_TexelSize.y));
	fixed4 c = tex2D(_MaskTex, lookupUv);
	return (half)all(c == fixed4(0, 0, 1, 1));
}

half4 frag( v2f i ) : COLOR
{
	half3 bump1 = UnpackNormal(tex2D( _BumpMap, i.bumpuv[0] )).rgb;
	half3 bump2 = UnpackNormal(tex2D( _BumpMap, i.bumpuv[1] )).rgb;
	half3 bump = (bump1 + bump2) * 0.5;
	
	half fresnel = dot( i.viewDir, bump );
	half4 water = tex2D( _ColorControl, float2(fresnel,fresnel) );
	
	half4 col;
	col.rgb = lerp( water.rgb, _horizonColor.rgb, water.a );
	col.a = _horizonColor.a;

	UNITY_APPLY_FOG(i.fogCoord, col);

	half match = (GetAlphaComponent(i.uv, fixed2(0, 0))) / 2 +
		(GetAlphaComponent(i.uv, fixed2(1, 0)) +
		GetAlphaComponent(i.uv, fixed2(0, 1)) +
		GetAlphaComponent(i.uv, fixed2(1, 1)) +
		GetAlphaComponent(i.uv, fixed2(-1, -1)) +
		GetAlphaComponent(i.uv, fixed2(1, -1)) +
		GetAlphaComponent(i.uv, fixed2(-1, 1)) +
		GetAlphaComponent(i.uv, fixed2(-1, 0)) +
		GetAlphaComponent(i.uv, fixed2(0, -1))
		) / 16;

	col.a = match;
	return col;
}
ENDCG
	}
}

}
