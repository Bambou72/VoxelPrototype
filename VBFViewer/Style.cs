using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VBFViewer
{
    internal static class Style
    {
        public static void SetupImGuiStyle()
        {
            // Fork of Microsoft style from ImThemes
            var style = ImGuiNET.ImGui.GetStyle();

            style.Alpha = 1.0f;
            style.DisabledAlpha = 0.6000000238418579f;
            style.WindowPadding = new Vector2(4.0f, 6.0f);
            style.WindowRounding = 0.0f;
            style.WindowBorderSize = 0.0f;
            style.WindowMinSize = new Vector2(32.0f, 32.0f);
            style.WindowTitleAlign = new Vector2(0.0f, 0.5f);
            style.WindowMenuButtonPosition = ImGuiDir.Left;
            style.ChildRounding = 0.0f;
            style.ChildBorderSize = 1.0f;
            style.PopupRounding = 0.0f;
            style.PopupBorderSize = 1.0f;
            style.FramePadding = new Vector2(8.0f, 6.0f);
            style.FrameRounding = 0.0f;
            style.FrameBorderSize = 1.0f;
            style.ItemSpacing = new Vector2(8.0f, 6.0f);
            style.ItemInnerSpacing = new Vector2(8.0f, 6.0f);
            style.CellPadding = new Vector2(4.0f, 2.0f);
            style.IndentSpacing = 20.0f;
            style.ColumnsMinSpacing = 6.0f;
            style.ScrollbarSize = 20.0f;
            style.ScrollbarRounding = 0.0f;
            style.GrabMinSize = 5.0f;
            style.GrabRounding = 0.0f;
            style.TabRounding = 4.0f;
            style.TabBorderSize = 0.0f;
            style.TabMinWidthForCloseButton = 0.0f;
            style.ColorButtonPosition = ImGuiDir.Right;
            style.ButtonTextAlign = new Vector2(0.5f, 0.5f);
            style.SelectableTextAlign = new Vector2(0.0f, 0.0f);

            style.Colors[(int)ImGuiCol.Text] = new Vector4(0.09803921729326248f, 0.09803921729326248f, 0.09803921729326248f, 1.0f);
            style.Colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.4980392158031464f, 0.4980392158031464f, 0.4980392158031464f, 1.0f);
            style.Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.9490196108818054f, 0.9490196108818054f, 0.9490196108818054f, 1.0f);
            style.Colors[(int)ImGuiCol.ChildBg] = new Vector4(0.9490196108818054f, 0.9490196108818054f, 0.9490196108818054f, 1.0f);
            style.Colors[(int)ImGuiCol.PopupBg] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.Border] = new Vector4(0.6000000238418579f, 0.6000000238418579f, 0.6000000238418579f, 1.0f);
            style.Colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            style.Colors[(int)ImGuiCol.FrameBg] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
            style.Colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.6560444831848145f, 0.6707022190093994f, 0.6824034452438354f, 0.2000000029802322f);
            style.Colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.468050628900528f, 0.4703408479690552f, 0.4721029996871948f, 1.0f);
            style.Colors[(int)ImGuiCol.TitleBg] = new Vector4(0.03921568766236305f, 0.03921568766236305f, 0.03921568766236305f, 1.0f);
            style.Colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.4806818962097168f, 0.4806838631629944f, 0.4806867241859436f, 1.0f);
            style.Colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.0f, 0.0f, 0.0f, 0.5099999904632568f);
            style.Colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.8588235378265381f, 0.8588235378265381f, 0.8588235378265381f, 1.0f);
            style.Colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.8588235378265381f, 0.8588235378265381f, 0.8588235378265381f, 1.0f);
            style.Colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.686274528503418f, 0.686274528503418f, 0.686274528503418f, 1.0f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.0f, 0.0f, 0.0f, 0.2000000029802322f);
            style.Colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);
            style.Colors[(int)ImGuiCol.CheckMark] = new Vector4(0.09803921729326248f, 0.09803921729326248f, 0.09803921729326248f, 1.0f);
            style.Colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.686274528503418f, 0.686274528503418f, 0.686274528503418f, 1.0f);
            style.Colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.0f, 0.0f, 0.0f, 0.5f);
            style.Colors[(int)ImGuiCol.Button] = new Vector4(0.8588235378265381f, 0.8588235378265381f, 0.8588235378265381f, 1.0f);
            style.Colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.7725244760513306f, 0.7725288271903992f, 0.7725322246551514f, 0.2000000029802322f);
            style.Colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.6652293801307678f, 0.6652330756187439f, 0.6652360558509827f, 1.0f);
            style.Colors[(int)ImGuiCol.Header] = new Vector4(0.8588235378265381f, 0.8588235378265381f, 0.8588235378265381f, 1.0f);
            style.Colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.6652293801307678f, 0.6652331352233887f, 0.6652360558509827f, 0.2000000029802322f);
            style.Colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.3776786029338837f, 0.377680778503418f, 0.3776823878288269f, 1.0f);
            style.Colors[(int)ImGuiCol.Separator] = new Vector4(0.4274509847164154f, 0.4274509847164154f, 0.4980392158031464f, 0.5f);
            style.Colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.5364753007888794f, 0.5364778637886047f, 0.5364806652069092f, 0.7799999713897705f);
            style.Colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.3948458135128021f, 0.3948476016521454f, 0.3948497772216797f, 1.0f);
            style.Colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.8583605289459229f, 0.8583645224571228f, 0.8583691120147705f, 0.2000000029802322f);
            style.Colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.6523540019989014f, 0.6523569822311401f, 0.6523605585098267f, 0.6700000166893005f);
            style.Colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.4849736988544464f, 0.4849759340286255f, 0.4849785566329956f, 0.949999988079071f);
            style.Colors[(int)ImGuiCol.Tab] = new Vector4(0.8326096534729004f, 0.8326134085655212f, 0.8326179981231689f, 0.8619999885559082f);
            style.Colors[(int)ImGuiCol.TabHovered] = new Vector4(0.785399854183197f, 0.7854037284851074f, 0.7854077219963074f, 0.800000011920929f);
            style.Colors[(int)ImGuiCol.TabActive] = new Vector4(0.5407671332359314f, 0.5407696962356567f, 0.540772557258606f, 1.0f);
            style.Colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.06666667014360428f, 0.1019607856869698f, 0.1450980454683304f, 0.9724000096321106f);
            style.Colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.5751015543937683f, 0.5751041769981384f, 0.5751073360443115f, 1.0f);
            style.Colors[(int)ImGuiCol.PlotLines] = new Vector4(0.6078431606292725f, 0.6078431606292725f, 0.6078431606292725f, 1.0f);
            style.Colors[(int)ImGuiCol.PlotLinesHovered] = new Vector4(1.0f, 0.4274509847164154f, 0.3490196168422699f, 1.0f);
            style.Colors[(int)ImGuiCol.PlotHistogram] = new Vector4(0.6394786238670349f, 0.6394850015640259f, 0.6394792199134827f, 1.0f);
            style.Colors[(int)ImGuiCol.PlotHistogramHovered] = new Vector4(0.2918455004692078f, 0.2918443977832794f, 0.2918425798416138f, 1.0f);
            style.Colors[(int)ImGuiCol.TableHeaderBg] = new Vector4(0.1882352977991104f, 0.1882352977991104f, 0.2000000029802322f, 1.0f);
            style.Colors[(int)ImGuiCol.TableBorderStrong] = new Vector4(0.3098039329051971f, 0.3098039329051971f, 0.3490196168422699f, 1.0f);
            style.Colors[(int)ImGuiCol.TableBorderLight] = new Vector4(0.2274509817361832f, 0.2274509817361832f, 0.2470588237047195f, 1.0f);
            style.Colors[(int)ImGuiCol.TableRowBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
            style.Colors[(int)ImGuiCol.TableRowBgAlt] = new Vector4(1.0f, 1.0f, 1.0f, 0.05999999865889549f);
            style.Colors[(int)ImGuiCol.TextSelectedBg] = new Vector4(0.3991376161575317f, 0.3991394340991974f, 0.3991416096687317f, 0.3499999940395355f);
            style.Colors[(int)ImGuiCol.DragDropTarget] = new Vector4(1.0f, 1.0f, 0.0f, 0.8999999761581421f);
            style.Colors[(int)ImGuiCol.NavHighlight] = new Vector4(0.5407671332359314f, 0.5407695770263672f, 0.540772557258606f, 1.0f);
            style.Colors[(int)ImGuiCol.NavWindowingHighlight] = new Vector4(1.0f, 1.0f, 1.0f, 0.699999988079071f);
            style.Colors[(int)ImGuiCol.NavWindowingDimBg] = new Vector4(0.800000011920929f, 0.800000011920929f, 0.800000011920929f, 0.2000000029802322f);
            style.Colors[(int)ImGuiCol.ModalWindowDimBg] = new Vector4(0.800000011920929f, 0.800000011920929f, 0.800000011920929f, 0.3499999940395355f);
        }
    }
}
