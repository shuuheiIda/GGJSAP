Shader "Custom/GridTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Progress ("Progress", Range(0, 1)) = 0
        _GridSize ("Grid Size", Vector) = (10, 10, 0, 0)
        _TransitionColor ("Transition Color", Color) = (0, 0, 0, 1)
        _RandomSeed ("Random Seed", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Progress;
            float4 _GridSize;
            float4 _TransitionColor;
            float _RandomSeed;

            // ランダム関数
            float random(float2 st)
            {
                return frac(sin(dot(st.xy + _RandomSeed, float2(12.9898, 78.233))) * 43758.5453123);
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // グリッドセルのインデックスを計算
                float2 gridUV = floor(i.uv * _GridSize.xy);
                
                // 各セルごとにランダムなオフセットを生成
                float randomOffset = random(gridUV) * 0.3; // 0〜0.3のランダム遅延
                
                // プログレス値にランダムオフセットを適用
                float cellProgress = saturate((_Progress - randomOffset) / (1.0 - 0.3));
                
                // トランジションカラーとの補間
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = lerp(texColor, _TransitionColor, cellProgress);
                
                return finalColor;
            }
            ENDCG
        }
    }
}
