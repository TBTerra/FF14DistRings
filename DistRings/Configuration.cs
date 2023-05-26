using Dalamud.Configuration;
using Dalamud.Plugin;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace DistRings
{
    public class Ring
    {
        public float radii;
        public float thickness;
        public Vector4 color;
        public int style;
        public int job;
        public Ring(float R, float T, Vector4 C, int s, int j) {
            radii = R;
            thickness = T;
            color = C;
            style = s;
            job = j;
        }
    }
    [Serializable]
    public class Configuration : IPluginConfiguration {
        public int Version { get; set; } = 0;

        public bool RingsEnabled { get; set; } = true;
        public bool DotEnabled { get; set; } = true;
        public Vector4 DotCol { get; set; } = new (1f,1f,1f,1f);//white, 100% opacity
        public List<Ring> ringList { get; set; } = new();
        public bool listAll { get; set; } = true;

        // the below exist just to make saving less cumbersome
        [NonSerialized]
        private DalamudPluginInterface? PluginInterface;

        public void Initialize(DalamudPluginInterface pluginInterface) {
            this.PluginInterface = pluginInterface;
        }

        public void Save() {
            this.PluginInterface!.SavePluginConfig(this);
        }
    }
}
