﻿Shader "Custom/WeightVisualizer" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_WeightAmount("Weight amount", Range(0,1)) = 1.0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Standard vertex:vert fullforwardshadows
			#pragma target 3.0
			struct Input {
				float2 uv_MainTex;
				float4 vertexColor; // Vertex color stored here by vert() method
			};

			struct v2f {
			  float4 pos : SV_POSITION;
			  fixed4 color : COLOR;
			};

			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input,o);
				o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
			}

			sampler2D _MainTex;

			half _WeightAmount;
			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				fixed4 c = ((1 - _WeightAmount) * (tex2D(_MainTex, IN.uv_MainTex) * _Color)) + _WeightAmount * IN.vertexColor;
				o.Albedo = c.rgb * IN.vertexColor; // Combine normal color with the vertex color
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}