Shader "Custom/FogOfWarAlphaShader"
{
    Properties
    {
        _FogMask ("Fog Mask (R=Coverage)", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0,0,0,1) // black overlay
    }
    SubShader
    {
        // Render in Transparent queue so alpha actually blends
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // Standard alpha blending
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
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _FogMask;
            float4 _FogColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // We store coverage in the red channel of the FogMask
                // coverage = 0 => fully transparent
                // coverage = 1 => fully opaque
                fixed coverage = tex2D(_FogMask, i.uv).r;

                // Output a color that is _FogColor, with alpha = coverage
                // e.g. if coverage=1 => black & alpha=1
                //     if coverage=0 => black & alpha=0 (fully see-thru)
                fixed4 finalColor = _FogColor;
                finalColor.a = coverage;

                return finalColor;
            }
            ENDCG
        }
    }
}
