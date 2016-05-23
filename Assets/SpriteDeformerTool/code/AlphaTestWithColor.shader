// Unlit alpha-cutout shader.
// - no lighting
// - no lightmap support

Shader "Unlit/Transparent Cutout Color" {
Properties {
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Color ("Tint", Color) = (1,1,1,1)
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}
SubShader {
	Tags {
	"Queue"="AlphaTest" 
	"IgnoreProjector"="True" 
	"RenderType"="TransparentCutout"
	"PreviewType"="Plane"
	"CanUseSpriteAtlas"="True"
	}
	LOD 100

	Lighting Off
	

	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;
			fixed4 _Color;
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color * _Color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
				clip(col.a - _Cutoff);

				return col;
			}
		ENDCG
	}
}
}

