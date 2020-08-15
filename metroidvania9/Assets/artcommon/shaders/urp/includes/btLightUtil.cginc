//=============================================================================
//
// (C) BLACKTRIANGLES 2016
// http://www.blacktriangles.com
//
// Howard N Smith | hsmith | howard@blacktriangles.com
//
//=============================================================================

#ifndef BTLIGHTUTIL_CGINC
#define BTLIGHTUTIL_CGINC

//
// light //////////////////////////////////////////////////////////////////////
//

float3 __calculateLight(float3 normal, float3 worldpos) {
    float3 normalDirection = normal;
    float3 viewDirection = normalize( _WorldSpaceCameraPos.xyz - worldpos.xyz );
    float3 lightDirection;
    float atten;
    
    if(_WorldSpaceLightPos0.w == 0.0){ //directional light
        atten = 1.0;
        lightDirection = normalize(_WorldSpaceLightPos0.xyz);
    }
    else{
        float3 fragmentToLightSource = _WorldSpaceLightPos0.xyz - worldpos.xyz;
        float distance = length(fragmentToLightSource);
        atten = 1.0/distance;
        lightDirection = normalize(fragmentToLightSource);
    }
    
    float3 diffuseReflection = atten * _LightColor0.xyz * saturate(dot(normalDirection, lightDirection));
    float3 specularReflection = float3(0,0,0);// diffuseReflection * _SpecColor.xyz * saturate(dot(reflect(-lightDirection, normalDirection), viewDirection));
    float3 lightFinal = UNITY_LIGHTMODEL_AMBIENT.xyz + diffuseReflection + specularReflection;
    
    return lightFinal;
}

#endif
