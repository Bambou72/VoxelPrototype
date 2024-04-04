using ImGuiNET;
using Newtonsoft.Json;
using VBF;
using VoxelPrototype.common.Game.World;

namespace VBFViewer
{
    internal static class Viewer
    {
        static RegionFile LoadedRegion;
        static VBFCompound[] Chunks;
        internal static void RenderVBFList(VBFList list,int Index)
        {
            if (list.ListType== VBFTag.DataType.Int)
            {
                ImGui.Text("Value : ");
                ImGui.SameLine();
                foreach (var tag in list.Tags)
                {
                    VBFInt vBFInt = (VBFInt)tag;
                    ImGui.Text(vBFInt.Value.ToString());
                }

            }
            if (list.ListType == VBFTag.DataType.Float)
            {
                ImGui.Text("Value : ");
                ImGui.SameLine();
                foreach (var tag in list.Tags)
                {
                    VBFFloat vBFFloat = (VBFFloat)tag;
                    ImGui.Text(vBFFloat.Value.ToString());
                }

            }
            if (list.ListType == VBFTag.DataType.String)
            {
                ImGui.Text("Value : ");
                ImGui.SameLine();

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
                foreach(var tag in  list.Tags)
                {
                    RenderVBF((VBFCompound)tag, Index);
                }
            }
        }

        internal static void RenderVBF(VBFCompound Comp,int Index)
        {
            foreach (var Tag in Comp.Tags)
            {
                if (ImGui.TreeNodeEx(Tag.Key+$"##{Index++}"))
                {

                    if (Tag.Value.Type == VBFTag.DataType.Int)
                    {
                        ImGui.Text("Value : ");
                        ImGui.SameLine();

                        VBFInt vBFInt = (VBFInt)Tag.Value;
                        ImGui.Text(vBFInt.Value.ToString());

                    }
                    if (Tag.Value.Type == VBFTag.DataType.Float)
                    {
                        ImGui.Text("Value : ");
                        ImGui.SameLine();

                        VBFFloat vBFFloat = (VBFFloat)Tag.Value;
                        ImGui.Text(vBFFloat.Value.ToString());

                    }
                    if (Tag.Value.Type == VBFTag.DataType.String)
                    {
                        ImGui.Text("Value : ");
                        ImGui.SameLine();


                        VBFString vBFString = (VBFString)Tag.Value;
                        ImGui.Text(vBFString.Value.ToString());

                    }
                    if (Tag.Value.Type == VBFTag.DataType.Bool)
                    {
                        ImGui.Text("Value : ");
                        ImGui.SameLine();


                        VBFBool vBFBool = (VBFBool)Tag.Value;
                        ImGui.Text(vBFBool.Value.ToString());

                    }
                    if (Tag.Value.Type == VBFTag.DataType.List)
                    {
                        RenderVBFList((VBFList)Tag.Value,Index);
                    }
                    if (Tag.Value.Type == VBFTag.DataType.Compound)
                    {

                        RenderVBF((VBFCompound)Tag.Value,Index);
                    }
                    ImGui.TreePop();
                }
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
                            if(LoadedRegion != null)
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
                ImGui.EndMainMenuBar();
            }
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(600, 600), ImGuiCond.Always);
            ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoTitleBar;

            // Création de la fenêtre ImGui
            ImGui.SetNextWindowPos(new System.Numerics.Vector2(0, ImGui.GetFrameHeightWithSpacing()), ImGuiCond.Always);
            if (ImGui.Begin("Main", windowFlags))
            {
                if(LoadedRegion != null)
                {
                    // Contenu de la fenêtre ImGui
                    if (ImGui.TreeNodeEx("Chunks"))
                    {
                        foreach (var chunk in Chunks)
                        {
                            var temp= new VBFCompound();
                            temp.Add(chunk.GetInt("PosX").Value.ToString() + ":" + chunk.GetInt("PosZ").Value.ToString(), chunk);
                            RenderVBF( temp,0);
                        }
                        ImGui.TreePop();
                    }


                }

                ImGui.End();
            }

        }
    }
}
