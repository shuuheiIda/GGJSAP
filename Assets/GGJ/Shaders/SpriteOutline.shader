Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        [Header(Outline Settings)]
        _OutlineColor ("Outline Color", Color) = (0,1,0,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1.5
        _OutlineIntensity ("Outline Intensity", Range(0, 5)) = 1.0
        
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };
            
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _OutlineIntensity;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                
                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif
                
                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                // メインテクスチャをサンプリング
                fixed4 mainColor = tex2D(_MainTex, i.texcoord);
                
                // アウトライン検出用に周囲8方向をサンプリング
                float outline = 0;
                float offsetX = _MainTex_TexelSize.x * _OutlineWidth;
                float offsetY = _MainTex_TexelSize.y * _OutlineWidth;
                
                // 8方向チェック
                outline += tex2D(_MainTex, i.texcoord + float2(offsetX, 0)).a;
                outline += tex2D(_MainTex, i.texcoord + float2(-offsetX, 0)).a;
                outline += tex2D(_MainTex, i.texcoord + float2(0, offsetY)).a;
                outline += tex2D(_MainTex, i.texcoord + float2(0, -offsetY)).a;
                outline += tex2D(_MainTex, i.texcoord + float2(offsetX, offsetY)).a;
                outline += tex2D(_MainTex, i.texcoord + float2(-offsetX, offsetY)).a;
                outline += tex2D(_MainTex, i.texcoord + float2(offsetX, -offsetY)).a;
                outline += tex2D(_MainTex, i.texcoord + float2(-offsetX, -offsetY)).a;
                
                // アウトラインの存在をチェック（周囲にピクセルがある場合）
                outline = step(0.1, outline);
                
                // メインカラーの適用
                fixed4 finalColor = mainColor * i.color;
                
                // アウトライン部分（スプライトの外側）の処理
                if (mainColor.a < 0.1 && outline > 0)
                {
                    finalColor = _OutlineColor * _OutlineIntensity;
                    finalColor.a = _OutlineColor.a;
                }
                
                return finalColor;
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}
