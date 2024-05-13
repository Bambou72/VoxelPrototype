using ImGuiNET;
using System.Numerics;
using VoxelPrototype.API;
using VoxelPrototype.API.Commands;
namespace VoxelPrototype.client.GUI
{
    internal static class Console
    {
        //console
        static List<string> consoleHistory = new List<string>();
        static int maxHistorySize = 100;
        static string inputText = "";
        static bool showAutoComplete = false;
        static List<string> suggestions = new();
        /// <summary>
        /// Console menu
        /// </summary>
        internal static void ConsoleDraw()
        {
            ImGuiWindowFlags ConsoleFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize;
            if (showAutoComplete)
            {
                ConsoleFlags |= ImGuiWindowFlags.NoBringToFrontOnFocus;
            }
            ImGui.SetNextWindowPos(new Vector2(10, ClientAPI.WindowHeight() - 350));
            ImGui.SetNextWindowSize(new Vector2(600, 300));
            ImGui.Begin("Chat", ConsoleFlags);
            {
                ImGui.BeginChild("ConsoleScroll", new Vector2(0, -ImGui.GetTextLineHeightWithSpacing() - 10));
                foreach (var message in consoleHistory)
                {
                    if (message.StartsWith("#"))
                    {
                        ImGui.TextColored(new Vector4(0.8f, 0.8f, 0.0f, 1.0f), message);
                    }
                    else if (message.StartsWith("server:"))
                    {
                        ImGui.TextColored(new Vector4(1f, 0f, 0.0f, 1.0f), message);
                    }
                    else
                    {
                        ImGui.Text(message);
                    }
                }
                ImGui.EndChild();
                ImGuiInputTextFlags flags = ImGuiInputTextFlags.EnterReturnsTrue;
                ImGui.SetNextItemWidth(485);
                ImGui.InputText("##input", ref inputText, 100, flags) ;
                
                GUIVar.InInputText = ImGui.IsItemActive();
                if (ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows) && !ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked(0))
                {
                    ImGui.SetKeyboardFocusHere(-1);
                }
                ImGui.SameLine();
                if (ImGui.Button("Enter"))
                {
                    ImGui.SetKeyboardFocusHere(-1);

                    ClientChat.SendMessage(inputText);
                    if (inputText.Length > 0 && inputText[0] == CommandRegister.commandPrefix)
                    {
                        AddToConsoleHistory(inputText);
                    }
                    inputText = "";
                    showAutoComplete = false;

                }
                Vector2 inputTextPos = ImGui.GetItemRectMin();
                ShowAutoCompletion();
                ImGui.End();
                if (showAutoComplete)
                {
                    float autocompleteWidth = suggestions.Count > 0 ? ImGui.CalcTextSize(suggestions[0]).X * 3 : 0;
                    ImGui.SetNextWindowPos(new Vector2(inputTextPos.X, inputTextPos.Y - ImGui.GetTextLineHeight() * suggestions.Count * 3));
                    ImGui.SetNextWindowSize(new Vector2(autocompleteWidth, ImGui.GetTextLineHeight() * suggestions.Count));
                    ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0);
                    ImGui.Begin("AutoCompletion", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.NoSavedSettings);
                    foreach (string suggestion in suggestions)
                    {
                        if (ImGui.Selectable(suggestion))
                        {
                            //ImGui.ClearActiveID();
                            inputText = suggestion;
                            showAutoComplete = false;
                        }
                    }
                    ImGui.End();
                    ImGui.PopStyleVar(1);
                }
            }
        }
        internal static void AddToConsoleHistory(string message)
        {
            consoleHistory.Add(message);
            // Maintain the history size
            if (consoleHistory.Count > maxHistorySize)
            {
                consoleHistory.RemoveAt(0);
            }
        }
        static void ShowAutoCompletion()
        {
            if (string.IsNullOrEmpty(inputText))
            {
                return;
            }
            suggestions = CommandRegister.autocommands.Where(cmd => cmd.StartsWith(inputText)).ToList();
            if (suggestions.Count > 0)
            {
                showAutoComplete = true;
            }
            else
            {
                showAutoComplete = false;
            }
        }
    }
}
