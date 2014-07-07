Shader "Tim/Unlit_transparent_nocull_rim" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
	_RimPower ("Rim Power", Range(0.5,10)) = 3.0
	_Cutoff ("Cutoff", Range(0,1)) = 0.5
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200



CGPROGRAM
#pragma surface surf Lambert alphatest:_Cutoff

	sampler2D _MainTex; 
	fixed4 _Color;
	float4 _RimColor;
	float _RimPower;

struct Input {
	float2 uv_MainTex;
	float3 viewDir;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal)); 
	o.Emission = c.rgb + _RimColor.rgb * pow (rim, _RimPower);
	o.Alpha = c.a;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
