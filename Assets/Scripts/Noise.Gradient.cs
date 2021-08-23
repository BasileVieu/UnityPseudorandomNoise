using Unity.Mathematics;
using static Unity.Mathematics.math;

public static partial class Noise
{
    public interface IGradient
    {
        float4 Evaluate(SmallXXHash4 _hash, float4 _x);
        
        float4 Evaluate(SmallXXHash4 _hash, float4 _x, float4 _y);
        
        float4 Evaluate(SmallXXHash4 _hash, float4 _x, float4 _y, float4 _z);

        float4 EvaluateAfterInterpolation(float4 _value);
    }

    public struct Value : IGradient
    {
        public float4 Evaluate(SmallXXHash4 _hash, float4 _x) => _hash.Floats01A * 2.0f - 1.0f;
        
        public float4 Evaluate(SmallXXHash4 _hash, float4 _x, float4 _y) => _hash.Floats01A * 2.0f - 1.0f;
        
        public float4 Evaluate(SmallXXHash4 _hash, float4 _x, float4 _y, float4 _z) => _hash.Floats01A * 2.0f - 1.0f;
        
        public float4 EvaluateAfterInterpolation(float4 _value) => _value;
    }

    public struct Perlin : IGradient
    {
        public float4 Evaluate(SmallXXHash4 _hash, float4 _x) => (1.0f + _hash.Floats01A) * select(-_x, _x, ((uint4)_hash & 1 << 8) == 0);

        public float4 Evaluate(SmallXXHash4 _hash, float4 _x, float4 _y)
        {
            float4 gx = _hash.Floats01A * 2.0f - 1.0f;
            float4 gy = 0.5f - abs(gx);

            gx -= floor(gx + 0.5f);

            return (gx * _x + gy * _y) * (2.0f / 0.53528f);
        }

        public float4 Evaluate(SmallXXHash4 _hash, float4 _x, float4 _y, float4 _z)
        {
            float4 gx = _hash.Floats01A * 2.0f - 1.0f;
            float4 gy = _hash.Floats01D * 2.0f - 1.0f;
            float4 gz = 1.0f - abs(gx) - abs(gy);
            float4 offset = max(-gz, 0.0f);

            gx += select(-offset, offset, gx < 0.0f);
            gy += select(-offset, offset, gy < 0.0f);

            return (gx * _x + gy * _y + gz * _z) * (1.0f / 0.56290f);
        }

        public float4 EvaluateAfterInterpolation(float4 _value) => _value;
    }

    public struct Turbulence<G> : IGradient where G : IGradient
    {
        public float4 Evaluate(SmallXXHash4 _hash, float4 _x) => default(G).Evaluate(_hash, _x);

        public float4 Evaluate(SmallXXHash4 _hash, float4 _x, float4 _y) => default(G).Evaluate(_hash, _x, _y);

        public float4 Evaluate(SmallXXHash4 _hash, float4 _x, float4 _y, float4 _z) => default(G).Evaluate(_hash, _x, _y, _z);

        public float4 EvaluateAfterInterpolation(float4 _value) => abs(default(G).EvaluateAfterInterpolation(_value));
    }
}