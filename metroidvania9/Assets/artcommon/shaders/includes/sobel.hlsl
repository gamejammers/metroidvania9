#ifndef BT_SOBEL_INCLUDED
#define BT_SOBEL_INCLUDED

float DepthSample(float2 uv)
{
    return SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv);
}

void Sobel_float(float thickness, float2 uv, out float result)
{
    float hr = 0;
    float vt = 0;
    
    hr += DepthSample(uv + float2(-1.0, -1.0) * thickness) *  1.0;
    hr += DepthSample(uv + float2( 1.0, -1.0) * thickness) * -1.0;
    hr += DepthSample(uv + float2(-1.0,  0.0) * thickness) *  2.0;
    hr += DepthSample(uv + float2( 1.0,  0.0) * thickness) * -2.0;
    hr += DepthSample(uv + float2(-1.0,  1.0) * thickness) *  1.0;
    hr += DepthSample(uv + float2( 1.0,  1.0) * thickness) * -1.0;
    
    vt += DepthSample(uv + float2(-1.0, -1.0) * thickness) *  1.0;
    vt += DepthSample(uv + float2( 0.0, -1.0) * thickness) *  2.0;
    vt += DepthSample(uv + float2( 1.0, -1.0) * thickness) *  1.0;
    vt += DepthSample(uv + float2(-1.0,  1.0) * thickness) * -1.0;
    vt += DepthSample(uv + float2( 0.0,  1.0) * thickness) * -2.0;
    vt += DepthSample(uv + float2( 1.0,  1.0) * thickness) * -1.0;
    
    result = sqrt(hr * hr + vt * vt);
}

#endif // BT_SOBEL_INCLUDED
