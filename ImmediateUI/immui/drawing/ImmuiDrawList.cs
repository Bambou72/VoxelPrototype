using OpenTK.Mathematics;
using ImmediateUI.immui.font;
using ImmediateUI.immui.math;
using ImmediateUI.immui.utils;
using System.Collections.Generic;
namespace ImmediateUI.immui.drawing
{
    [Flags]
    public enum DrawFlags
    {
        None = 0,
        Closed = 1 << 0,
        RoundCornersTopLeft = 1 << 4,
        RoundCornersTopRight = 1 << 5,
        RoundCornersBottomRight = 1 << 6,
        RoundCornersBottomLeft = 1 << 7,
        RoundCornersNone = 1 << 8,
        RoundCornersTop = RoundCornersTopLeft | RoundCornersTopRight,
        RoundCornersBottom = RoundCornersBottomLeft | RoundCornersBottomRight,
        RoundCornersLeft = RoundCornersBottomLeft | RoundCornersTopLeft,
        RoundCornersRight = RoundCornersBottomRight | RoundCornersTopRight,
        RoundCornersAll = RoundCornersBottomRight | RoundCornersTopRight | RoundCornersBottomLeft | RoundCornersTopLeft,
        RoundCornersMask = RoundCornersAll | RoundCornersNone,
    }
    [Flags]
    public enum DrawListFlags
    {
        None = 0,
        AntiAliasedLines = 1,
        AntiAliasedFill = 2,
    };

    public class ImmuiDrawList
    {
        public DrawListFlags DrawListFlags;
        public List<ImmuiDrawCommand> Commands = new();
        public List<Vertex> VertexBuffer = new();
        public List<uint> IndexBuffer = new();
        internal uint VtxCurrentIdx;
        internal int VertexPtr = 0;
        internal int IndexPtr = 0;
        public List<Rect> ClipStack = new();
        public List<int> TextureStack = new();
        public Vector<Vector2> Path = new();
        public ImmuiDrawCommandHeader CmdHeader = new();
        public ImmuiDrawListSharedData Data;
        public float FringeScale = 1;
        public ImmuiDrawList(ImmuiDrawListSharedData Data)
        {
            this.Data = Data;
        }

