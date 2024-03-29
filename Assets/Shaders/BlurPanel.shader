Shader "UI/BlurPanel"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Size("Blur Radius", Range(0, 4)) = 3
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
        GrabPass
        {
            Tags
            {
                "LightMode" = "Always" // but I want dark mode D: /j
            }
        }
        
        // Horizontal gaussian blur pass
        Pass
        {
            Tags
            {
                "LightMode" = "Always"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct vertdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct fragout_t {
                float4 vertex : POSITION;
                float4 uvgrab : TEXCOORD0;
            };

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Size;

            #define BLURX(uv,weight,kernel) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(uv.x + _GrabTexture_TexelSize.x * kernel * _Size, uv.y, uv.z, uv.w))) * weight
            
            fragout_t vert(vertdata_t v)
            {
                fragout_t o;

                o.vertex = UnityObjectToClipPos(v.vertex);

#if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
#else
                float scale = 1.0;
#endif

                o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                o.uvgrab.zw = o.vertex.zw;
                return o;
            }

            half4 frag(fragout_t f) : COLOR
            {
                half4 sum = half4(0,0,0,0);

                sum += BLURX(f.uvgrab, 0.05, -4.0);
                sum += BLURX(f.uvgrab, 0.09, -3.0);
                sum += BLURX(f.uvgrab, 0.12, -2.0);
                sum += BLURX(f.uvgrab, 0.15, -1.0);
                sum += BLURX(f.uvgrab, 0.18, 0.0);
                sum += BLURX(f.uvgrab, 0.15, +1.0);
                sum += BLURX(f.uvgrab, 0.12, +2.0);
                sum += BLURX(f.uvgrab, 0.09, +3.0);
                sum += BLURX(f.uvgrab, 0.05, +4.0);

                return sum;
            }
            ENDCG
        }
        
        GrabPass
        {
            Tags
            {
                "LightMode" = "Always" // but I want dark mode D: /j
            }
        }

        // Vertical gaussian blur pass
        Pass
        {
            Tags
            {
                "LightMode" = "Always"
            }
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma fragmentoption ARB_precision_hint_fastest
            #include "UnityCG.cginc"

            struct vertdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct fragout_t {
                float4 vertex : POSITION;
                float4 uvgrab : TEXCOORD0;
            };

            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float _Size;

            #define BLURY(uv,weight,kernel) tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(float4(uv.x, uv.y + _GrabTexture_TexelSize.y * kernel * _Size, uv.z, uv.w))) * weight
            
            fragout_t vert(vertdata_t v)
            {
                fragout_t o;

                o.vertex = UnityObjectToClipPos(v.vertex);

#if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
#else
                float scale = 1.0;
#endif

                o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
                o.uvgrab.zw = o.vertex.zw;
                return o;
            }

            half4 frag(fragout_t f) : COLOR
            {
                half4 sum = half4(0,0,0,0);

                sum += BLURY(f.uvgrab, 0.05, -4.0);
                sum += BLURY(f.uvgrab, 0.09, -3.0);
                sum += BLURY(f.uvgrab, 0.12, -2.0);
                sum += BLURY(f.uvgrab, 0.15, -1.0);
                sum += BLURY(f.uvgrab, 0.18, 0.0);
                sum += BLURY(f.uvgrab, 0.15, +1.0);
                sum += BLURY(f.uvgrab, 0.12, +2.0);
                sum += BLURY(f.uvgrab, 0.09, +3.0);
                sum += BLURY(f.uvgrab, 0.05, +4.0);

                return sum;
            }
            ENDCG
        }

        // Image blend pass
        Pass
        {
            Name "Default"
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #pragma multi_compile __ UNITY_UI_ALPHACLIP

            struct vertdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct fragdata_t
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float4 uvgrab : TEXCOORD2;
            };

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _GrabTexture_TexelSize;
            float4 _ClipRect;
            float4 _MainTex_ST;
            half4 _TextureSampleAdd;

            fragdata_t vert(vertdata_t v)
            {
                fragdata_t OUT;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

                #if UNITY_UV_STARTS_AT_TOP
                float scale = -1.0;
                #else
                float scale = 1.0;
                #endif

                OUT.uvgrab.xy = (float2(OUT.vertex.x, OUT.vertex.y * scale) + OUT.vertex.w) * 0.5;
                OUT.uvgrab.zw = OUT.vertex.zw;

                OUT.color = v.color;
                return OUT;
            }

            fixed4 frag(fragdata_t IN) : SV_TARGET
            {
                half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
                half4 blur = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.uvgrab));
                return blur * color;
            }
            
            ENDCG
        }
    }
}
