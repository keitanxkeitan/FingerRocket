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

            #define T_TO_B 0
            #define T_TO_L 1
            #define T_TO_R 2
            #define B_TO_T 3
            #define B_TO_L 4
            #define B_TO_R 5
            #define L_TO_T 6
            #define L_TO_B 7
            #define L_TO_R 8
            #define R_TO_T 9
            #define R_TO_B 10
            #define R_TO_L 11

            int _PartType;
            fixed4 _CourseColor;
            fixed4 _BackgroundColor;
            float _CourseWidth;

            bool checkCourseCurve(v2f IN, float2 center, float min, float max)
            {
            	float squaredDistance = (IN.texcoord.x - center.x) * (IN.texcoord.x - center.x) + (IN.texcoord.y - center.y) * (IN.texcoord.y - center.y);
            	return (squaredDistance >= (min * min)) && (squaredDistance <= (max * max));
            }

            bool checkCourse(v2f IN, int partType, float courseWidth)
            {
            	float min = 0.5 * (1.0 - courseWidth);
            	float max = 0.5 * (1.0 + courseWidth);

            	if((partType == T_TO_B) || (partType == B_TO_T))
            	{
            		return (IN.texcoord.x >= min) && (IN.texcoord.x <= max);
            	}
            	else if((partType == L_TO_R) || (partType == R_TO_L))
            	{
            		return (IN.texcoord.y >= min) && (IN.texcoord.y <= max);
            	}
            	else if((partType == T_TO_L) || (partType == L_TO_T))
            	{
            		return checkCourseCurve(IN, float2(0.0, 1.0), min, max);
            	}
            	else if((partType == T_TO_R) || (partType == R_TO_T))
            	{
            		return checkCourseCurve(IN, float2(1.0, 1.0), min, max);
            	}
            	else if((partType == B_TO_L) || (partType == L_TO_B))
            	{
            		return checkCourseCurve(IN, float2(0.0, 0.0), min, max);
            	}
            	else if((partType == B_TO_R) || (partType == R_TO_B))
            	{
            		return checkCourseCurve(IN, float2(1.0, 0.0), min, max);
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