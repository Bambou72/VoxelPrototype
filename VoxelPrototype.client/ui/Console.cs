using ImGuiNET;
using System.Numerics;
using VoxelPrototype.api.command;
using VoxelPrototype.client.game;

namespace VoxelPrototype.client.ui
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
            //ImGui.PushStyleColor(ImGuiCol.WindowBg, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0)));
            //ImGui.PushStyleColor(ImGuiCol.Border, ImGui.ColorConvertFloat4ToU32(new System.Numerics.Vector4(0)));
            ImGuiWindowFlags ConsoleFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize;
            if (showAutoComplete)
            {
                ConsoleFlags |= ImGuiWindowFlags.NoBringToFrontOnFocus;
            }
            ImGui.SetNextWindowPos(new Vector2(10, Client.TheClient.ClientSize.Y - 350));
            ImGui.SetNextWindowSize(new Vector2(500, 300));
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
                ImGui.SetNextItemWidth(425);
                ImGui.InputText("##input", ref inputText, 100, flags);

                GUIManager.InInputText = ImGui.IsItemActive();
                if (ImGui.IsWindowFocused(ImGuiFocusedFlags.RootAndChildWindows) && !ImGui.IsAnyItemActive() && !ImGui.IsMouseClicked(0))
                {
                    ImGui.SetKeyboardFocusHere(-1);
                }
                ImGui.SameLine();
                if (ImGui.Button("Enter"))
                {
                    ImGui.SetKeyboardFocusHere(-1);

                    Client.TheClient.World.Chat.SendMessage(inputText);
                    if (inputText.Length > 0 && inputText[0] == CommandRegistry.GetInstance().commandPrefix)
                    {
                        AddToConsoleHistory(inputText);
                    }
                    inputText = "";
                    showAutoComplete = false;

                }
                ShowAutoCompletion();
                ImGui.End();
                if (showAutoComplete)
                {
                    float autocompleteWidth = CalculateAutoWidth();
                    ImGui.SetNextWindowPos(new(17, Client.TheClient.ClientSize.Y - 185));
                    ImGui.SetNextWindowSize(new Vector2(autocompleteWidth, ImGui.GetTextLineHeight() * suggestions.Count * 1.5f));
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
            //ImGui.PopStyleColor(2);
        }
        internal static int CalculateAutoWidth()
        {
            if(suggestions.Count == 0)
            {
                return 0;
            }
            int MaxIndex = 0;
            int CurrentMax = 0;
            for(int i = 0; i < suggestions.Count;i++)
            {
                if (suggestions[i].Length > CurrentMax)
                {
                    CurrentMax = suggestions[i].Length;
                    MaxIndex = i;
                }
            }
            return (int)ImGui.CalcTextSize(suggestions[MaxIndex]).X *2;
            //suggestions.Count > 0 ? ImGui.CalcTextSize(suggestions[0]).X * 3 : 0
        }
        internal static void ClearConsoleHistory()
        {
            consoleHistory.Clear();
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
            suggestions = CommandRegistry.GetInstance().autocommands.Where(cmd => cmd.StartsWith(inputText)).ToList();
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