        //
        //Primitives
        //
        public void AddLine(Vector2 Start, Vector2 End, uint Color, float Thickness = 1)
        {
            if ((Color & Immui.AlphaMask) == 0) return;
            PathLineTo(Start + new Vector2(0.5f));
            PathLineTo(End + new Vector2(0.5f));
            PathStroke(Color, 0, Thickness);
        }
        public void AddRect(Vector2 Min, Vector2 Max, uint Color, float Rounding = 0, DrawFlags Flags = DrawFlags.None, float Thickness = 1)
        {
            if ((Color & Immui.AlphaMask) == 0) return;
            if ((DrawListFlags & DrawListFlags.AntiAliasedLines) != 0)
            {
                PathRect(Min + new Vector2(0.5f), Max - new Vector2(0.5f), Rounding, Flags);
            }
            else
            {
                PathRect(Min + new Vector2(0.5f), Max - new Vector2(0.49f), Rounding, Flags);
            }
            PathStroke(Color, DrawFlags.Closed, Thickness);
        }
        public void AddRectFilled(Vector2 Min, Vector2 Max, uint Color, float Rounding = 0, DrawFlags Flags = DrawFlags.None)
        {
            if ((Color & Immui.AlphaMask) == 0) return;
            if (Rounding < 0.5f || (Flags & DrawFlags.RoundCornersMask) == DrawFlags.RoundCornersNone)
            {
                PrimReserve(6, 4);
                PrimRect(Min, Max, Color);

            }
            else
            {
                PathRect(Min, Max, Rounding, Flags);
                PathFillConvex(Color);
            }
        }
        public void AddRectFilledMultiColor(Vector2 Min, Vector2 Max, uint ColorBottomLeft, uint ColorBottomRight, uint ColorTopLeft, uint ColorTopRight, float Rounding = 0, DrawFlags Flags = DrawFlags.None)
        {
            if (((ColorBottomLeft | ColorBottomRight | ColorTopLeft | ColorTopRight) & Immui.AlphaMask) == 0)
                return;
            PrimReserve(6, 4);
            PrimWriteIdx(VtxCurrentIdx); PrimWriteIdx(VtxCurrentIdx + 1); PrimWriteIdx(VtxCurrentIdx + 2);
            PrimWriteIdx(VtxCurrentIdx); PrimWriteIdx(VtxCurrentIdx + 2); PrimWriteIdx(VtxCurrentIdx + 3);
            PrimWriteVtx(Min, Vector2.Zero, ColorTopLeft);
            PrimWriteVtx(new(Max.X, Min.Y), Vector2.Zero, ColorTopRight);
            PrimWriteVtx(Max, Vector2.Zero, ColorBottomRight);
            PrimWriteVtx(new(Min.X, Max.Y), Vector2.Zero, ColorBottomLeft);
        }
        public void AddQuad(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4, uint Color, float Thickness = 1)
        {
            if ((Color & Immui.AlphaMask) == 0)
                return;
            PathLineTo(P1);
            PathLineTo(P2);
            PathLineTo(P3);
            PathLineTo(P4);
            PathStroke(Color, DrawFlags.Closed, Thickness);
        }
        public void AddQuadFilled(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4, uint Color)
        {
            if ((Color & Immui.AlphaMask) == 0)
                return;
            PathLineTo(P1);
            PathLineTo(P2);
            PathLineTo(P3);
            PathLineTo(P4);
            PathFillConvex(Color);
        }
        public void AddTriangle(Vector2 P1, Vector2 P2, Vector2 P3, uint Color, float Thickness = 1)
        {
            if ((Color & Immui.AlphaMask) == 0)
                return;

            PathLineTo(P1);
            PathLineTo(P2);
            PathLineTo(P3);
            PathStroke(Color, DrawFlags.Closed, Thickness);
        }
        public void AddTriangleFilled(Vector2 P1, Vector2 P2, Vector2 P3, uint Color)
        {
            if ((Color & Immui.AlphaMask) == 0)
                return;

            PathLineTo(P1);
            PathLineTo(P2);
            PathLineTo(P3);
            PathFillConvex(Color);
        }
        public void AddCircle(Vector2 Center, float Radius, uint Color, int NumOfSegments = 0, float Thickness = 1)
        {
            if ((Color & Immui.AlphaMask) == 0 || Radius < 0.5f)
                return;

            if (NumOfSegments <= 0)
            {
                PathArcToFastEx(Center, Radius - 0.5f, 0, Immui.ARCFAST_SAMPLE_MAX, 0);
                Path.Size--;
            }
            else
            {
                NumOfSegments = Math.Clamp(NumOfSegments, 3, Immui.CIRCLE_AUTO_SEGMENT_MAX);
                float a_max = MathF.PI * 2.0f * (NumOfSegments - 1.0f) / NumOfSegments;
                PathArcTo(Center, Radius - 0.5f, 0.0f, a_max, NumOfSegments - 1);
            }

            PathStroke(Color, DrawFlags.Closed, Thickness);
        }
        public void AddCircleFilled(Vector2 Center, float Radius, uint Color, int NumOfSegments = 0)
        {
            if ((Color & Immui.AlphaMask) == 0 || Radius < 0.5f)
                return;

            if (NumOfSegments <= 0)
            {
                PathArcToFastEx(Center, Radius - 0.5f, 0, Immui.ARCFAST_SAMPLE_MAX, 0);
                Path.Size--;
            }
            else
            {
                NumOfSegments = Math.Clamp(NumOfSegments, 3, Immui.CIRCLE_AUTO_SEGMENT_MAX);

                float a_max = MathF.PI * 2.0f * (NumOfSegments - 1.0f) / NumOfSegments;
                PathArcTo(Center, Radius, 0.0f, a_max, NumOfSegments - 1);
            }

            PathFillConvex(Color);
        }
        public void AddNGon(Vector2 Center, float Radius, uint Color, int NumOfSegments = 0, float Thickness = 1)
        {
            if ((Color & Immui.AlphaMask) == 0 || NumOfSegments <= 2)
                return;
            float a_max = MathF.PI * 2.0f * (NumOfSegments - 1.0f) / NumOfSegments;
            PathArcTo(Center, Radius - 0.5f, 0.0f, a_max, NumOfSegments - 1);
            PathStroke(Color, DrawFlags.Closed, Thickness);
        }
        public void AddNGonFilled(Vector2 Center, float Radius, uint Color, int NumOfSegments = 0)
        {
            if ((Color & Immui.AlphaMask) == 0 || NumOfSegments <= 2)
                return;
            float a_max = MathF.PI * 2.0f * (NumOfSegments - 1.0f) / NumOfSegments;
            PathArcTo(Center, Radius, 0.0f, a_max, NumOfSegments - 1);
            PathFillConvex(Color);
        }
        public void AddEllipse(Vector2 Center, Vector2 Radius, uint Color, float Rotation = 0f, int NumOfSegments = 0, float Thickness = 1)
        {
            if ((Color & Immui.AlphaMask) == 0)
                return;

            if (NumOfSegments <= 0)
                NumOfSegments = CalcCircleAutoSegmentCount(Math.Max(Radius.X, Radius.Y));
            float a_max = MathF.PI * 2.0f * (NumOfSegments - 1.0f) / NumOfSegments;
            PathEllipticalArcTo(Center, Radius, Rotation, 0, a_max, NumOfSegments - 1);
            PathStroke(Color, DrawFlags.Closed, Thickness);
        }
        public void AddEllipseFilled(Vector2 Center, Vector2 Radius, uint Color, float Rotation = 0, int NumOfSegments = 0)
        {
            if ((Color & Immui.AlphaMask) == 0)
                return;
            if (NumOfSegments <= 0)
                NumOfSegments = CalcCircleAutoSegmentCount(Math.Max(Radius.X, Radius.Y));
            float a_max = MathF.PI * 2.0f * (NumOfSegments - 1.0f) / NumOfSegments;
            PathEllipticalArcTo(Center, Radius, Rotation, 0.0f, a_max, NumOfSegments - 1);
            PathFillConvex(Color);
        }
        public void AddText(Vector2 Position, string Text, uint Color)
        {
            AddText(null , 0.0f, Position, Text, Color);
        }
        //BUG : Make memory leak and pressure on GC when big string 
        public void AddText(Font Font, float Scale, Vector2 Position, string Text, uint Color, float WrapWidth = 0, Rect CpuClip = default)
        {
            if ((Color & Immui.AlphaMask) == 0)
            {
                return;
            }
            if (Font == null)
            {
               Font = Immui.GetCurrentFont();
            }
            if (Scale == 0.0f)
            {
                Scale = Font.Height;
            }
            Rect clip_rect = CmdHeader.ClipRect;
            if (!CpuClip.Equals(default))
            {
                clip_rect.Min.X = Math.Max(clip_rect.Min.X, CpuClip.Min.X);
                clip_rect.Min.Y = Math.Max(clip_rect.Min.Y, CpuClip.Min.Y);
                clip_rect.Max.X = Math.Min(clip_rect.Max.X, CpuClip.Max.X);
                clip_rect.Max.Y = Math.Min(clip_rect.Max.Y, CpuClip.Max.Y);
            }
            CmdHeader.ClipRect = clip_rect;
            PushTextureID(Font.Atlas.GetHandle());
            Font.RenderText(this, Scale, Position, Color, clip_rect, Text, WrapWidth, !CpuClip.Equals(default));
            PopTextureID();
        }
        public void AddBezierCurveCubic(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4, uint Color, float Thickness, int NumOfSegments = 0)
        {
            if ((Color & Immui.AlphaMask) == 0)
            {
                return;
            }
            PathLineTo(P1);
            PathBezierCubicCurveTo(P2, P3, P4, NumOfSegments);
            PathStroke(Color, 0, Thickness);
        }
        public void AddBezierCurveQuadratic(Vector2 P1, Vector2 P2, Vector2 P3, uint Color, float Thickness, int NumOfSegments = 0)
        {
            if ((Color & Immui.AlphaMask) == 0)
            {
                return;
            }
            PathLineTo(P1);
            PathBezierQuadraticCurveTo(P2, P3, NumOfSegments);
            PathStroke(Color, 0, Thickness);
        }
        //
        //Polygons
        //
        public void AddPolyLine(Vector2[] Points, int PointsCount, uint Color, DrawFlags Flags, float Thickness)
        {
            if (PointsCount < 2 || (Color & Immui.AlphaMask) == 0)
            {
                return;
            }
            bool closed = (Flags & DrawFlags.Closed) != 0;
            Vector2 opaqueUv = Vector2.Zero;
            int count = closed ? PointsCount : PointsCount - 1;
            bool thickLine = Thickness > FringeScale;
            if ((DrawListFlags & DrawListFlags.AntiAliasedLines) != 0)
            {
                float AA_SIZE = FringeScale;
                uint col_trans = Color & ~Immui.AlphaMask;

                Thickness = Math.Max(Thickness, 1.0f);
                int integer_thickness = (int)Thickness;
                float fractional_thickness = Thickness - integer_thickness;
                int idx_count = thickLine ? count * 18 : count * 12;
                int vtx_count = thickLine ? PointsCount * 4 : PointsCount * 3;
                PrimReserve(idx_count, vtx_count);
                List<Vector2> temp_normals = new List<Vector2>();
                temp_normals.Resize(PointsCount);
                List<Vector2> temp_points = new List<Vector2>();
                temp_points.Resize(PointsCount * (!thickLine ? 2 : 4));
                for (int i1 = 0; i1 < count; i1++)
                {
                    int i2 = i1 + 1 == PointsCount ? 0 : i1 + 1;
                    float dx = Points[i2].X - Points[i1].X;
                    float dy = Points[i2].Y - Points[i1].Y;
                    Immui.Normalize(ref dx, ref dy);
                    temp_normals[i1] = new(dy, -dx);
                }
                if (!closed)
                {
                    temp_normals[PointsCount - 1] = temp_normals[PointsCount - 2];
                }
                if (!thickLine)
                {
                    float half_draw_size = AA_SIZE;

                    if (!closed)
                    {
                        temp_points[0] = Points[0] + temp_normals[0] * half_draw_size;
                        temp_points[1] = Points[0] - temp_normals[0] * half_draw_size;
                        temp_points[(PointsCount - 1) * 2 + 0] = Points[PointsCount - 1] + temp_normals[PointsCount - 1] * half_draw_size;
                        temp_points[(PointsCount - 1) * 2 + 1] = Points[PointsCount - 1] - temp_normals[PointsCount - 1] * half_draw_size;
                    }
                    uint idx1 = VtxCurrentIdx;
                    for (int i1 = 0; i1 < count; i1++)
                    {
                        int i2 = i1 + 1 == PointsCount ? 0 : i1 + 1;
                        uint idx2 = i1 + 1 == PointsCount ? VtxCurrentIdx : idx1 + 3;

                        float dm_x = (temp_normals[i1].X + temp_normals[i2].X) * 0.5f;
                        float dm_y = (temp_normals[i1].Y + temp_normals[i2].Y) * 0.5f;
                        Immui.FixNormal(ref dm_x, ref dm_y);
                        dm_x *= half_draw_size;
                        dm_y *= half_draw_size;

                        temp_points[i2 * 2] = new(Points[i2].X + dm_x, Points[i2].Y + dm_y);
                        temp_points[i2 * 2 + 1] = new(Points[i2].X - dm_x, Points[i2].Y - dm_y);

                        IndexBuffer[IndexPtr] = idx2 + 0;
                        IndexBuffer[IndexPtr + 1] = idx1 + 0;
                        IndexBuffer[IndexPtr + 2] = idx1 + 2;
                        IndexBuffer[IndexPtr + 3] = idx1 + 2;
                        IndexBuffer[IndexPtr + 4] = idx2 + 2;
                        IndexBuffer[IndexPtr + 5] = idx2 + 0;
                        IndexBuffer[IndexPtr + 6] = idx2 + 1;
                        IndexBuffer[IndexPtr + 7] = idx1 + 1;
                        IndexBuffer[IndexPtr + 8] = idx1 + 0;
                        IndexBuffer[IndexPtr + 9] = idx1 + 0;
                        IndexBuffer[IndexPtr + 10] = idx2 + 0;
                        IndexBuffer[IndexPtr + 11] = idx2 + 1;
                        IndexPtr += 12;
                        idx1 = idx2;
                    }
                    for (int i = 0; i < PointsCount; i++)
                    {
                        VertexBuffer[VertexPtr] = new(Points[i], opaqueUv, Color);
                        VertexBuffer[VertexPtr + 1] = new(temp_points[i * 2 + 0], opaqueUv, col_trans);
                        VertexBuffer[VertexPtr + 2] = new(temp_points[i * 2 + 1], opaqueUv, col_trans);
                        VertexPtr += 3;
                    }
                }
                else
                {
                    float halfInnerThickness = (Thickness - AA_SIZE) * 0.5f;
                    if (!closed)
                    {
                        int pointsLast = PointsCount - 1;
                        temp_points[0] = Points[0] + temp_normals[0] * (halfInnerThickness + AA_SIZE);
                        temp_points[1] = Points[0] + temp_normals[0] * halfInnerThickness;
                        temp_points[2] = Points[0] - temp_normals[0] * halfInnerThickness;
                        temp_points[3] = Points[0] - temp_normals[0] * (halfInnerThickness + AA_SIZE);
                        temp_points[pointsLast * 4 + 0] = Points[pointsLast] + temp_normals[pointsLast] * (halfInnerThickness + AA_SIZE);
                        temp_points[pointsLast * 4 + 1] = Points[pointsLast] + temp_normals[pointsLast] * halfInnerThickness;
                        temp_points[pointsLast * 4 + 2] = Points[pointsLast] - temp_normals[pointsLast] * halfInnerThickness;
                        temp_points[pointsLast * 4 + 3] = Points[pointsLast] - temp_normals[pointsLast] * (halfInnerThickness + AA_SIZE);
                    }
                    uint idx1 = VtxCurrentIdx;
                    for (int i1 = 0; i1 < count; i1++)
                    {
                        int i2 = i1 + 1 == PointsCount ? 0 : i1 + 1;
                        uint idx2 = i1 + 1 == PointsCount ? VtxCurrentIdx : idx1 + 4U;
                        float dmX = (temp_normals[i1].X + temp_normals[i2].X) * 0.5f;
                        float dmY = (temp_normals[i1].Y + temp_normals[i2].Y) * 0.5f;
                        Immui.FixNormal(ref dmX, ref dmY);
                        float dmOutX = dmX * (halfInnerThickness + AA_SIZE);
                        float dmOutY = dmY * (halfInnerThickness + AA_SIZE);
                        float dmInX = dmX * halfInnerThickness;
                        float dmInY = dmY * halfInnerThickness;
                        temp_points[i2 * 4] = new(Points[i2].X + dmOutX, Points[i2].Y + dmOutY);
                        temp_points[i2 * 4 + 1] = new(Points[i2].X + dmInY, Points[i2].Y + dmInY);
                        temp_points[i2 * 4 + 2] = new(Points[i2].X - dmInX, Points[i2].Y - dmInY);
                        temp_points[i2 * 4 + 3] = new(Points[i2].X - dmOutX, Points[i2].Y - dmOutY);
                        IndexBuffer[IndexPtr] = idx2 + 1;
                        IndexBuffer[IndexPtr + 1] = idx1 + 1;
                        IndexBuffer[IndexPtr + 2] = idx1 + 2;
                        IndexBuffer[IndexPtr + 3] = idx1 + 2;
                        IndexBuffer[IndexPtr + 4] = idx2 + 2;
                        IndexBuffer[IndexPtr + 5] = idx2 + 1;
                        IndexBuffer[IndexPtr + 6] = idx2 + 1;
                        IndexBuffer[IndexPtr + 7] = idx1 + 1;
                        IndexBuffer[IndexPtr + 8] = idx1 + 0;
                        IndexBuffer[IndexPtr + 9] = idx1 + 0;
                        IndexBuffer[IndexPtr + 10] = idx2 + 0;
                        IndexBuffer[IndexPtr + 11] = idx2 + 1;
                        IndexBuffer[IndexPtr + 12] = idx2 + 2;
                        IndexBuffer[IndexPtr + 13] = idx1 + 2;
                        IndexBuffer[IndexPtr + 14] = idx1 + 3;
                        IndexBuffer[IndexPtr + 15] = idx1 + 3;
                        IndexBuffer[IndexPtr + 16] = idx2 + 3;
                        IndexBuffer[IndexPtr + 17] = idx2 + 2;
                        IndexPtr += 18;
                        idx1 = idx2;
                    }
                    for (int i = 0; i < PointsCount; i++)
                    {
                        VertexBuffer[VertexPtr] = new(temp_points[i * 4 + 0], opaqueUv, Color);
                        VertexBuffer[VertexPtr + 1] = new(temp_points[i * 4 + 1], opaqueUv, Color);
                        VertexBuffer[VertexPtr + 2] = new(temp_points[i * 4 + 2], opaqueUv, Color);
                        VertexBuffer[VertexPtr + 3] = new(temp_points[i * 4 + 3], opaqueUv, Color);
                        VertexPtr += 4;
                    }
                }
                VtxCurrentIdx += (uint)vtx_count;
            }
            else
            {
                int idx_count = count * 6;
                int vtx_count = count * 4;
                PrimReserve(idx_count, vtx_count);

                for (int i1 = 0; i1 < count; i1++)
                {
                    int i2 = i1 + 1 == PointsCount ? 0 : i1 + 1;
                    Vector2 p1 = Points[i1];
                    Vector2 p2 = Points[i2];

                    float dx = p2.X - p1.X;
                    float dy = p2.Y - p1.Y;
                    Immui.Normalize(ref dx, ref dy);
                    dx *= Thickness * 0.5f;
                    dy *= Thickness * 0.5f;

                    VertexBuffer[VertexPtr] = new Vertex { Position = new(p1.X + dy, p1.Y - dx), UV = opaqueUv, Color = Color };
                    VertexBuffer[VertexPtr + 1] = new Vertex { Position = new(p2.X + dy, p2.Y - dx), UV = opaqueUv, Color = Color };
                    VertexBuffer[VertexPtr + 2] = new Vertex { Position = new(p2.X - dy, p2.Y + dx), UV = opaqueUv, Color = Color };
                    VertexBuffer[VertexPtr + 3] = new Vertex { Position = new(p1.X - dy, p1.Y + dx), UV = opaqueUv, Color = Color };
                    VertexPtr += 4;
                    IndexBuffer[IndexPtr] = VtxCurrentIdx;
                    IndexBuffer[IndexPtr + 1] = VtxCurrentIdx + 1;
                    IndexBuffer[IndexPtr + 2] = VtxCurrentIdx + 2;
                    IndexBuffer[IndexPtr + 3] = VtxCurrentIdx;
                    IndexBuffer[IndexPtr + 4] = VtxCurrentIdx + 2;
                    IndexBuffer[IndexPtr + 5] = VtxCurrentIdx + 3;
                    IndexPtr += 6;
                    VtxCurrentIdx += 4;
                }
            }
        }
        public void AddConvexPolyFiled(Vector2[] Points, int PointsCount, uint Color)
        {
            if (PointsCount < 3 || (Color & Immui.AlphaMask) == 0)
                return;

            Vector2 uv = Vector2.Zero;

            if ((DrawListFlags & DrawListFlags.AntiAliasedFill) != 0)
            {
                float AA_SIZE = FringeScale;
                uint col_trans = Color & ~Immui.AlphaMask;
                int idx_count = (PointsCount - 2) * 3 + PointsCount * 6;
                int vtx_count = PointsCount * 2;
                PrimReserve(idx_count, vtx_count);

                uint vtx_inner_idx = VtxCurrentIdx;
                uint vtx_outer_idx = VtxCurrentIdx + 1;
                for (int i = 2; i < PointsCount; i++)
                {
                    IndexBuffer[IndexPtr] = vtx_inner_idx;
                    IndexBuffer[IndexPtr + 1] = (uint)(vtx_inner_idx + (i - 1 << 1));
                    IndexBuffer[IndexPtr + 2] = (uint)(vtx_inner_idx + (i << 1));
                    IndexPtr += 3;
                }
                List<Vector2> temp_normals = new();
                temp_normals.Resize(PointsCount);
                for (int i0 = PointsCount - 1, i1 = 0; i1 < PointsCount; i0 = i1++)
                {
                    Vector2 p0 = Points[i0];
                    Vector2 p1 = Points[i1];
                    float dx = p1.X - p0.X;
                    float dy = p1.Y - p0.Y;
                    Immui.Normalize(ref dx, ref dy);
                    var TempVec = temp_normals[i0];
                    temp_normals[i0] = new(dy, -dx);
                }

                for (int i0 = PointsCount - 1, i1 = 0; i1 < PointsCount; i0 = i1++)
                {
                    Vector2 n0 = temp_normals[i0];
                    Vector2 n1 = temp_normals[i1];
                    float dm_x = (n0.X + n1.X) * 0.5f;
                    float dm_y = (n0.Y + n1.Y) * 0.5f;
                    Immui.FixNormal(ref dm_x, ref dm_y);
                    dm_x *= AA_SIZE * 0.5f;
                    dm_y *= AA_SIZE * 0.5f;
                    //
                    VertexBuffer[VertexPtr] = new Vertex { Position = new(Points[i1].X - dm_x, Points[i1].Y - dm_y), UV = uv, Color = Color };
                    VertexBuffer[VertexPtr + 1] = new Vertex { Position = new(Points[i1].X + dm_x, Points[i1].Y + dm_y), UV = uv, Color = col_trans };
                    VertexPtr += 2;
                    //
                    IndexBuffer[IndexPtr] = (uint)(vtx_inner_idx + (i1 << 1));
                    IndexBuffer[IndexPtr + 1] = (uint)(vtx_inner_idx + (i0 << 1));
                    IndexBuffer[IndexPtr + 2] = (uint)(vtx_outer_idx + (i0 << 1));
                    IndexBuffer[IndexPtr + 3] = (uint)(vtx_outer_idx + (i0 << 1));
                    IndexBuffer[IndexPtr + 4] = (uint)(vtx_outer_idx + (i1 << 1));
                    IndexBuffer[IndexPtr + 5] = (uint)(vtx_inner_idx + (i1 << 1));
                    IndexPtr += 6;
                }
                VtxCurrentIdx += (uint)vtx_count;
            }
            else
            {
                int idx_count = (PointsCount - 2) * 3;
                int vtx_count = PointsCount;
                PrimReserve(idx_count, vtx_count);
                for (int i = 0; i < vtx_count; i++)
                {
                    VertexBuffer[VertexPtr] = new() { Position = Points[i], UV = uv, Color = Color };
                    VertexPtr += 1;
                }
                for (int i = 2; i < PointsCount; i++)
                {
                    IndexBuffer[IndexPtr] = VtxCurrentIdx;
                    IndexBuffer[IndexPtr + 1] = (uint)(VtxCurrentIdx + i - 1);
                    IndexBuffer[IndexPtr + 2] = (uint)(VtxCurrentIdx + i);
                    IndexPtr += 3;
                }
                VtxCurrentIdx += (uint)vtx_count;
            }

        }
        //
        //Images
        //
        public void AddImage(int Texture, Vector2 Min, Vector2 Max, Vector2 UV0 = default, Vector2 UV1 = default, uint Color = 0xFFFFFFFF)
        {
            if (UV0 == default)
                UV0 = Vector2.Zero;
            if (UV1 == default)
                UV1 = Vector2.One;
            if ((Color & Immui.AlphaMask) == 0)
            {
                return;
            }
            bool push_texture_id = Texture != CmdHeader.TextureID;
            if (push_texture_id)
            {
                PushTextureID(Texture);
            }
            PrimReserve(6, 4);
            PrimRectUV(Min, Max, UV0, UV1, Color);
            if (push_texture_id)
            {
                PopTextureID();
            }
        }
        public void AddImageQuad(int Texture, Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4, Vector2 UV0 = default, Vector2 UV1 = default, Vector2 UV2 = default, Vector2 UV3 = default, uint Color = 0xFFFFFFFF)
        {
            if (UV0 == default) UV0 = Vector2.Zero;
            if (UV1 == default) UV1 = new(1, 0);
            if (UV2 == default) UV2 = Vector2.One;
            if (UV3 == default) UV3 = new(0, 1);
            if ((Color & Immui.AlphaMask) == 0)
            {
                return;
            }
            bool push_texture_id = Texture != CmdHeader.TextureID;
            if (push_texture_id)
            {
                PushTextureID(Texture);
            }
            PrimReserve(6, 4);
            PrimQuadUV(P1, P2, P3, P4, UV0, UV1, UV2, UV3, Color);
            if (push_texture_id)
            {
                PopTextureID();
            }
        }
        public void AddImageRounded(int Texture, Vector2 Min, Vector2 Max, Vector2 UV0, Vector2 UV1, uint Color, float Rounding, DrawFlags Flags = 0)
        {
            if ((Color & Immui.AlphaMask) == 0)
            {
                return;
            }
            Flags = FixRectCornerFlags(Flags);
            if (Rounding < 0.5f || (Flags & DrawFlags.RoundCornersMask) == DrawFlags.RoundCornersNone)
            {
                AddImage(Texture, Min, Max, Min, Min, Color);
                return;
            }
            bool push_texture_id = Texture != CmdHeader.TextureID;
            if (push_texture_id)
            {
                PushTextureID(Texture);
            }
            int vert_start_idx = VertexBuffer.Count;
            PathRect(Min, Max, Rounding, Flags);
            PathFillConvex(Color);
            int vert_end_idx = VertexBuffer.Count;
            ShadeVertsLinearUV(this, vert_start_idx, vert_end_idx, Min, Max, Min, Max, true);
            if (push_texture_id)
            {
                PopTextureID();
            }
        }
        //
        //Path API
        //
        public void PathClear() => Path.Size=0;
        public void PathLineTo(Vector2 Position) => Path.PushBack(Position);
        public void PathLineToMergeDuplicate(Vector2 Position) { if (Path.Size == 0 || !Path[Path.Size - 1].Equals(Position)) Path.PushBack(Position); }
        public void PathFillConvex(uint Color) { AddConvexPolyFiled(Path.Data, Path.Size, Color); PathClear(); }
        public void PathStroke(uint Color, DrawFlags Flags = DrawFlags.None, float Thickness = 1) { AddPolyLine(Path.Data, Path.Size, Color, Flags, Thickness); PathClear(); }
        public void PathArcTo(Vector2 Center, float Radius, float AMin, float AMax, int NumOfSegments = 0)
        {
            if (Radius < 0.5f)
            {
                Path.PushBack(Center);
                return;
            }

            if (NumOfSegments > 0)
            {
                PathArcToN(Center, Radius, AMin, AMax, NumOfSegments);
                return;
            }

            if (Radius <= Data.ArcFastRadiusCutoff)
            {
                bool a_is_reverse = AMax < AMin;

                float a_min_sample_f = Immui.ARCFAST_SAMPLE_MAX * AMin / (MathF.PI * 2.0f);
                float a_max_sample_f = Immui.ARCFAST_SAMPLE_MAX * AMax / (MathF.PI * 2.0f);

                int a_min_sample = a_is_reverse ? (int)Math.Floor(a_min_sample_f) : (int)Math.Ceiling(a_min_sample_f);
                int a_max_sample = a_is_reverse ? (int)Math.Ceiling(a_max_sample_f) : (int)Math.Floor(a_max_sample_f);
                int a_mid_samples = a_is_reverse ? Math.Max(a_min_sample - a_max_sample, 0) : Math.Max(a_max_sample - a_min_sample, 0);

                float a_min_segment_angle = a_min_sample * MathF.PI * 2.0f / Immui.ARCFAST_SAMPLE_MAX;
                float a_max_segment_angle = a_max_sample * MathF.PI * 2.0f / Immui.ARCFAST_SAMPLE_MAX;
                bool a_emit_start = Math.Abs(a_min_segment_angle - AMin) >= 1e-5f;
                bool a_emit_end = Math.Abs(AMax - a_max_segment_angle) >= 1e-5f;

                if (a_emit_start)
                    Path.PushBack(new(Center.X + MathF.Cos(AMin) * Radius, Center.Y + MathF.Sin(AMin) * Radius));
                if (a_mid_samples > 0)
                    PathArcToFastEx(Center, Radius, a_min_sample, a_max_sample, 0);
                if (a_emit_end)
                    Path.PushBack(new(Center.X + MathF.Cos(AMax) * Radius, Center.Y + MathF.Sin(AMax) * Radius));
            }
            else
            {
                float arc_length = Math.Abs(AMax - AMin);
                int circle_segment_count = CalcCircleAutoSegmentCount(Radius);
                int arc_segment_count = Math.Max((int)Math.Ceiling(circle_segment_count * arc_length / (MathF.PI * 2.0f)), (int)(2.0f * MathF.PI / arc_length));
                PathArcToN(Center, Radius, AMin, AMax, arc_segment_count);
            }
        }
        public void PathArcToFast(Vector2 Center, float Radius, float AMinOf12, float AMaxOf12)
        {
            if (Radius < 0.5f)
            {
                Path.PushBack(Center);
                return;
            }
            PathArcToFastEx(Center, Radius, (int)(AMinOf12 * Immui.ARCFAST_SAMPLE_MAX / 12), (int)(AMaxOf12 * Immui.ARCFAST_SAMPLE_MAX / 12), 0);
        }
        public void PathEllipticalArcTo(Vector2 Center, Vector2 Radius, float Rotation, float AMin, float AMax, int NumOfSegments = 0)
        {
            if (NumOfSegments <= 0)
                NumOfSegments = CalcCircleAutoSegmentCount(Math.Max(Radius.X, Radius.Y));

            Path.Resize(Path.Size + NumOfSegments + 1);

            float cos_rot = MathF.Cos(Rotation);
            float sin_rot = MathF.Sin(Rotation);
            for (int i = 0; i <= NumOfSegments; i++)
            {
                float a = AMin + (float)i / NumOfSegments * (AMax - AMin);
                Vector2 point = new(MathF.Cos(a) * Radius.X, MathF.Sin(a) * Radius.Y);
                Vector2 rel = new(point.X * cos_rot - point.Y * sin_rot, point.X * sin_rot + point.Y * cos_rot);
                point.X = rel.X + Center.X;
                point.Y = rel.Y + Center.Y;

                Path[i] = point;
            }
        }

