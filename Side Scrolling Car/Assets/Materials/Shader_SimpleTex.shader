Shader "Unlit/Shader_SimpleTex"
{
	Properties{
		_MainTex("Main Texture", 2D) = "white" {}
		//_ScrollSpeed("Scroll Speed", Float) = 0
		_OverlayColor("Overlay Color", Color) = (1, 1, 1, 1)
		_ParallaxMultiplier("Parallax Multipler", Float) = 1
	}
	SubShader{
		Tags{"Queue" = "Geometry" "RenderType" = "Transparent" "IgnoreProjector" = "True"}
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct Input {
				float4 vertex : POSITION;
				fixed2 uv : TEXCOORD0;
				fixed4 color : COLOR;
 			};
			struct Output{
				float4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			sampler2D _MainTex;
			float _ScrollSpeed;
			float4 _OverlayColor;
			float _ParallaxMultiplier;

			//Vertex Shader
			Output vert(Input i) {
				Output o;
				o.vertex = UnityObjectToClipPos(i.vertex);
				o.uv = i.uv;
				o.color = i.color * _OverlayColor;

				return o;
			}

			//Fragment Shader
			fixed4 frag(Output o) : COLOR {
				fixed4 finalColor;
				o.uv.x = o.uv.x + _ScrollSpeed * _Time.x * _ParallaxMultiplier;
				finalColor = tex2D(_MainTex, o.uv);
				return finalColor * o.color;
			}


			ENDCG
		}

	}
}
