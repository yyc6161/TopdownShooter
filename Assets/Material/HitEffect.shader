Shader "Sprites/HitEffect"
{
    Properties
    {
        [PerRendererData] _MainTex ("精灵纹理", 2D) = "white" {}
        _color("混合颜色", Color) = (1,1,1,1)
		_glow("光晕强度", Range(1,10)) = 3
		[Space]
		_blend("效果混合度", Range(0,1)) = 0
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
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
            };
            
            half4 _color;
			half _glow, _blend;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            half4 frag (v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * i.color;

                col.rgb = lerp(col.rgb, _color.rgb * _glow, _blend);
                col.rgb *= col.a;
                
                return col;
            }
            ENDCG
        }
    }
}