        public void PathBezierCubicCurveTo(Vector2 P1, Vector2 P2, Vector2 P3, int NumOfSegments = 0)
        {
            Vector2 p0 = Path[Path.Size - 1];
            if (NumOfSegments == 0)
            {
                PathBezierCubicCurveToCasteljau(Path, p0.X, p0.Y, P1.X, P1.Y, P2.X, P2.Y, P3.X, P3.Y, Data.CurveTessellationTol, 0); // Auto-tessellated
            }
            else
            {
                float t_step = 1.0f / NumOfSegments;
                for (int i_step = 1; i_step <= NumOfSegments; i_step++)
                    Path.PushBack(BezierCubicCalc(p0, P1, P2, P3, t_step * i_step));
            }
        }
        public void PathBezierQuadraticCurveTo(Vector2 P1, Vector2 P2, int NumOfSegments = 0)
        {
            Vector2 p0 = Path[Path.Size - 1];
            if (NumOfSegments == 0)
            {
                PathBezierQuadraticCurveToCasteljau(Path, p0.X, p0.Y, P1.X, P1.Y, P2.X, P2.Y, Data.CurveTessellationTol, 0);// Auto-tessellated
            }
            else
            {
                float t_step = 1.0f / NumOfSegments;
                for (int i_step = 1; i_step <= NumOfSegments; i_step++)
                    Path.PushBack(BezierQuadraticCalc(p0, P1, P2, t_step * i_step));
            }
        }
        public void PathRect(Vector2 Min, Vector2 Max, float Rounding = 0, DrawFlags Flags = DrawFlags.None)
        {
            if (Rounding >= 0.5f)
            {
                Flags = FixRectCornerFlags(Flags);
                Rounding = Math.Min(Rounding, Math.Abs(Max.X - Min.X) * ((Flags & DrawFlags.RoundCornersTop) == DrawFlags.RoundCornersTop || (Flags & DrawFlags.RoundCornersBottom) == DrawFlags.RoundCornersBottom ? 0.5f : 1.0f) - 1.0f);
                Rounding = Math.Min(Rounding, Math.Abs(Max.Y - Min.Y) * ((Flags & DrawFlags.RoundCornersLeft) == DrawFlags.RoundCornersLeft || (Flags & DrawFlags.RoundCornersRight) == DrawFlags.RoundCornersRight ? 0.5f : 1.0f) - 1.0f);
            }
            if (Rounding < 0.5f || (Flags & DrawFlags.RoundCornersMask) == DrawFlags.RoundCornersNone)
            {
                PathLineTo(Min);
                PathLineTo(new(Max.X, Min.Y));
                PathLineTo(Max);
                PathLineTo(new(Min.X, Max.Y));
            }
            else
            {
                float rounding_tl = (Flags & DrawFlags.RoundCornersTopLeft) != 0 ? Rounding : 0.0f;
                float rounding_tr = (Flags & DrawFlags.RoundCornersTopRight) != 0 ? Rounding : 0.0f;
                float rounding_br = (Flags & DrawFlags.RoundCornersBottomRight) != 0 ? Rounding : 0.0f;
                float rounding_bl = (Flags & DrawFlags.RoundCornersBottomLeft) != 0 ? Rounding : 0.0f;
                PathArcToFast(new(Min.X + rounding_tl, Min.Y + rounding_tl), rounding_tl, 6, 9);
                PathArcToFast(new(Max.X - rounding_tr, Min.Y + rounding_tr), rounding_tr, 9, 12);
                PathArcToFast(new(Max.X - rounding_br, Max.Y - rounding_br), rounding_br, 0, 3);
                PathArcToFast(new(Min.X + rounding_bl, Max.Y - rounding_bl), rounding_bl, 3, 6);
            }
        }
        //
        //Primitives Allocation
        //
        // OPT : This make go hard the GC
        internal void PrimReserve(int IndexCount, int VertexCount)
        {
            //WARN This is not the perfect solution i want
            var Command = Commands[Commands.Count - 1];
            Command.Count += IndexCount;
            Commands[Commands.Count - 1] = Command;
            //
            VertexBuffer.AddRange(Enumerable.Repeat(default(Vertex), VertexCount));
            IndexBuffer.AddRange(Enumerable.Repeat(default(Vertex), VertexCount));
        }

