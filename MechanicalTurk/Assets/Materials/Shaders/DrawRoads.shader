Shader "Custom/Roads"
{
	Properties{

		_RoadTex("Road (RGB)", 2D) = "white" {}
	_RoadColor("Road Color", color) = (1,1,1,1)
		_GroundTex("Ground (RGB)", 2D) = "white" {}
	_GroundColor("Ground Color", color) = (1,1,1,1)
		_Splat("Splat", 2D) = "black" {}
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0

		struct appdata
	{
		float4 vertex : POSITION;
		float4 tangent : TANGENT;
		float3 normal : NORMAL;
		float2 texcoord : TEXCOORD0;
	};

	sampler2D _Splat;



	sampler2D _GroundTex;
	fixed4 _GroundColor;

	sampler2D _RoadTex;
	fixed4 _RoadColor;

	struct Input
	{
		float2 uv_GroundTex;
		float2 uv_RoadTex;
		float2 uv_Splat;
	};

	void surf(Input IN, inout SurfaceOutputStandard o)
	{
		half amount = tex2Dlod(_Splat, float4(IN.uv_Splat, 0, 0)).r;
		half4 c = lerp(tex2D(_GroundTex, IN.uv_GroundTex) * _GroundColor, tex2D(_RoadTex, IN.uv_RoadTex) * _RoadColor, amount);
		//half4 c = tex2D(_GroundTex, IN.uv_GroundTex) * _GroundColor;
		o.Albedo = c.rgb;
		o.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}