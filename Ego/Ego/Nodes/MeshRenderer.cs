﻿using ImGuiNET;
using Rendering;

namespace Ego;

public class MeshRenderer : Node3D
{
    private MeshCollection Meshes;

    private int MeshIndex = 0;
    
    public MeshRenderer()
    {
        Meshes = Program.Context.AssetManager.GetAsset<MeshCollection>("Models/basicmesh.glb");
        
        Program.Context.RendererApi.ERender += ERender;
    }

    public override void Inspect()
    {
        ImGui.SliderInt("Mesh Index", ref MeshIndex, 0, Meshes.Meshes.Count - 1);
    }

    private void ERender(List<MeshRenderData> aRenderData)
    {
        aRenderData.Add(new() { Mesh = Meshes.Meshes[MeshIndex], WorldMatrix = WorldMatrix });
    }
}