Shader "Unlit/WorldGrid"
{
	Properties
	{
		_GridColor ("Grid Color", Color) = (0.5, 0.5, 1.0, 1.0)
		_GridScale ("Grid Scale", Range(0.1, 20)) = 1
		_GridPower ("Grid Power", Range(2, 100)) = 30
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry-1"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			float _GridScale;
			float _GridPower;
			half4 _GridColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 bf = normalize(abs(i.normal));
				fixed3 rem = frac(i.worldPos);
				rem = abs(cos(rem * _GridScale * 3.14159));
				rem = pow(rem, _GridPower);
				fixed2 result = (bf.x > bf.y) * rem.yz + (bf.x <= bf.y) * rem.xz;
				fixed maxaxis = max(bf.x, bf.y);
				result = (maxaxis > bf.z) * result + (maxaxis <= bf.z) * rem.xy;
				fixed sum = (result.x + result.y) / 2;
				fixed4 col = _GridColor * sum;

				return col;
			}
			ENDCG
		}
	}
}
