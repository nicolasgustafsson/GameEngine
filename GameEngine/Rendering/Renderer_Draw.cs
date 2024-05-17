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

        if (myWantsResize)
        {
            Resize();
            myWantsResize = false;
        }
        
        myCurrentFrame.MyDeletionQueue.Flush();
        
        var nextImage = Device.AcquireNextImage(mySwapchain, myCurrentFrame.MyImageAvailableSemaphore);
        uint imageIndex = nextImage.imageIndex;
        if (nextImage.result == VkResult.ErrorOutOfDateKHR)
        {
            myWantsResize = true;
            return;
        }
        
        VkImage currentSwapchainImage = mySwapchain.MyImages[(int)imageIndex];

        CommandBuffer cmd = myCurrentFrame.MyCommandBuffer;

        cmd.Reset();
        cmd.BeginRecording();

        cmd.TransitionImage(myDrawImage, VkImageLayout.General);
        cmd.TransitionImage(myDepthImage, VkImageLayout.DepthAttachmentOptimal);

        DrawBackground(cmd);

        cmd.TransitionImage(myDrawImage, VkImageLayout.ColorAttachmentOptimal);

        DrawGeometry(cmd);

        cmd.TransitionImage(myDrawImage, VkImageLayout.TransferSrcOptimal);
        cmd.TransitionImage(currentSwapchainImage, VkImageLayout.Undefined, VkImageLayout.TransferDstOptimal);

        cmd.Blit(myDrawImage, currentSwapchainImage, mySwapchain.MyExtents);
        
        cmd.TransitionImage(currentSwapchainImage, VkImageLayout.TransferDstOptimal, VkImageLayout.PresentSrcKHR);

        cmd.EndRecording();

        myDrawQueue.Submit(cmd, myCurrentFrame.MyImageAvailableSemaphore, myCurrentFrame.MyRenderFinishedSemaphore, myCurrentFrame.MyRenderFence);
        
        VkResult result = myDrawQueue.Present(mySwapchain, myCurrentFrame.MyRenderFinishedSemaphore, imageIndex);
        
        if (result == VkResult.ErrorOutOfDateKHR)
            myWantsResize = true;
        
        myFrameNumber++;
    }

    private void DrawGeometry(CommandBuffer cmd)
    {
        cmd.BeginRendering(myDrawImage, myDepthImage);

        cmd.BindPipeline(myTrianglePipeline);

        Matrix4x4 view = Matrix4x4.CreateTranslation(new Vector3(0f, 0f, -2f));
        Matrix4x4 projection = MatrixExtensions.CreatePerspectiveFieldOfView(90f * (float)(Math.PI/180f), (float)myDrawImage.MyExtent.width / (float)myDrawImage.MyExtent.height, 10000f, 0.1f);

        projection[1, 1] *= -1f;

        MeshPushConstants pushConstants = new();
        pushConstants.WorldMatrix = view * projection;
        pushConstants.VertexBufferAddress = myMonke.MyMeshBuffers.MyVertexBufferAddress;

        cmd.SetPushConstants(pushConstants, myTrianglePipeline.MyVkLayout, VkShaderStageFlags.Vertex);

        cmd.BindIndexBuffer(myMonke.MyMeshBuffers.MyIndexBuffer);

        cmd.DrawIndexed(myMonke.MySurfaces[0].Count);
       
        cmd.EndRendering();
    }

    private void DrawBackground(CommandBuffer cmd)
    {
        cmd.BindPipeline(myGradientPipeline);

        cmd.BindDescriptorSet(myGradientPipeline.MyVkLayout, myDrawImageDescriptorSet, VkPipelineBindPoint.Compute);

        PushConstants pushConstants = new();
        pushConstants.data1.X = 0f;

        cmd.SetPushConstants(pushConstants, myGradientPipeline.MyVkLayout, VkShaderStageFlags.Compute);

        cmd.DispatchCompute((uint)Math.Ceiling(mySwapchain.MyExtents.width / 16d), (uint)Math.Ceiling(mySwapchain.MyExtents.height / 16d));
    }
}