        private void PrimRect(Vector2 A, Vector2 C, uint Color)
        {
            uint Index = VtxCurrentIdx;
            //Index
            IndexBuffer[IndexPtr] = Index;
            IndexBuffer[IndexPtr + 1] = Index + 1;
            IndexBuffer[IndexPtr + 2] = Index + 2;
            IndexBuffer[IndexPtr + 3] = Index;
            IndexBuffer[IndexPtr + 4] = Index + 2;
            IndexBuffer[IndexPtr + 5] = Index + 3;
            //Vertex
            VertexBuffer[VertexPtr] = new(A, Vector2.Zero, Color);
            VertexBuffer[VertexPtr + 1] = new(new(C.X, A.Y), Vector2.Zero, Color);
            VertexBuffer[VertexPtr + 2] = new(C, Vector2.Zero, Color);
            VertexBuffer[VertexPtr + 3] = new(new(A.X, C.Y), Vector2.Zero, Color);
            VertexPtr += 4;
            VtxCurrentIdx += 4;
            IndexPtr += 6;
        }
        private void PrimRectUV(Vector2 A, Vector2 C, Vector2 UVA, Vector2 UVC, uint Color)
        {
            uint Index = VtxCurrentIdx;
            //Index
            IndexBuffer[IndexPtr] = Index;
            IndexBuffer[IndexPtr + 1] = Index + 1;
            IndexBuffer[IndexPtr + 2] = Index + 2;
            IndexBuffer[IndexPtr + 3] = Index;
            IndexBuffer[IndexPtr + 4] = Index + 2;
            IndexBuffer[IndexPtr + 5] = Index + 3;
            //Vertex
            VertexBuffer[VertexPtr] = new(A, UVA, Color);
            VertexBuffer[VertexPtr + 1] = new(new(C.X, A.Y), new(UVC.X, UVA.Y), Color);
            VertexBuffer[VertexPtr + 2] = new(C, UVC, Color);
            VertexBuffer[VertexPtr + 3] = new(new(A.X, C.Y), new(UVA.X, UVC.Y), Color);
            VertexPtr += 4;
            VtxCurrentIdx += 4;
            IndexPtr += 6;
        }
        private void PrimQuadUV(Vector2 A, Vector2 B, Vector2 C, Vector2 D, Vector2 UVA, Vector2 UVB, Vector2 UVC, Vector2 UVD, uint Color)
        {
            uint Index = VtxCurrentIdx;
            //Index
            IndexBuffer[IndexPtr] = Index;
            IndexBuffer[IndexPtr + 1] = Index + 1;
            IndexBuffer[IndexPtr + 2] = Index + 2;
            IndexBuffer[IndexPtr + 3] = Index;
            IndexBuffer[IndexPtr + 4] = Index + 2;
            IndexBuffer[IndexPtr + 5] = Index + 3;
            //Vertex
            VertexBuffer[VertexPtr] = new(A, UVA, Color);
            VertexBuffer[VertexPtr + 1] = new(B, UVB, Color);
            VertexBuffer[VertexPtr + 2] = new(C, UVC, Color);
            VertexBuffer[VertexPtr + 3] = new(D, UVD, Color);
            VertexPtr += 4;
            VtxCurrentIdx += 4;
            IndexPtr += 6;
        }
        private void PrimWriteVtx(Vector2 Position, Vector2 UV, uint Color)
        {
            VertexBuffer[VertexPtr] = new Vertex { Position = Position, UV = UV, Color = Color };
            VertexPtr++;
            VtxCurrentIdx++;
        }
        private void PrimWriteIdx(uint Index)
        {
            IndexBuffer[IndexPtr] = Index;
            IndexPtr++;
        }
        private void PrimVtx(Vector2 Position, Vector2 UV, uint Color)
        {
            PrimWriteIdx((uint)VertexBuffer.Count);
            PrimWriteVtx(Position, UV, Color);
        }
        //
        //Utils
        //

