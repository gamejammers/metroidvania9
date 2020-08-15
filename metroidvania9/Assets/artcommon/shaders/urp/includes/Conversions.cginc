//
// (c) BLACKTRIANGLES 2020
// http://www.blacktriangles.com
//

const float PI = 3.1415926535897932384626433832795028841971693993751058209749;

//
// ----------------------------------------------------------------------------
//

float2 toPolar(float2 uv)
{
    return float2(
        sqrt(uv.x*uv.x+uv.y*uv.y),
        atan(uv.y/uv.x)
    );
}
