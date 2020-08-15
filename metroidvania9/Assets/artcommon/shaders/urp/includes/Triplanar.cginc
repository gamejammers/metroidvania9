//=============================================================================
//
// (C) BLACKTRIANGLES 2016
// http://www.blacktriangles.com
//
// Howard N Smith | hsmith | howard@blacktriangles.com
//
//=============================================================================

#ifndef TRIPLANAR_CGINC
#define TRIPLANAR_CGINC

// variables ///////////////////////////////////////////////////////////////////
uniform sampler2D _Side, _Top, _Bottom;
uniform float _SideScale, _TopScale, _BottomScale;
uniform float4 _SideColor, _TopColor, _BottomColor;

// sample a triplanar pixel color //////////////////////////////////////////////
float3 triplanarPixelColor( float3 normalDir, float3 worldPos )
{
    // pixel color
    float3 projNormal = saturate(pow(normalDir * 1.4, 4));

    //// sides
    float3 x = tex2D(_Side, frac(worldPos.zy * _SideScale)) * abs(normalDir.x) * _SideColor;

    // top and bottom
    float3 y = 0;
    if (normalDir.y > 0) {
        y = tex2D(_Top, frac(worldPos.zx * _TopScale)) * abs(normalDir.y) * _TopColor;
    } else {
        y = tex2D(_Bottom, frac(worldPos.zx * _BottomScale)) * abs(normalDir.y) * _BottomColor;
    }

    // front and back
    float3 z = tex2D(_Side, frac(worldPos.xy * _SideScale)) * abs(normalDir.z) * + _SideColor;

    float3 pixelColor = z;
    pixelColor = lerp( pixelColor, x, projNormal.x);
    pixelColor = lerp( pixelColor, y, projNormal.y);
    return pixelColor;
}


#endif