        //
        //Internals
        //
        private int CalcCircleAutoSegmentCount(float Radius)
        {
            int radius_idx = (int)(Radius + 0.999999f);
            if (radius_idx >= 0 && radius_idx < Data.CircleSegmentCounts.Length)
            {
                return Data.CircleSegmentCounts[radius_idx];
            }
            else
            {
                return Immui.CircleAutoSegmentCalc(Radius, Data.CircleSegmentMaxError);
            }
        }
        public void PushClipRect(Rect ClipRegion, bool IntersectWithCurrentClip = false)
        {
            if (IntersectWithCurrentClip)
            {
                Rect Current = CmdHeader.ClipRect;
                if (ClipRegion.Min.X < Current.Min.X) ClipRegion.Min.X = Current.Min.X;
                if (ClipRegion.Min.Y < Current.Min.Y) ClipRegion.Min.Y = Current.Min.Y;
                if (ClipRegion.Max.X > Current.Max.X) ClipRegion.Max.X = Current.Max.X;
                if (ClipRegion.Max.Y > Current.Max.Y) ClipRegion.Max.Y = Current.Max.Y;
            }
            ClipRegion.Max.X = Math.Max(ClipRegion.Min.X, ClipRegion.Max.X);
            ClipRegion.Max.Y = Math.Max(ClipRegion.Min.Y, ClipRegion.Max.Y);
            ClipStack.Add(ClipRegion);
            CmdHeader.ClipRect = ClipRegion;
            OnChangedClipRect();
        }
        public void PushClipRectFullScreen()
        {
            PushClipRect(new(Vector2.Zero,Immui.GetScreenSize()));
        }
        public void PopClipRect()
        {
            ClipStack.RemoveAt(ClipStack.Count - 1);
            CmdHeader.ClipRect = ClipStack.Count == 0 ? new(Vector2.Zero, Immui.GetScreenSize()) : ClipStack[ClipStack.Count - 1];
            OnChangedClipRect();
        }
        public void PushTextureID(int TextureID)
        {
            TextureStack.Add(TextureID);
            CmdHeader.TextureID = TextureID;
            OnChangedTextureID();
        }

