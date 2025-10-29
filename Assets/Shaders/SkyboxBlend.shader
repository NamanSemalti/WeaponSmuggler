Shader "Skybox/Blend"
{
    Properties
    {
        _Skybox1 ("Skybox 1 (Day)", Cube) = "" {}
        _Skybox2 ("Skybox 2 (Night)", Cube) = "" {}
        _Blend ("Blend", Range(0,1)) = 0.0
        _Exposure ("Exposure", Range(0,8)) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Background" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            samplerCUBE _Skybox1;
            samplerCUBE _Skybox2;
            float _Blend;
            float _Exposure;

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 vertex : SV_POSITION; float3 texcoord : TEXCOORD0; };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c1 = texCUBE(_Skybox1, normalize(i.texcoord));
                fixed4 c2 = texCUBE(_Skybox2, normalize(i.texcoord));
                fixed4 color = lerp(c1, c2, _Blend);
                color.rgb *= _Exposure;
                return color;
            }
            ENDCG
        }
    }
}
