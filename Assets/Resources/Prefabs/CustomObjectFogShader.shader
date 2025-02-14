Shader "Custom/ObjectFogShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FogMask ("Fog Mask (R=Coverage)", 2D) = "white" {}
        // _FogOrigin should be set to the world-space bottom-left of your fog overlay.
        _FogOrigin ("Fog Origin (World Pos)", Vector) = (0,0,0,0)
        // _FogScale should equal the width (or height) of your fog overlay in world units.
        _FogScale ("Fog Scale (World Units)", Float) = 100.0
    }
    SubShader
    {
        // Render in the Transparent queue so that alpha blending is applied.
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        // Standard alpha blending.
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma target 2.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;

            sampler2D _FogMask;
            float4 _FogOrigin; // Only the XY components are used.
            float _FogScale;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos      : SV_POSITION;
                float2 uv       : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                // Compute the world position.
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos.xy;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the object's main texture.
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Compute fog mask UV coordinates from the object's world position.
                // Mapping world coordinates to 0..1 UV space based on _FogOrigin and _FogScale.
                float2 fogUV = (i.worldPos - _FogOrigin.xy) / _FogScale;
                // Do not flip Y here if _FogOrigin is already the bottom-left.
                
                // Sample the fog mask's red channel.
                fixed fogCoverage = tex2D(_FogMask, fogUV).r;
                
                // Use the step function:
                // If fogCoverage is less than 0.1 (i.e. visible), step returns 1 (opaque);
                // Otherwise (explored or unexplored), step returns 0 (transparent).
                col.a *= step(fogCoverage, 0.1);
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
