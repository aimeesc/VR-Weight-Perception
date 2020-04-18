Shader "Custom/Outline" {

    Properties {
        _Color("Outline Color", Color) = (0, 0, 0, 0)
        _Width("Outline Width", float) = 0.1
    }

    SubShader {
        Tags {
            "Queue"="Transparent"
            "LightMode"="ForwardBase"
            "RenderType"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        //1パス目.
        Pass {

            Cull Front

            CGPROGRAM

            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag

            float4 _Color;
            float _Width;

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v) {
                float distance = UnityObjectToViewPos(v.vertex).z;
                v.vertex.xyz += v.normal * -distance * _Width;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return _Color;
            }

            ENDCG
        }

        //2パス目.
        // Pass {

        //     Cull Back

        //     CGPROGRAM

        //     #include "UnityCG.cginc"
        //     #include "UnityLightingCommon.cginc"

        //     #pragma vertex vert
        //     #pragma fragment frag

        //     float4 _Color;

        //     struct v2f
        //     {
        //         float2 uv : TEXCOORD0;
        //         fixed4 diff : COLOR0;
        //         float4 vertex : SV_POSITION;
        //     };

        //     v2f vert (appdata_base v) {
        //         v2f o;
        //         o.vertex = UnityObjectToClipPos(v.vertex);

        //         o.uv = v.texcoord;
        //         half3 worldNormal = UnityObjectToWorldNormal(v.normal);
        //         half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
        //         o.diff = nl * _LightColor0;

        //         // 前のシェーダーとくらべ、唯一の相違点
        //         // 主要ライトの拡散ライティングに加え
        //         // アンビエントやライトプローブの照明を加えます。
        //         // UnityCG.cginc の ShadeSH9 関数がワールド空間法線で
        //         // それを評価します。
        //         o.diff.rgb += ShadeSH9(half4(worldNormal,1));
        //         return o;
        //     }

        //     fixed4 frag (v2f i) : SV_Target {
        //         fixed4 col = _Color;
        //         col *= i.diff;
        //         col.a = 1;
        //         return col;
        //     }

        //     ENDCG
        // }
    }
}
