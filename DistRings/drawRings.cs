using System;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Interface;
using ImGuiNET;
using System.Numerics;
using Dalamud.Game.ClientState.Objects.Types;
using Lumina.Excel.GeneratedSheets;

namespace DistRings {
    public partial class Plugin {
        const float TAU = 6.2831855f;
        private void drawRing(GameObject me,Ring r) {
            if (r.style==0){//solid
                int segs = (int)(r.radii * 20f);
                if (segs > 100) { segs = 100; }
                if (segs < 3) { segs = 3; }
                float segAng = TAU/segs;
                for(int i = 0; i < segs; i++) {
                    gui_.WorldToScreen(new Vector3(me.Position.X + r.radii*(float)Math.Sin(segAng*i), me.Position.Y, me.Position.Z + r.radii * (float)Math.Cos(segAng*i)), out var pos);
                    ImGui.GetWindowDrawList().PathLineTo(pos);
                }
                ImGui.GetWindowDrawList().PathStroke(ImGui.GetColorU32(r.color), ImDrawFlags.Closed, r.thickness);
            }else if (r.style == 1) {//dotted
                int segs = (int)(r.radii * 2*TAU)+1;//dot every 0.5 yalm
                if (segs < 5) { segs = 5; }
                float segAng = TAU / segs;
                for (int i = 0; i < segs; i++) {
                    gui_.WorldToScreen(new Vector3(me.Position.X + r.radii*(float)Math.Sin(segAng*i), me.Position.Y, me.Position.Z + r.radii*(float)Math.Cos(segAng*i)), out var pos);
                    ImGui.GetWindowDrawList().AddCircleFilled(pos, r.thickness, ImGui.GetColorU32(r.color));
                }
             }else if (r.style == 2) {//dashed
                int segs = (int)(r.radii * 2*TAU)+1;//dash every 0.5 yalm
                if (segs < 5) { segs = 5; }
                float segAng = TAU / segs;
                for (int i = 0; i < segs; i++) {
                    gui_.WorldToScreen(new Vector3(me.Position.X + r.radii*(float)Math.Sin(segAng*i), me.Position.Y, me.Position.Z + r.radii*(float)Math.Cos(segAng*i)), out var pos1);
                    gui_.WorldToScreen(new Vector3(me.Position.X + r.radii*(float)Math.Sin(segAng*(i+0.3f)), me.Position.Y, me.Position.Z + r.radii*(float)Math.Cos(segAng*(i+0.3f))), out var pos2);
                    ImGui.GetWindowDrawList().AddLine(pos1, pos2, ImGui.GetColorU32(r.color), r.thickness);
                }
             }else if (r.style == 3) {//spaced
                int segs = (int)(r.radii * 4)+1;//dot every ~1.5 yalm
                if (segs < 5) { segs = 5; }
                float segAng = TAU / segs;
                for (int i = 0; i < segs; i++) {
                    gui_.WorldToScreen(new Vector3(me.Position.X + r.radii*(float)Math.Sin(segAng*i), me.Position.Y, me.Position.Z + r.radii*(float)Math.Cos(segAng*i)), out var pos1);
                    gui_.WorldToScreen(new Vector3(me.Position.X + r.radii*(float)Math.Sin(segAng*(i+0.3f)), me.Position.Y, me.Position.Z + r.radii*(float)Math.Cos(segAng*(i+0.3f))), out var pos2);
                    ImGui.GetWindowDrawList().AddLine(pos1, pos2, ImGui.GetColorU32(r.color), r.thickness);
                }
             }
        }
        private void drawDot(uint col) {
            if (CS.LocalPlayer == null) return;
            var me = CS.LocalPlayer;
            gui_.WorldToScreen( new Vector3(me.Position.X, me.Position.Y, me.Position.Z), out var pos);
            ImGui.GetWindowDrawList().AddCircleFilled(pos, 2.5f, col);
        }

        private void doDraw() {//does the overlay draws
            if (CS.LocalPlayer == null) return;//cant draw if we dont know where we are
            ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(0, 0));
            ImGuiHelpers.ForceNextWindowMainViewport();
            ImGuiHelpers.SetNextWindowPosRelativeMainViewport(new Vector2(0, 0));
            ImGui.Begin("Canvas",
                ImGuiWindowFlags.NoInputs | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.NoTitleBar |
                ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoBackground);
            ImGui.SetWindowSize(ImGui.GetIO().DisplaySize);
            if (Configuration.DotEnabled) {
                drawDot(ImGui.GetColorU32(Configuration.DotCol));
            }
            if (Configuration.RingsEnabled) {
                foreach (Ring r in Configuration.ringList) {
                    if(r.job== CS.LocalPlayer.ClassJob.Id)
                        drawRing(CS.LocalPlayer, r);
                }
            }
            ImGui.End();
            ImGui.PopStyleVar();
        }
    }
}
