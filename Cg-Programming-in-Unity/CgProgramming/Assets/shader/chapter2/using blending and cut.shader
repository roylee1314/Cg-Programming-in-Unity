﻿Shader "Cg shader using blending and cutaways" {
	SubShader{
		Tags{ "Queue" = "Transparent" }
		// first pass (is executed before the second pass)
		Pass{
		Cull Front // cull only front faces
		ZWrite Off // don't write to depth buffer
				   // in order not to occlude other objects
		Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		struct vertexInput {
		float4 vertex : POSITION;
	};
	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 posInObjectCoords : TEXCOORD0;
	};
	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;
		output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
		output.posInObjectCoords = input.vertex;
		return output;
	}
	float4 frag(vertexOutput input) : COLOR
	{
		if (input.posInObjectCoords.y > 0.0)
		{
			discard; // drop the fragment if y coordinate > 0
		}
		return float4(1.0, 0.0, 0.0, 0.8);
	}
		ENDCG
	}
		// second pass (is executed after the first pass)
		Pass{
		Cull Back // cull only back faces
		ZWrite Off // don't write to depth buffer
				   // in order not to occlude other objects
		Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
		struct vertexInput {
		float4 vertex : POSITION;
	};
	struct vertexOutput {
		float4 pos : SV_POSITION;
		float4 posInObjectCoords : TEXCOORD0;
	};
	vertexOutput vert(vertexInput input)
	{
		vertexOutput output;
		output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
		output.posInObjectCoords = input.vertex;
		return output;
	}
	float4 frag(vertexOutput input) : COLOR
	{
		//if (input.posInObjectCoords.y > 0.0)
		//{
		//	discard; // drop the fragment if y coordinate > 0
		//}
		return float4(0.0, 1.0, 0.0, 0.3);
	}
		ENDCG
	}
	}
}