using System.Runtime.InteropServices;

namespace Rendering;
using Vortice.Vulkan;
using Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
struct PushConstants
{
    public Vector4 data1;
    public Vector4 data2;
    public Vector4 data3;
    public Vector4 data4;
}

public partial class Renderer
{
    private void DrawInternal()
    {
        myCurrentFrame.MyRenderFence.Wait();
        myCurrentFrame.MyRenderFence.Reset();

        myCurrentFrame.MyDeletionQueue.Flush();
        
        uint imageIndex = Device.AcquireNextImage(mySwapchain, myCurrentFrame.MyImageAvailableSemaphore);
        VkImage currentSwapchainImage = mySwapchain.MyImages[(int)imageIndex];

        CommandBuffer cmd = myCurrentFrame.MyCommandBuffer;

        cmd.Reset();
        cmd.BeginRecording();

        cmd.TransitionImage(myDrawImage, VkImageLayout.General);

        DrawBackground(cmd);

        cmd.TransitionImage(myDrawImage, VkImageLayout.ColorAttachmentOptimal);

        DrawGeometry(cmd);

        cmd.TransitionImage(myDrawImage, VkImageLayout.TransferSrcOptimal);
        cmd.TransitionImage(currentSwapchainImage, VkImageLayout.Undefined, VkImageLayout.TransferDstOptimal);

        cmd.Blit(myDrawImage, currentSwapchainImage, mySwapchain.MyExtents);
        
        cmd.TransitionImage(currentSwapchainImage, VkImageLayout.TransferDstOptimal, VkImageLayout.PresentSrcKHR);

        cmd.EndRecording();

        myDrawQueue.Submit(cmd, myCurrentFrame.MyImageAvailableSemaphore, myCurrentFrame.MyRenderFinishedSemaphore, myCurrentFrame.MyRenderFence);
        
        myDrawQueue.Present(mySwapchain, myCurrentFrame.MyRenderFinishedSemaphore, imageIndex);
        
        myFrameNumber++;
    }

    private unsafe void DrawGeometry(CommandBuffer cmd)
    {
        cmd.BeginRendering(myDrawImage);

        cmd.BindPipeline(myTrianglePipeline);

        MeshPushConstants pushConstants = new();
        pushConstants.WorldMatrix = Matrix4x4.Identity;
        pushConstants.VertexBufferAddress = myTriangleMeshBuffers.MyVertexBufferAddress;

        Vulkan.vkCmdPushConstants(cmd.MyVkCommandBuffer, myTrianglePipeline.MyVkLayout, VkShaderStageFlags.Vertex, 0, (uint)sizeof(MeshPushConstants), &pushConstants);
        Vulkan.vkCmdBindIndexBuffer(cmd.MyVkCommandBuffer, myTriangleMeshBuffers.MyIndexBuffer.MyBuffer, 0, VkIndexType.Uint32);

        cmd.DrawIndexed(6);

        cmd.EndRendering();
    }

    private unsafe void DrawBackground(CommandBuffer cmd)
    {
        cmd.BindPipeline(myGradientPipeline);

        Vulkan.vkCmdBindDescriptorSets(cmd.MyVkCommandBuffer, VkPipelineBindPoint.Compute, myGradientPipeline.MyVkLayout, 0, myDrawImageDescriptorSet);

        PushConstants pushConstants = new();
        pushConstants.data1.X = 0f;

        Vulkan.vkCmdPushConstants(cmd.MyVkCommandBuffer, myGradientPipeline.MyVkLayout, VkShaderStageFlags.Compute, 0,
            (uint)sizeof(PushConstants), &pushConstants);

        Vulkan.vkCmdDispatch(cmd.MyVkCommandBuffer, (uint)Math.Ceiling(mySwapchain.MyExtents.width / 16d),
            (uint)Math.Ceiling(mySwapchain.MyExtents.height / 16d), 1);
    }
}