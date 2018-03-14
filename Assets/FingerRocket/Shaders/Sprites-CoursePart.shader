// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/CoursePart"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
        _PartType ("PartType", Int) = 0
        _CourseColor ("CourseColor", Color) = (1,1,1,1)
        _BackgroundColor ("BackgroundColor", Color) = (0,0,0,1)
        _CourseWidth ("CourseWidth", Float) = 0.5
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
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment coursePartFlag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"

            #define T2B 0
            #define T2L 1
            #define T2R 2
            #define B2T 3
            #define B2L 4
            #define B2R 5
            #define L2T 6
            #define L2B 7
            #define L2R 8
            #define R2T 9
            #define R2B 10
            #define R2L 11
            #define T2B_Sin 12
            #define B2T_Sin 13
            #define L2R_Sin 14
            #define R2L_Sin 15

            int _PartType;
            fixed4 _CourseColor;
            fixed4 _BackgroundColor;
            float _CourseWidth;

            bool checkCourseCurve(v2f IN, float2 center, float min, float max)
            {
            	float squaredDistance = (IN.texcoord.x - center.x) * (IN.texcoord.x - center.x) + (IN.texcoord.y - center.y) * (IN.texcoord.y - center.y);
            	return (squaredDistance >= (min * min)) && (squaredDistance <= (max * max));
            }

            bool checkCourseSin(float x, float y, float courseWidth)
            {
            	float min = sin(x) * 0.25 - courseWidth;
            	float max = sin(x) * 0.25 + courseWidth;
            	return (y >= min) && (y <= max);
            }

            bool checkCourse(v2f IN, int partType, float courseWidth)
            {
            	float min = 0.5 * (1.0 - courseWidth);
            	float max = 0.5 * (1.0 + courseWidth);

            	if((partType == T2B) || (partType == B2T))
            	{
            		return (IN.texcoord.x >= min) && (IN.texcoord.x <= max);
            	}
            	else if((partType == L2R) || (partType == R2L))
            	{
            		return (IN.texcoord.y >= min) && (IN.texcoord.y <= max);
            	}
            	else if((partType == T2L) || (partType == L2T))
            	{
            		return checkCourseCurve(IN, float2(0.0, 1.0), min, max);
            	}
            	else if((partType == T2R) || (partType == R2T))
            	{
            		return checkCourseCurve(IN, float2(1.0, 1.0), min, max);
            	}
            	else if((partType == B2L) || (partType == L2B))
            	{
            		return checkCourseCurve(IN, float2(0.0, 0.0), min, max);
            	}
            	else if((partType == B2R) || (partType == R2B))
            	{
            		return checkCourseCurve(IN, float2(1.0, 0.0), min, max);
            	}
            	else if((partType == T2B_Sin) || (partType == B2T_Sin))
            	{
            		float x = IN.texcoord.y * 3.14159 * 2.0;
            		float y = (0.5 - IN.texcoord.x) / 0.5;
            		return checkCourseSin(x, y, courseWidth);
            	}
            	else if((partType == L2R_Sin) || (partType == R2L_Sin))
            	{
            		float x = IN.texcoord.x * 3.14159 * 2.0;
            		float y = (IN.texcoord.y - 0.5) / 0.5;
            		return checkCourseSin(x, y, courseWidth);
            	}
            	return false;
            }

            fixed4 coursePartFlag(v2f IN) : SV_Target
            {
            	return checkCourse(IN, _PartType, _CourseWidth) ? _CourseColor : _BackgroundColor;
            }
        ENDCG
        }
    }
}
