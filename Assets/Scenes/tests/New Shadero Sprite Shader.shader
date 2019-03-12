//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.4                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/New Shadero Sprite Shader"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_SourceNewTex_1("_SourceNewTex_1(RGB)", 2D) = "white" { }
_HolographicParalax_Size_1("_HolographicParalax_Size_1", Range(1, 0)) = 0.15
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_OperationBlend_Fade_1("_OperationBlend_Fade_1", Range(0, 1)) = 1
_NewTex_2("NewTex_2(RGB)", 2D) = "white" { }
_MaskRGBA_Fade_1("_MaskRGBA_Fade_1", Range(0, 1)) = 0
_Generate_Starfield_PosX_1("_Generate_Starfield_PosX_1", Range(-1, 2)) = 1.052
_Generate_Starfield_PosY_1("_Generate_Starfield_PosY_1", Range(-1, 2)) = 0.5
_Generate_Starfield_Number_1("_Generate_Starfield_Number_1", Range(0, 20)) = 5
_Generate_Starfield_Size_1("_Generate_Starfield_Size_1", Range(0.01, 16)) = 2
_Generate_Starfield_Speed_1("_Generate_Starfield_Speed_1", Range(-100, 100)) = 5
_OperationBlend_Fade_2("_OperationBlend_Fade_2", Range(0, 1)) = 1
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" "DisableBatching" = "True"}
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off 

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float4 color    : COLOR;
};

sampler2D _MainTex;
float _SpriteFade;
sampler2D _SourceNewTex_1;
float _HolographicParalax_Size_1;
sampler2D _NewTex_1;
float _OperationBlend_Fade_1;
sampler2D _NewTex_2;
float _MaskRGBA_Fade_1;
float _Generate_Starfield_PosX_1;
float _Generate_Starfield_PosY_1;
float _Generate_Starfield_Number_1;
float _Generate_Starfield_Size_1;
float _Generate_Starfield_Speed_1;
float _OperationBlend_Fade_2;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 OperationBlend(float4 origin, float4 overlay, float blend)
{
float4 o = origin; 
o.a = overlay.a + origin.a * (1 - overlay.a);
o.rgb = (overlay.rgb * overlay.a + origin.rgb * origin.a * (1 - overlay.a)) * (o.a+0.0000001);
o.a = saturate(o.a);
o = lerp(origin, o, blend);
return o;
}
float4 Generate_Starfield(float2 uv, float posx, float posy, float number, float size, float speed, float black)
{
float2 position = uv-float2(posx,posy);
float spx = speed * _Time;
float angle = atan2(position.y, position.x) / (size*3.14159265359);
angle -= floor(angle);
float rad = length(position);
float color = 0.0;
float a = angle * 360;
float aF = frac(a);
float aR = floor(a) + 1.;
float aR1 = frac(aR*frac(aR*.7331)*54.3);
float aR2 = frac(aR*frac(aR*.82345)*12.345);
float t = spx + aR1*100.;
float radDist = sqrt(aR2 + float(0));
float adist = radDist / rad;
float dist = (t + adist);
dist = abs(frac(dist) - 0.15);
color += max(0., .5 - dist*100. / adist)*(.5 - abs(aF - .5))*number / adist / radDist;
angle = frac(angle);
float4 result = color;
result.a = saturate(color + black);
return result;
}
float4 HolographicParalax(float2 uv, sampler2D source, float value)
{
float rtx = -unity_ObjectToWorld[0][2];
rtx = rtx * 0.1;
float4 rgb = tex2D(source, uv + float2(rtx, 0));
float r = tex2D(source, uv + float2(rtx * (1-value), 0)).r;
float b = tex2D(source, uv + float2(rtx * (1+value), 0)).b;
return float4(r, rgb.g, b,rgb.a);
}
float4 frag (v2f i) : COLOR
{
float4 _HolographicParalax_1 = HolographicParalax(i.texcoord,_SourceNewTex_1,_HolographicParalax_Size_1);
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 OperationBlend_1 = OperationBlend(_HolographicParalax_1, NewTex_1, _OperationBlend_Fade_1); 
float4 NewTex_2 = tex2D(_NewTex_2, i.texcoord);
float4 MaskRGBA_1=OperationBlend_1;
MaskRGBA_1.a = lerp(NewTex_2.r * OperationBlend_1.a, (1 - NewTex_2.r) * OperationBlend_1.a,_MaskRGBA_Fade_1);
float4 _Generate_Starfield_1 = Generate_Starfield(i.texcoord,_Generate_Starfield_PosX_1,_Generate_Starfield_PosY_1,_Generate_Starfield_Number_1,_Generate_Starfield_Size_1,_Generate_Starfield_Speed_1,0);
float4 OperationBlend_2 = OperationBlend(MaskRGBA_1, _Generate_Starfield_1, _OperationBlend_Fade_2); 
float4 FinalResult = OperationBlend_2;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
