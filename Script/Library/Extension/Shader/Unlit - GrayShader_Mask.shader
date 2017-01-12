Shader "Unlit/GrayShader_Mask" {
	Properties
	{
		_MainTex ("Base (RGB), Alpha (A)", 2D) = "black" {}
		_Mask ("Mask", 2D) = "white" {}
	}
	
	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent+1"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Offset -1, -1
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
				
			#include "UnityCG.cginc"
	
			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};
	
			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				fixed gray : TEXCOORD1; 
			};
	
			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _Mask;
			float4 _Mask_ST;
				
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = v.texcoord;
				o.color = v.color;
				o.gray = dot(v.color, fixed4(1,1,1,0));
				return o;
			}
				
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col; 
				fixed4 alphaCol;
				if (i.gray == 0) 
				{ 
					col = tex2D(_MainTex, i.texcoord); 
					col.rgb = dot(col.rgb, fixed3(.222,.707,.071)); 
					alphaCol =  tex2D(_Mask, i.texcoord); 
					col.a = alphaCol.r;
					col = col*i.color.a;				    
				} 
				else 
				{ 
					col = tex2D(_MainTex, i.texcoord); 
					alphaCol =  tex2D(_Mask, i.texcoord); 
					col.a = alphaCol.r;
					col = col * i.color;
				} 
				return col; 
			}
			ENDCG
		}
	}

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
				Combine Texture * Primary
			}
		}
	}
		Fallback "Mobile/Diffuse"
}
