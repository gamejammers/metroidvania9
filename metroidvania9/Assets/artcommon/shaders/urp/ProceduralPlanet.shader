//
// (c) BLACKTRIANGLES 2020
// http://www.blacktriangles.com
//

Shader "blacktriangles/URP/ProceduralPlanet" {

    //
    // properties /////////////////////////////////////////////////////////////
    //
    
    Properties {
        _Seed("Seed", float) = 1
        _PixelSize("PixelSize", Range(1,1000)) = 1

        _LightPower ("Light Power", float) = 1
        _LightEnabled("Light Enabled", Range(0,1)) = 0
        _Saturation("Saturation", Range(0.0,1.0)) = 1.0
        _Intensity("Intensity", Range(0.0,1.0)) = 1.0

        _BaseColor ("Base Color", Color) = (0.0, 0.1, 0.2, 1.0)

        _Layer0Color ("Layer0: Color", Color) = (0.0, 1.0, 0.0, 1.0)
        _Layer0Amp   ("Layer0: Amplitude", float) = 4
        _Layer0Mult  ("Layer0: Multiplier", float) = 0.5
        _Layer0Oct   ("Layer0: Octaves", int) = 5

        _Layer1Color ("Layer1: Color", Color) = (0.0, 1.0, 0.0, 1.0)
        _Layer1Amp   ("Layer1: Amplitude", float) = 4
        _Layer1Mult  ("Layer1: Multiplier", float) = 0.5
        _Layer1Oct   ("Layer1: Octaves", int) = 5

        _PolarColor ("Polar Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _PolarHeight ("Polar Height", float) = 25

        _AtmoOct    ("Atmophere: Octaves", int) = 10
        _AtmoSpeed  ("Atmosphere: Speed", float) = 10
        _AtmoColor0 ("Atmosphere: Color 0", Color) = (0.1, 0.2, 0.0, 1.0)
        _AtmoColor1 ("Atmosphere: Color 1", Color) = (0.7, 0.4, 0.3, 1.0)
        _AtmoSize1  ("Atmosphere: Size 1", float) = 1
        _AtmoColor2 ("Atmosphere: Color 2", Color) = (1.0, 0.4, 0.2, 1.0)
        _AtmoSize2  ("Atmosphere: Size 2", float) = 1
        _AtmoColor3 ("Atmosphere: Color 3", Color) = (1.0, 0.4, 0.2, 1.0)
        _AtmoSize3  ("Atmosphere: Size 3", float) = 1
    }

    //
    // ########################################################################
    //
    
    CGINCLUDE

    //
    // includes ///////////////////////////////////////////////////////////////
    //

    #include "UnityCG.cginc"
    #include "UnityLightingCommon.cginc"
    #include "Lighting.cginc"
    #include "AutoLight.cginc"
    
    #include "includes/btLightUtil.cginc"
    #include "includes/NoiseCommon.cginc"
    #include "includes/Perlin.cginc"
    #include "includes/Simplex.cginc"

    //
    // variables //////////////////////////////////////////////////////////////
    //

    uniform float _Seed;
    uniform float _PixelSize;

    uniform float _LightPower;
    uniform int _LightEnabled;
    uniform float _Saturation;
    uniform float _Intensity;

    uniform float4 _BaseColor;

    uniform float4 _Layer0Color;
    uniform float  _Layer0Amp;
    uniform float  _Layer0Mult;
    uniform int    _Layer0Oct;

    uniform float4 _Layer1Color;
    uniform float  _Layer1Amp;
    uniform float  _Layer1Mult;
    uniform int    _Layer1Oct;

    uniform float4 _PolarColor;
    uniform float _PolarHeight;

    uniform int _AtmoOct;
    uniform float _AtmoSpeed;
    uniform float4 _AtmoColor0;
    uniform float4 _AtmoColor1;
    uniform float _AtmoSize1;
    uniform float4 _AtmoColor2;
    uniform float _AtmoSize2;
    uniform float4 _AtmoColor3;
    uniform float _AtmoSize3;

    //
    // types //////////////////////////////////////////////////////////////////
    //
    
    // app -> vert
    struct appdata
    {
        float4 vertex : POSITION;
        float3 normal: NORMAL;
        float3 uv0 : TEXCOORD0;
    };

    //
    // ------------------------------------------------------------------------
    //
    
    // vert -> frag
    struct v2f
    {
        float4 pos: SV_POSITION;
        float3 uv0: TEXCOORD0;
        float4 worldpos: TEXCOORD1;
        float4 vert: TEXCOORD2;
        float3 normal: NORMAL;
    };

    //
    // utils //////////////////////////////////////////////////////////////////
    //

    float terrain(float3 x, float amplitude, float mult, int octaves) {
        float res = 0;
        for(int i = 0; i < octaves; i++) {
            res += amplitude * perlin(x);
            x = x*2+float3(100, 100, 100);
            amplitude *= mult;
        }
        return res;
    }

    //
    // ------------------------------------------------------------------------
    //
    
    float atmosphere(float3 x) {
        float v = 0.0;
	    float a = 1.0;
	    float3 shift = float3(100, 100, 100);
	    for (int i = 0; i < _AtmoOct; ++i) {
	    	v += a * perlin(x);
	    	x = x * 2.0 + shift;
	    	a *= 0.5;
	    }
	    return v;
    }

    //
    // vertex program /////////////////////////////////////////////////////////
    //

    v2f vert(appdata v)
    {
        v2f o;

        float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
        o.pos = UnityObjectToClipPos(v.vertex);
        o.worldpos = mul(unity_ObjectToWorld, v.vertex);
        o.vert = v.vertex;
        o.normal = worldNormal;
        o.uv0 = v.uv0;

        return o;
    }

    //
    // fragment program ///////////////////////////////////////////////////////
    //

    float3 frag(v2f i) : COLOR
    {
        float3 pos = i.vert + _Seed;
        if(_PixelSize > 1)
        {
            float scale = 1000 / _PixelSize;
            pos = floor((pos) * scale) / scale;
        }

        float f = saturate(terrain(pos, _Layer0Amp, _Layer0Mult, _Layer0Oct));
        float f2 = f * saturate(terrain(pos*2, _Layer1Amp, _Layer1Mult, _Layer1Oct));
        float3 col = lerp(_BaseColor, lerp(_Layer0Color, _Layer1Color, f2), f);

        col = lerp(col, _PolarColor, pow(abs(i.normal.y), _PolarHeight)*_PolarColor.a);

        float3 atmoPos = pos + float3(_Time.y,_Time.x,_Time.z) * _AtmoSpeed * 0.01;
        float atmo1 = atmosphere(atmoPos*_AtmoSize1);
        float atmo2 = atmosphere(atmoPos*_AtmoSize2 - 100);
        float atmo3 = atmosphere(atmoPos*_AtmoSize3 + 200);

        float3 atmoColor = _AtmoColor0 * _AtmoColor0.a;
        atmoColor = lerp(atmoColor, _AtmoColor1 * _AtmoColor1.a, atmo1);
        atmoColor = lerp(atmoColor, _AtmoColor2 * _AtmoColor2.a, atmo2);
        atmoColor = lerp(atmoColor, _AtmoColor3 * _AtmoColor3.a, atmo3);

        col = lerp(col, atmoColor, atmoColor);

        float3 lightFinal = float3(1,1,1);
        if(_LightEnabled > 0)
        {
            lightFinal = __calculateLight(i.normal, i.worldpos) * _LightPower * 0.1;
        }
        else
        {
            lightFinal *= _LightPower;
        }

        float c = (col.r + col.g + col.b) / 3.0;
        return saturate(lerp(float3(c,c,c), col * lightFinal, _Saturation)) * _Intensity;
    }
    
    ENDCG

    //
    // ########################################################################
    //
    
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            ZWrite On
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG

        }
    }
}
