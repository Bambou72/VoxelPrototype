using ImGuiNET;
using ModelEditor.ModelEditor;
using ModelEditor.ModelEditor.Block;
using System.Numerics;
namespace ModelEditor
{
    internal static class ModelEditorGUI
    {
        static List<Face> faces = new List<Face>();
        static int listBoxSelectedItem = 0;
#pragma warning disable CS0414 // Le champ 'ModelEditorGUI.TextureSelectedItem' est assigné, mais sa valeur n'est jamais utilisée
        static int TextureSelectedItem = 0;
#pragma warning restore CS0414 // Le champ 'ModelEditorGUI.TextureSelectedItem' est assigné, mais sa valeur n'est jamais utilisée
        static uint IndexCounter = 0;
        static bool showFilePicker = false;
        static string selectedFilePath = "";
        static bool showFilePickerSave = false;
        static string selectedFilePathSave = "";
        static string SaveName = "";
        static bool showFilePickerSaveTextures = false;
        static string selectedFilePathSaveTextures = "";
        static string SaveNameTextures = "";
        static string SaveNameModel = "";
        internal static void Render()
        {
            ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
            ImGui.BeginMainMenuBar();
            if (ImGui.BeginMainMenuBar())
            {
                if (ImGui.BeginMenu("File"))
                {
                    if (ImGui.MenuItem("Open", "Ctrl+O"))
                    {
                        showFilePicker = true;
                    }
                    if (ImGui.MenuItem("Save", "Ctrl+S"))
                    {
                        showFilePickerSave = true;
                    }
                    if (ImGui.MenuItem("Save Texture File", "Ctrl+S"))
                    {
                        showFilePickerSaveTextures = true;
                    }
                    ImGui.EndMenu();
                }
                // Autres éléments de menu ou onglets peuvent être ajoutés ici
                ImGui.EndMainMenuBar();
            }
            if (showFilePicker)
            {
                ImGui.OpenPopup("FilePicker");
            }
            if (ImGui.BeginPopup("FilePicker"))
            {
                // Affichez une liste de fichiers ou dossiers disponibles ici
                // Vous pouvez utiliser ImGui.Text pour afficher chaque fichier/dossier
                ImGui.InputText("FilePath", ref selectedFilePath, 100);
                if (ImGui.Button("Select"))
                {
                    faces = BlockDeserializer.Deserialize(selectedFilePath);
                    showFilePicker = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    showFilePicker = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
            if (showFilePickerSave)
            {
                ImGui.OpenPopup("FilePickerSave");
            }
            if (ImGui.BeginPopup("FilePickerSave"))
            {
                // Affichez une liste de fichiers ou dossiers disponibles ici
                // Vous pouvez utiliser ImGui.Text pour afficher chaque fichier/dossier
                ImGui.InputText("FilePath", ref selectedFilePathSave, 100);
                ImGui.InputText("Save", ref SaveName, 100);
                if (ImGui.Button("Select"))
                {
                    BlockDeserializer.Serialize(faces, SaveName, selectedFilePathSave);
                    showFilePickerSave = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    showFilePickerSave = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
            if (showFilePickerSaveTextures)
            {
                ImGui.OpenPopup("FilePickerSaveTextures");
            }
            if (ImGui.BeginPopup("FilePickerSaveTextures"))
            {
                // Affichez une liste de fichiers ou dossiers disponibles ici
                // Vous pouvez utiliser ImGui.Text pour afficher chaque fichier/dossier
                ImGui.InputText("FilePath", ref selectedFilePathSaveTextures, 100);
                ImGui.InputText("Save", ref SaveNameTextures, 100);
                ImGui.InputText("Model Name", ref SaveNameModel, 100);
                if (ImGui.Button("Select"))
                {
                    BlockDeserializer.SerializeTextures(faces, SaveNameTextures, SaveNameModel, selectedFilePathSaveTextures);
                    showFilePickerSaveTextures = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.SameLine();
                if (ImGui.Button("Cancel"))
                {
                    showFilePickerSaveTextures = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
            if (ImGui.Begin("Info"))
            {
                ImGui.Text("Target Position:" + ModelRenderer.Camera.Target.X + ";" + ModelRenderer.Camera.Target.Z);
                ImGui.Separator();
                ImGui.Text("Faces number:" + faces.Count);
                ImGui.Text("Vertex number:" + faces.Count * 4);
                ImGui.End();
            }
            ImGui.Begin("ModelEditorMenu");
            (float[] vertices, uint[] indices) = GetVertices();
            ModelRenderer.BlockMesh = vertices;
            ModelRenderer.BlockIndices = indices;
            ModelRenderer.Render();
            ImGui.Image(ModelRenderer.texture, new Vector2(1280, 720));
            ImGui.End();
            ImGui.Begin("Vertices");
            if (ImGui.Button("Add Face"))
            {
                faces.Add(new Face());
            }
            if (ImGui.Button("Remove Face"))
            {
                if (faces.Count > 0)
                {
                    faces.RemoveAt(listBoxSelectedItem);
                    listBoxSelectedItem = 0;
                }
            }
            ImGui.ListBox("Liste", ref listBoxSelectedItem, GetItemIndices(faces), faces.Count);
            ImGui.End();
            ImGui.Begin("Vertices Editor");
            if (faces.Count > 0)
            {
                ImGui.InputFloat3("Vertex 1", ref faces[listBoxSelectedItem].data[0]);
                ImGui.InputFloat3("Vertex 2", ref faces[listBoxSelectedItem].data[1]);
                ImGui.InputFloat3("Vertex 3", ref faces[listBoxSelectedItem].data[2]);
                ImGui.InputFloat3("Vertex 4", ref faces[listBoxSelectedItem].data[3]);
                ImGui.InputText("Texture", ref faces[listBoxSelectedItem].texture, 100);
                ImGui.InputFloat2("Uv 1", ref faces[listBoxSelectedItem].uv[0]);
                ImGui.InputFloat2("Uv 2", ref faces[listBoxSelectedItem].uv[1]);
                ImGui.InputFloat2("Uv 3", ref faces[listBoxSelectedItem].uv[2]);
                ImGui.InputFloat2("Uv 4", ref faces[listBoxSelectedItem].uv[3]);
            }
            ImGui.End();
            ImGui.Begin("Textures");
            if (ImGui.Button("Reload Textures"))
            {
                TextureManager.LoadTextures();
            }
            ImGui.Image(TextureManager.VoxelAtlas.Handle, new Vector2(256, 256));
            ImGui.End();
        }
        static string[] GetItemIndices(List<Face> items)
        {
            string[] indices = new string[items.Count];
            for (int i = 0; i < items.Count; i++)
            {
                indices[i] = i.ToString();
            }
            return indices;
        }
        static (float[], uint[]) GetVertices()
        {
            List<float> vertices = new List<float>();
            List<uint> Indices = new List<uint>();
            foreach (Face face in faces)
            {
                float[] Tex = TextureManager.GetAtlasTextures(face.texture);
                for (int i = 0; i < 4; i++)
                {
                    vertices.Add(face.data[i].X);
                    vertices.Add(face.data[i].Y);
                    vertices.Add(face.data[i].Z);
                    vertices.Add((Tex[4] - Tex[0]) * face.uv[i].X + Tex[0]);
                    vertices.Add((Tex[3] - Tex[1]) * face.uv[i].Y + Tex[1]);
                    if (face == faces[listBoxSelectedItem])
                    {
                        vertices.Add(1);
                    }
                    else
                    {
                        vertices.Add(0);
                    }
                }
                uint[] indice = new uint[] { 0, 1, 2, 2, 3, 0 };
                for (uint i = 0; i < 6; i++)
                {
                    indice[i] += IndexCounter;
                }
                Indices.AddRange(indice);
                IndexCounter += 4;
            }
            IndexCounter = 0;
            return (vertices.ToArray(), Indices.ToArray());
        }
    }
}
