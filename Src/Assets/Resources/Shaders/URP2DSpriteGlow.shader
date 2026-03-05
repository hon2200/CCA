Shader "Custom/URP2DSpriteGlow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        // 发光参数
        _GlowColor ("Glow Color", Color) = (1,1,1,1)
        _GlowPower ("Glow Strength", Range(0, 10)) = 2
        _GlowRange ("Glow Range", Range(0, 0.1)) = 0.02
        _GlowThreshold ("Glow Threshold", Range(0, 1)) = 0.1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
            "RenderPipeline" = "UniversalPipeline" // 声明为URP管线
        }

        Pass
        {
            Tags { "LightMode" = "Universal2D" } // 适配2D渲染

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _GlowPower;
            float _GlowRange;
            float _GlowThreshold;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 采样原图并应用颜色
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                
                // 剔除完全透明的像素，保证主体可见
                clip(col.a - 0.001);

                // 计算发光：采样周围像素的透明度
                float alphaSum = 0;
                alphaSum += tex2D(_MainTex, i.texcoord + float2(-_GlowRange, 0)).a;
                alphaSum += tex2D(_MainTex, i.texcoord + float2(_GlowRange, 0)).a;
                alphaSum += tex2D(_MainTex, i.texcoord + float2(0, -_GlowRange)).a;
                alphaSum += tex2D(_MainTex, i.texcoord + float2(0, _GlowRange)).a;
                alphaSum /= 4;
                
                // 只在边缘阈值以上生成发光
                float glow = smoothstep(_GlowThreshold, 1, alphaSum) * _GlowPower;
                
                // 发光叠加到原图
                col.rgb += _GlowColor.rgb * glow * col.a;

                return col;
            }
            ENDCG
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}