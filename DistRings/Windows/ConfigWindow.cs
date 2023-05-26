using System;
using System.Numerics;
using System.Text;
using Dalamud.Interface;
using Dalamud.Interface.Windowing;
using Dalamud.Game.ClientState;
using FFXIVClientStructs.FFXIV.Common.Configuration;
using ImGuiNET;
using Newtonsoft.Json;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;

namespace DistRings.Windows;

public class ConfigWindow : Window, IDisposable {
    private Configuration Configuration;
    private ClientState CState;

    private Ring tempR = new(5f,1f,new Vector4(1f,1f,1f,1f),0,0);
    static Dictionary<int, string> jobs = new Dictionary<int, string>() {
        {19,"PLD"},{21,"WAR"},{32,"DRK"},{37,"GNB"},
        {24,"WHM"},{28,"SCH"},{33,"AST"},{40,"SGE"},
        {20,"MNK"},{22,"DRG"},{30,"NIN"},{34,"SAM"},{39,"RPR"},
        {23,"BRD"},{31,"MCH"},{38,"DNC"},
        {25,"BLM"},{27,"SMN"},{35,"RDM"},{36,"BLU"},
        {0,"ERR"}
    };

    public ConfigWindow(Plugin plugin) : base(
        "Distance Rings Config",
        ImGuiWindowFlags.None) {
        this.Size = new Vector2(380, 150);
        this.SizeCondition = ImGuiCond.FirstUseEver;

        this.Configuration = plugin.Configuration;
        this.CState = plugin.CS;

    }

    public void Dispose() { this.Configuration.Save(); }//save pon window close?

    public override void Draw() {
        // can't ref a property, so use a local copy
        var ringsE = this.Configuration.RingsEnabled;
        if (ImGui.Checkbox("Enable Rings", ref ringsE)) {
            this.Configuration.RingsEnabled = ringsE;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }
        ImGui.SameLine();
        var dotE = this.Configuration.DotEnabled;
        if (ImGui.Checkbox("Enable hitbox Dot", ref dotE)) {
            this.Configuration.DotEnabled = dotE;
            // can save immediately on change, if you don't want to provide a "Save and Close" button
            this.Configuration.Save();
        }
        ImGui.SameLine();
        var dotC = this.Configuration.DotCol;
        if (ImGui.ColorEdit4("dotCol", ref dotC, ImGuiColorEditFlags.NoInputs|ImGuiColorEditFlags.NoLabel)) {
            this.Configuration.DotCol = dotC;
            this.Configuration.Save();
        }
        
        if (CState.LocalPlayer != null && jobs.ContainsKey((int)CState.LocalPlayer.ClassJob.Id)) {
            ImGui.SameLine();
            ImGui.LabelText("##classLab", $"Class:{jobs[(int)CState.LocalPlayer.ClassJob.Id]}");
        }
        int num = 0;
        foreach (var ring in this.Configuration.ringList) {
            if(!this.Configuration.listAll && (CState.LocalPlayer==null || (int)CState.LocalPlayer.ClassJob.Id != ring.job)) {
                num++;
                continue;
            }
            ImGui.PushItemWidth(70);
            ImGui.DragFloat($"##radius{num}", ref this.Configuration.ringList[num].radii, 0.1f, 0.1f, 50f, "R:%.1fy", ImGuiSliderFlags.AlwaysClamp); ImGui.SameLine();
            ImGui.SameLine();
            ImGui.PushItemWidth(70);
            ImGui.DragFloat($"##Thickness{num}", ref this.Configuration.ringList[num].thickness, 0.05f, 1f, 10f, "T:%.1fpx", ImGuiSliderFlags.AlwaysClamp); ImGui.SameLine();
            ImGui.SameLine();
            ImGui.ColorEdit4($"##ringCol{num}", ref this.Configuration.ringList[num].color, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel);
            ImGui.SameLine();
            ImGui.PushItemWidth(75);
            ImGui.Combo($"##style{num}", ref this.Configuration.ringList[num].style, "Solid\0Dotted\0Dashed\0Spaced\0\0");
            ImGui.SameLine();
            ImGui.PushFont(UiBuilder.IconFont);
            if (ImGui.Button($"{FontAwesomeIcon.Minus.ToIconString()}##{num}")) { this.Configuration.ringList.RemoveAt(num); this.Configuration.Save(); }
            ImGui.PopFont();
            ImGui.SameLine();
            ImGui.LabelText($"##classLab{num}", jobs[this.Configuration.ringList[num].job]);
            num++;
        }
        ImGui.Separator();
        ImGui.PushItemWidth(100);
        ImGui.DragFloat("##radius", ref tempR.radii, 0.1f, 0.1f, 50f, "Radius %.1fy", ImGuiSliderFlags.AlwaysClamp); ImGui.SameLine();
        ImGui.PushItemWidth(100);
        ImGui.DragFloat("##Thickness", ref tempR.thickness, 0.05f, 1f, 10f, "Thickness %.1fpx", ImGuiSliderFlags.AlwaysClamp); ImGui.SameLine();
        ImGui.ColorEdit4("##ringCol", ref tempR.color, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel); ImGui.SameLine();
        ImGui.PushItemWidth(75);
        ImGui.Combo("##style", ref tempR.style, "Solid\0Dotted\0Dashed\0Spaced\0\0");
        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont);
        if (ImGui.Button(FontAwesomeIcon.Plus.ToIconString()) && CState.LocalPlayer!=null) {
            this.Configuration.ringList.Add(new Ring(tempR.radii,tempR.thickness,tempR.color,tempR.style, (int)CState.LocalPlayer.ClassJob.Id));//for some reason theres no 'copy of' shortcut
            this.Configuration.Save();
        }
        ImGui.PopFont();
        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Add ring to this class"); }

        ImGui.PushFont(UiBuilder.IconFont); if (ImGui.Button(FontAwesomeIcon.Save.ToIconString())) { this.Configuration.Save(); } ImGui.PopFont();
        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Manual save settings(should autosave on close)"); }
        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont); if (ImGui.Button(FontAwesomeIcon.ArrowUpFromBracket.ToIconString())) {
            var json = JsonConvert.SerializeObject(this.Configuration.ringList);
            var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            ImGui.SetClipboardText(base64);
        } ImGui.PopFont();
        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Exports rings to clipboard for sharing"); }
        ImGui.SameLine();
        ImGui.PushFont(UiBuilder.IconFont); if (ImGui.Button(FontAwesomeIcon.ArrowsDownToLine.ToIconString())) { } ImGui.PopFont();
        if (ImGui.IsItemHovered()) { ImGui.SetTooltip("Imports rings from clipboard"); }

        ImGui.SameLine();
        var listAll = this.Configuration.listAll;
        if (ImGui.Checkbox("Show all jobs rings", ref listAll)) {
            this.Configuration.listAll = listAll;
            this.Configuration.Save();
        }
    }
}
