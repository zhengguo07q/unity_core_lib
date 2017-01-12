// ***************************************************************
//  Copyright(c) Yeto
//  FileName	: YetoMobileParticlesAdditive.shader
//  Creator 	:  
//  Date		: 2016-8-4
//  Comment		: 渲染纹理专用，只输出RGB通道， 然后UITexture使用解包的颜色
//				Simplified Additive Particle shader. Differences from regular Additive Particle one:
//				- no Tint color
//				- no Smooth particle support
//				- no AlphaTest
//				- no ColorMask
// ***************************************************************


Shader "Yeto/Mobile/Particles/Additive"
{
	Properties
	{
		_MainTex("Particle Texture", 2D) = "white" {}
	}

	Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Cull Off Lighting Off ZWrite Off Fog{ Color(0,0,0,0) }

		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader
		{
			Pass
			{
				ColorMask RGB
				Blend SrcAlpha One
				SetTexture[_MainTex]
				{
					combine texture * primary
				}
			}
		}
	}
}