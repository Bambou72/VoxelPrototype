using ImGuiNET;
using VoxelPrototype.common;
using VoxelPrototype.common.World;

namespace VBFViewer
{
    internal static class Viewer
    {
        static RegionFile LoadedRegion;
        static VBFCompound[] Chunks;
        static long Index = 0;
        internal static void RenderVBFList(VBFList list)
        {
            if (list.ListType == VBFTag.DataType.Int)
            {
                ImGui.Text("Values : ");
                foreach (var tag in list.Tags)
                {
                    VBFInt vBFInt = (VBFInt)tag;
                    ImGui.Text(vBFInt.Value.ToString());
                }
            }
            if (list.ListType == VBFTag.DataType.Byte)
            {
                ImGui.Text("Values : ");
                foreach (var tag in list.Tags)
                {
                    VBFByte vBFByte = (VBFByte)tag;
                    ImGui.Text(vBFByte.Value.ToString());
                }
            }
            if (list.ListType == VBFTag.DataType.Long)
            {
                ImGui.Text("Values : ");
                foreach (var tag in list.Tags)
                {
                    VBFLong vBFLong = (VBFLong)tag;
                    ImGui.Text(vBFLong.Value.ToString());
                }
            }
            if (list.ListType == VBFTag.DataType.Double)
            {
                ImGui.Text("Values : ");
                foreach (var tag in list.Tags)
                {
                    VBFDouble vBFDouble = (VBFDouble)tag;
                    ImGui.Text(vBFDouble.Value.ToString());
                }
            }
            if (list.ListType == VBFTag.DataType.Float)
            {
                ImGui.Text("Values : ");
                foreach (var tag in list.Tags)
                {
                    VBFFloat vBFFloat = (VBFFloat)tag;
                    ImGui.Text(vBFFloat.Value.ToString());
                }
            }
            if (list.ListType == VBFTag.DataType.String)
            {
                ImGui.Text("Values : ");
                foreach (var tag in list.Tags)
                {

                    VBFString vBFString = (VBFString)tag;
                    ImGui.Text(vBFString.Value.ToString());
                }
            }
            if (list.ListType == VBFTag.DataType.Bool)
            {
                ImGui.Text("Values : ");
                foreach (var tag in list.Tags)
                {
                    VBFBool vBFBool = (VBFBool)tag;
                    ImGui.Text(vBFBool.Value.ToString());

                }
            }
            if (list.ListType == VBFTag.DataType.Compound)
            {
                long ListIndex = 0;
                foreach (var tag in list.Tags)
                {
                    RenderTag(ListIndex.ToString(), tag);
                    ListIndex++;
                }
            }
        }
        internal static void RenderTag(string Name, VBFTag Tag)
        {
            if (ImGui.TreeNodeEx(Name + $"##{Index++}"))
            {
                if (Tag.Type == VBFTag.DataType.Int)
                {
                    ImGui.Text("Value : ");
                    ImGui.SameLine();

                    VBFInt vBFInt = (VBFInt)Tag;
                    ImGui.Text(vBFInt.Value.ToString());

                }
                if (Tag.Type == VBFTag.DataType.Byte)
                {
                    ImGui.Text("Value : ");
                    ImGui.SameLine();

                    VBFByte vBFByte = (VBFByte)Tag;
                    ImGui.Text(vBFByte.Value.ToString());

                }
                if (Tag.Type == VBFTag.DataType.Long)
                {
                    ImGui.Text("Value : ");
                    ImGui.SameLine();

                    VBFLong vBFLong = (VBFLong)Tag;
                    ImGui.Text(vBFLong.Value.ToString());

                }
                if (Tag.Type == VBFTag.DataType.Double)
                {
                    ImGui.Text("Value : ");
                    ImGui.SameLine();

                    VBFDouble vBFDouble = (VBFDouble)Tag;
                    ImGui.Text(vBFDouble.Value.ToString());

                }
                if (Tag.Type == VBFTag.DataType.ByteArray)
                {
                    ImGui.Text("Values : ");

                    VBFByteArray vBFByteArray = (VBFByteArray)Tag;
                    foreach (byte b in vBFByteArray.Value)
                    {
                        ImGui.Text(b.ToString());
                    }
                }
                if (Tag.Type == VBFTag.DataType.IntArray)
                {
                    ImGui.Text("Values : ");
                    VBFIntArray vBFIntArray = (VBFIntArray)Tag;
                    foreach (int b in vBFIntArray.Value)
                    {
                        ImGui.Text(b.ToString());
                    }
                }
                if (Tag.Type == VBFTag.DataType.Float)
                {
                    ImGui.Text("Value : ");
                    ImGui.SameLine();

                    VBFFloat vBFFloat = (VBFFloat)Tag;
                    ImGui.Text(vBFFloat.Value.ToString());

                }
                if (Tag.Type == VBFTag.DataType.String)
                {
                    ImGui.Text("Value : ");
                    ImGui.SameLine();


                    VBFString vBFString = (VBFString)Tag;
                    ImGui.Text(vBFString.Value.ToString());

                }
                if (Tag.Type == VBFTag.DataType.Bool)
                {
                    ImGui.Text("Value : ");
                    ImGui.SameLine();


                    VBFBool vBFBool = (VBFBool)Tag;
                    ImGui.Text(vBFBool.Value.ToString());

                }
                if (Tag.Type == VBFTag.DataType.List)
                {
                    RenderVBFList((VBFList)Tag);
                }
                if (Tag.Type == VBFTag.DataType.Compound)
                {
                    VBFCompound VBFCompoundTag = (VBFCompound)Tag;
                    foreach (var CompTag in VBFCompoundTag.Tags)
                    {
                        RenderTag(CompTag.Key, CompTag.Value);
                    }
                }
                ImGui.TreePop();
            }
        }
        internal static void RenderGUI()
        {
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open Region"))
                    {
                        var Result = NativeFileDialogSharp.Dialog.FileOpen("vpr");
                        if (Result.IsOk)
                        {
                            if (LoadedRegion != null)
                            {
                                LoadedRegion.Close();
                            }
                            LoadedRegion = new RegionFile(Result.Path);
                            Chunks = LoadedRegion.ReadAllChunkAsVBF();

                        }
                    }
                    if (ImGui.MenuItem("Close Region"))
                    {
                        if (LoadedRegion != null)
                        {
                            LoadedRegion.Close();
                            LoadedRegion = null;
                            Chunks = null;
                        }
                    }
                    /*
                    if (ImGui.MenuItem("Save"))
                    {
                    }*/
                    if (ImGui.MenuItem("Exit"))
                    {
                        Program.window.Close(); // Quitter la boucle
                    }
                    ImGui.EndMenu();
                }
                ImGui.Separator();
                if (Chunks != null)
                {
                    ImGui.Text("Number of loaded chunks : " + Chunks.Length);
                }
                ImGui.EndMainMenuBar();
            }
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(Program.window.ClientSize.X, Program.window.ClientSize.Y), ImGuiCond.Always);
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar;

            // Création de la fenêtre ImGui
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, ImGui.GetFrameHeightWithSpacing()), ImGuiCond.Always);
            if (ImGui.Begin("Main", windowFlags))
            {
                if (LoadedRegion != null)
                {
                    Index = 0;
                    // Contenu de la fenêtre ImGui
                    if (ImGui.TreeNodeEx("Chunks"))
                    {
                        foreach (var chunk in Chunks)
                        {
                            RenderTag(chunk.GetInt("PosX").Value.ToString() + ":" + chunk.GetInt("PosZ").Value.ToString(), chunk);
                        }
                        ImGui.TreePop();
                    }


                }

                ImGui.End();
            }

        }
    }
}
