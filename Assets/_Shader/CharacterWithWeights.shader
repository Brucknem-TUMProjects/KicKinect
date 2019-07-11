Shader "Custom/CharacterWithWeights" {

	Properties{
		_diffuseTexture("Diffuse Texture", 2D) = "white" {}
		_normalTexture("Normal Texture", 2D) = "bump" {}
		_normalIntensity("Normal Intensity", Range(0,10)) = 1
		_brightness("Brightness", Range(0,10)) = 1
		_WeightAmount("Weight amount", Range(0,1)) = 1.0
	}

		SubShader{

			CGPROGRAM
				#pragma surface surf Lambert vertex:vert
			
				half _WeightAmount;
				sampler2D _diffuseTexture;
				sampler2D _normalTexture;
				half _normalIntensity;
				half _brightness;

				struct Input {
					float2 uv_diffuseTexture;
					float2 uv_normalTexture;
					float4 vertexColor; // Vertex color stored here by vert() method
				};

				struct v2f {
					float4 pos : SV_POSITION;
					fixed4 color : COLOR;
				};

				void vert(inout appdata_full v, out Input o)
				{
					UNITY_INITIALIZE_OUTPUT(Input, o);
					o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
				}

				void surf(Input IN, inout SurfaceOutput o) {
					o.Albedo = ((1 - _WeightAmount) * tex2D(_diffuseTexture, IN.uv_diffuseTexture).rgb) + _WeightAmount * IN.vertexColor;
					o.Normal = UnpackNormal(tex2D(_normalTexture, IN.uv_normalTexture)) * _brightness;
					o.Normal *= float3(_normalIntensity,_normalIntensity,1);
				}

			ENDCG
		}
			Fallback "Diffuse"
}