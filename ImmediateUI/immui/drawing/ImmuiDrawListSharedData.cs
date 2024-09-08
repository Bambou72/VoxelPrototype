using ImmediateUI.immui.math;
using OpenTK.Mathematics;

namespace ImmediateUI.immui.drawing
{
    public class ImmuiDrawListSharedData
    {
        public float CircleSegmentMaxError;
        public float ArcFastRadiusCutoff;
        public float CurveTessellationTol = 1.25f;
        public byte[] CircleSegmentCounts = new byte[64];
        public Vector2[] ArcFastVtx = new Vector2[Immui.ARCFAST_TABLE_SIZE];
        public DrawListFlags InitialFlags = DrawListFlags.AntiAliasedLines | DrawListFlags.AntiAliasedFill;

        public ImmuiDrawListSharedData()
        {
            for (int i = 0; i < ArcFastVtx.Length; i++)
            {
                float a = (float)i * 2 * MathF.PI / ArcFastVtx.Length;
                ArcFastVtx[i] = new(MathF.Cos(a), MathF.Sin(a));
            }
            ArcFastRadiusCutoff = Immui.CircleAutoSegmentCalcR(Immui.ARCFAST_SAMPLE_MAX, CircleSegmentMaxError);
            SetCircleTessellationMaxError(0.30f);
        }
        void SetCircleTessellationMaxError(float MaxError)
        {
            if (CircleSegmentMaxError == MaxError)
                return;
            CircleSegmentMaxError = MaxError;
            for (int i = 0; i < CircleSegmentCounts.Length; i++)
            {
                float radius = i;
                CircleSegmentCounts[i] = (byte)(i > 0 ? Immui.CircleAutoSegmentCalc(radius, CircleSegmentMaxError) : Immui.ARCFAST_SAMPLE_MAX);
            }
            ArcFastRadiusCutoff = Immui.CircleAutoSegmentCalcR(Immui.ARCFAST_SAMPLE_MAX, CircleSegmentMaxError);
        }
    }
}