        public void PopTextureID()
        {
            TextureStack.RemoveAt(TextureStack.Count - 1);
            CmdHeader.TextureID = TextureStack.Count == 0 ? 0 : TextureStack[TextureStack.Count - 1];
            OnChangedTextureID();
        }
        void SetTextureID(int TextureID)
        {
            if (CmdHeader.TextureID == TextureID)
                return;
            CmdHeader.TextureID = TextureID;
            OnChangedTextureID();
        }
        Vector2 GetClipRectMin() { Rect cr = ClipStack[^1]; return cr.Min; }

        Vector2 GetClipRectMax() { Rect cr = ClipStack[^1]; return cr.Max; }

        //
        //Helpers
        //
        internal void ResetForNewFrame()
        {
            Commands.Clear();
            IndexBuffer.Clear();
            VertexBuffer.Clear();
            DrawListFlags = Data.InitialFlags;
            CmdHeader = new();
            VtxCurrentIdx = 0;
            VertexPtr = 0;
            IndexPtr = 0;
            ClipStack.Clear();
            TextureStack.Clear();
            Path.Clear();
            //Splitter.Clear();
            Commands.Add(new ImmuiDrawCommand());
            FringeScale = 1.0f;
        }
        internal void PopUnusedDrawCmd()
        {
            while (Commands.Count > 0)
            {
                ImmuiDrawCommand curr_cmd = Commands[Commands.Count - 1];
                if (curr_cmd.Count != 0)
                    return;
                Commands.RemoveAt(Commands.Count - 1);
            }
        }
        internal void TryMergeDrawCmds()
        {
            ImmuiDrawCommand curr_cmd = Commands[Commands.Count - 1];
            ImmuiDrawCommand prev_cmd = Commands[Commands.Count - 2];
            if (curr_cmd.Equals(prev_cmd) == false && DrawCmdAreSequentialIdxOffset(prev_cmd, curr_cmd))
            {
                prev_cmd.Count += curr_cmd.Count;
                Commands[Commands.Count - 2] = prev_cmd;
                Commands.RemoveAt(Commands.Count - 1);
            }
        }
        internal void OnChangedClipRect()
        {
            ImmuiDrawCommand curr_cmd = Commands[Commands.Count - 1];
            if (curr_cmd.Count != 0 && curr_cmd.ClipRect.Equals(CmdHeader.ClipRect))
            {
                AddDrawCmd();
                return;
            }
            if (Commands.Count > 2)
            {
                ImmuiDrawCommand prev_cmd = Commands[Commands.Count - 2];
                if (curr_cmd.Count == 0 && Commands.Count > 1 && CmdHeader.Equals(prev_cmd) == false && DrawCmdAreSequentialIdxOffset(prev_cmd, curr_cmd))
                {
                    Commands.RemoveAt(Commands.Count - 1);
                    return;
                }

            }
            curr_cmd.ClipRect = CmdHeader.ClipRect;
            Commands[Commands.Count - 1] = curr_cmd;
        }
        internal void OnChangedTextureID()
        {
            ImmuiDrawCommand curr_cmd = Commands[Commands.Count - 1];
            if (curr_cmd.Count != 0 && curr_cmd.TextureID != CmdHeader.TextureID)
            {
                AddDrawCmd();
                return;
            }
            if (Commands.Count > 2)
            {

                ImmuiDrawCommand prev_cmd = Commands[Commands.Count - 2];
                if (curr_cmd.Count == 0 && Commands.Count > 1 && !CmdHeader.Equals(prev_cmd) && DrawCmdAreSequentialIdxOffset(prev_cmd, curr_cmd))
                {
                    Commands.RemoveAt(Commands.Count - 1);
                    return;
                }
            }
            curr_cmd.TextureID = CmdHeader.TextureID;
            Commands[Commands.Count - 1] = curr_cmd;
        }
        internal void PathArcToFastEx(Vector2 Center, float Radius, int AMinSample, int AMaxSample, int AStep)
        {
            if (Radius < 0.5f)
            {
                Path.PushBack(Center);
                return;
            }

            if (AStep <= 0)
                AStep = Immui.ARCFAST_SAMPLE_MAX / CalcCircleAutoSegmentCount(Radius);
            AStep = Math.Clamp(AStep, 1, Immui.ARCFAST_TABLE_SIZE / 4);
            int sample_range = Math.Abs(AMaxSample - AMinSample);
            int a_next_step = AStep;
            int samples = sample_range + 1;
            bool extra_max_sample = false;
            if (AStep > 1)
            {
                samples = sample_range / AStep + 1;
                int overstep = sample_range % AStep;

                if (overstep > 0)
                {
                    extra_max_sample = true;
                    samples++;
                    if (sample_range > 0)
                        AStep -= (AStep - overstep) / 2;
                }
            }

            Path.Resize(Path.Size + samples);
            Vector2 out_ptr = Path[Path.Size - samples];
            int Pointer = 0;
            int sample_index = AMinSample;
            if (sample_index < 0 || sample_index >= Immui.ARCFAST_SAMPLE_MAX)
            {
                sample_index = sample_index % Immui.ARCFAST_SAMPLE_MAX;
                if (sample_index < 0)
                    sample_index += Immui.ARCFAST_SAMPLE_MAX;
            }

            if (AMaxSample >= AMinSample)
            {
                for (int a = AMinSample; a <= AMaxSample; a += AStep, sample_index += AStep, AStep = a_next_step)
                {
                    out_ptr = Path[Path.Size - samples + Pointer];
                    if (sample_index >= Immui.ARCFAST_SAMPLE_MAX)
                        sample_index -= Immui.ARCFAST_SAMPLE_MAX;

                    Vector2 s = Data.ArcFastVtx[sample_index];
                    out_ptr.X = Center.X + s.X * Radius;
                    out_ptr.Y = Center.Y + s.Y * Radius;
                    Path[Path.Size - samples + Pointer] = out_ptr;
                    Pointer++;

                }
            }
            else
            {
                for (int a = AMinSample; a >= AMaxSample; a -= AStep, sample_index -= AStep, AStep = a_next_step)
                {
                    out_ptr = Path[Path.Size - samples + Pointer];

                    if (sample_index < 0)
                        sample_index += Immui.ARCFAST_SAMPLE_MAX;

                    Vector2 s = Data.ArcFastVtx[sample_index];
                    out_ptr.X = Center.X + s.X * Radius;
                    out_ptr.Y = Center.Y + s.Y * Radius;
                    Path[Path.Size - samples + Pointer] = out_ptr;
                    Pointer++;

                }
            }

            if (extra_max_sample)
            {
                int normalized_max_sample = AMaxSample % Immui.ARCFAST_SAMPLE_MAX;
                if (normalized_max_sample < 0)
                    normalized_max_sample += Immui.ARCFAST_SAMPLE_MAX;

                Vector2 s = Data.ArcFastVtx[normalized_max_sample];
                out_ptr.X = Center.X + s.X * Radius;
                out_ptr.Y = Center.Y + s.Y * Radius;
                Path[Path.Size - samples + Pointer] = out_ptr;
                Pointer++;
            }
        }
        internal void PathArcToN(Vector2 Center, float Radius, float AMin, float AMax, int ANumOfSegments)
        {
            if (Radius < 0.5f)
            {
                Path.PushBack(Center);
                return;
            }

            Path.Resize(Path.Size + ANumOfSegments + 1);
            for (int i = 0; i <= ANumOfSegments; i++)
            {
                float a = AMin + i / (float)ANumOfSegments * (AMax - AMin);
                Path[i] = new(Center.X + MathF.Cos(a) * Radius, Center.Y + MathF.Sin(a) * Radius);
            }
        }
        internal void AddDrawCmd()
        {
            ImmuiDrawCommand draw_cmd = new();
            draw_cmd.ClipRect = CmdHeader.ClipRect;
            draw_cmd.TextureID = CmdHeader.TextureID;
            draw_cmd.Offset = IndexBuffer.Count;
            Commands.Add(draw_cmd);
        }
        private static DrawFlags FixRectCornerFlags(DrawFlags flags)
        {
            if ((flags & DrawFlags.RoundCornersMask) == 0)
                flags |= DrawFlags.RoundCornersAll;
            return flags;
        }
        Vector2 BezierCubicCalc(Vector2 P1, Vector2 P2, Vector2 P3, Vector2 P4, float T)
        {
            float u = 1.0f - T;
            float w1 = u * u * u;
            float w2 = 3 * u * u * T;
            float w3 = 3 * u * T * T;
            float w4 = T * T * T;
            return new Vector2(w1 * P1.X + w2 * P2.X + w3 * P3.X + w4 * P4.X, w1 * P1.Y + w2 * P2.Y + w3 * P3.Y + w4 * P4.Y);
        }
        Vector2 BezierQuadraticCalc(Vector2 P1, Vector2 P2, Vector2 P3, float T)
        {
            float u = 1.0f - T;
            float w1 = u * u;
            float w2 = 2 * u * T;
            float w3 = T * T;
            return new Vector2(w1 * P1.X + w2 * P2.X + w3 * P3.X, w1 * P1.Y + w2 * P2.Y + w3 * P3.Y);
        }
        static void PathBezierCubicCurveToCasteljau(Vector<Vector2> Path, float X1, float Y1, float X2, float Y2, float X3, float Y3, float X4, float Y4, float TessTol, int Level)
        {
            float dx = X4 - X1;
            float dy = Y4 - Y1;
            float d2 = (X2 - X4) * dy - (Y2 - Y4) * dx;
            float d3 = (X3 - X4) * dy - (Y3 - Y4) * dx;
            d2 = d2 >= 0 ? d2 : -d2;
            d3 = d3 >= 0 ? d3 : -d3;
            if ((d2 + d3) * (d2 + d3) < TessTol * (dx * dx + dy * dy))
            {
                Path.PushBack(new(X4, Y4));
            }
            else if (Level < 10)
            {
                float x12 = (X1 + X2) * 0.5f, y12 = (Y1 + Y2) * 0.5f;
                float x23 = (X2 + X3) * 0.5f, y23 = (Y2 + Y3) * 0.5f;
                float x34 = (X3 + X4) * 0.5f, y34 = (Y3 + Y4) * 0.5f;
                float x123 = (x12 + x23) * 0.5f, y123 = (y12 + y23) * 0.5f;
                float x234 = (x23 + x34) * 0.5f, y234 = (y23 + y34) * 0.5f;
                float x1234 = (x123 + x234) * 0.5f, y1234 = (y123 + y234) * 0.5f;
                PathBezierCubicCurveToCasteljau(Path, X1, Y1, x12, y12, x123, y123, x1234, y1234, TessTol, Level + 1);
                PathBezierCubicCurveToCasteljau(Path, x1234, y1234, x234, y234, x34, y34, X4, Y4, TessTol, Level + 1);
            }
        }
        static void PathBezierQuadraticCurveToCasteljau(Vector<Vector2> Path, float X1, float Y1, float X2, float Y2, float X3, float Y3, float TessTol, int Level)
        {
            float dx = X3 - X1, dy = Y3 - Y1;
            float det = (X2 - X3) * dy - (Y2 - Y3) * dx;
            if (det * det * 4.0f < TessTol * (dx * dx + dy * dy))
            {
                Path.PushBack(new(X3, Y3));
            }
            else if (Level < 10)
            {
                float x12 = (X1 + X2) * 0.5f, y12 = (Y1 + Y2) * 0.5f;
                float x23 = (X2 + X3) * 0.5f, y23 = (Y2 + Y3) * 0.5f;
                float x123 = (x12 + x23) * 0.5f, y123 = (y12 + y23) * 0.5f;
                PathBezierQuadraticCurveToCasteljau(Path, X1, Y1, x12, y12, x123, y123, TessTol, Level + 1);
                PathBezierQuadraticCurveToCasteljau(Path, x123, y123, x23, y23, X3, Y3, TessTol, Level + 1);
            }
        }
        void ShadeVertsLinearUV(ImmuiDrawList DrawList, int VertStartIndex, int VertEndIndex, Vector2 A, Vector2 B, Vector2 UVA, Vector2 UVB, bool Clamp)
        {
            Vector2 size = B - A;
            Vector2 uv_size = UVB - UVA;
            Vector2 scale = new(
            size.X != 0.0f ? uv_size.X / size.X : 0.0f,
            size.Y != 0.0f ? uv_size.Y / size.Y : 0.0f);

            if (Clamp)
            {
                Vector2 min = Min(UVA, UVB);
                Vector2 max = Max(UVA, UVB);
                for (int i = VertStartIndex; i < VertEndIndex; i++)
                {
                    Vertex Vert = DrawList.VertexBuffer[i];
                    Vert.UV = ImmuiDrawList.Clamp(UVA + new Vector2(Vert.Position.X, Vert.Position.Y) - A * scale, min, max);
                    DrawList.VertexBuffer[i] = Vert;
                }
            }
            else
            {
                for (int i = VertStartIndex; i < VertEndIndex; i++)
                {
                    Vertex Vert = DrawList.VertexBuffer[i];
                    Vert.UV = UVA + new Vector2(Vert.Position.X, Vert.Position.Y) - A * scale;
                    DrawList.VertexBuffer[i] = Vert;

                }
            }
        }
        private static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(MathHelper.Min(a.X, b.X), MathHelper.Min(a.Y, b.Y));
        }

        private static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(MathHelper.Max(a.X, b.X), MathHelper.Max(a.Y, b.Y));
        }
        private static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            return new Vector2(
                MathHelper.Clamp(value.X, min.X, max.X),
                MathHelper.Clamp(value.Y, min.Y, max.Y)
            );
        }
        private static bool DrawCmdAreSequentialIdxOffset(ImmuiDrawCommand L, ImmuiDrawCommand R)
        {
            return L.Offset + L.Count == R.Offset;
        }
    }
}
