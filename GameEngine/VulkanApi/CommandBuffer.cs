namespace Graphics;

//Currently only one buffer per pool, this might change in the future
public unsafe class CommandBuffer
{
    public VkCommandPool MyVkCommandPool;
    public VkCommandBuffer MyVkCommandBuffer;
    
    public CommandBuffer(Device aDevice, Queue aQueue)
    {
        VkCommandPoolCreateInfo createInfo = new();
        createInfo.flags = VkCommandPoolCreateFlags.ResetCommandBuffer;
        createInfo.queueFamilyIndex = aQueue.MyQueueFamilyIndex;
        
        vkCreateCommandPool(aDevice.MyVkDevice, &createInfo, null, out MyVkCommandPool).CheckResult();

        VkCommandBufferAllocateInfo allocateInfo = new();
        allocateInfo.commandPool = MyVkCommandPool;
        allocateInfo.commandBufferCount = 1;
        allocateInfo.level = VkCommandBufferLevel.Primary;

        vkAllocateCommandBuffer(aDevice.MyVkDevice, &allocateInfo, out MyVkCommandBuffer).CheckResult();
    }
    
    public void Destroy(Device aDevice)
    {
        vkDestroyCommandPool(aDevice.MyVkDevice, MyVkCommandPool);
    }
    
    public void Reset()
    {
        vkResetCommandBuffer(MyVkCommandBuffer, VkCommandBufferResetFlags.None).CheckResult();
    }
    
    public void BeginRecording()
    {
        vkBeginCommandBuffer(MyVkCommandBuffer, VkCommandBufferUsageFlags.OneTimeSubmit).CheckResult();
    }
    
    public void EndRecording()
    {
        vkEndCommandBuffer(MyVkCommandBuffer);
    }
    
    public void TransitionImage(Image aImage, VkImageLayout aFrom, VkImageLayout aTo)
    {
        TransitionImage(aImage.MyVkImage, aFrom, aTo);
    }
    
    public void TransitionImage(VkImage aImage, VkImageLayout aFrom, VkImageLayout aTo)
    {
        VkImageMemoryBarrier2 imageBarrier = new();
        imageBarrier.srcStageMask = VkPipelineStageFlags2.AllCommands;
        imageBarrier.srcAccessMask = VkAccessFlags2.MemoryWrite;
        imageBarrier.dstStageMask = VkPipelineStageFlags2.AllCommands;
        imageBarrier.dstAccessMask = VkAccessFlags2.MemoryRead | VkAccessFlags2.MemoryWrite;

        imageBarrier.oldLayout = aFrom;
        imageBarrier.newLayout = aTo;
        VkImageAspectFlags aspectMask = (aTo == VkImageLayout.DepthAttachmentOptimal)
            ? VkImageAspectFlags.Depth
            : VkImageAspectFlags.Color;

        imageBarrier.subresourceRange = new VkImageSubresourceRange(aspectMask);
        imageBarrier.image = aImage;

        VkDependencyInfo depInfo = new();
        depInfo.imageMemoryBarrierCount = 1;
        depInfo.pImageMemoryBarriers = &imageBarrier;

        vkCmdPipelineBarrier2(MyVkCommandBuffer, &depInfo);
    }
    
    public void Blit(Image aFrom, Image aTo)
    {
        Blit(aFrom, aTo.MyVkImage, new VkExtent2D(aTo.MyExtent.width, aTo.MyExtent.height));
    }
    
    public void Blit(Image aFrom, VkImage aTo, VkExtent2D aExtent)
    {
        VkImageBlit2 blitRegion = new();

        blitRegion.srcOffsets[1].x = (int)aFrom.MyExtent.width;
        blitRegion.srcOffsets[1].y = (int)aFrom.MyExtent.height;
        blitRegion.srcOffsets[1].z = 1;
        
        blitRegion.dstOffsets[1].x = (int)aExtent.width;
        blitRegion.dstOffsets[1].y = (int)aExtent.height;
        blitRegion.dstOffsets[1].z = 1;

        blitRegion.srcSubresource.aspectMask = VkImageAspectFlags.Color;
        blitRegion.srcSubresource.baseArrayLayer = 0;
        blitRegion.srcSubresource.layerCount = 1;
        blitRegion.srcSubresource.mipLevel = 0;
        
        blitRegion.dstSubresource.aspectMask = VkImageAspectFlags.Color;
        blitRegion.dstSubresource.baseArrayLayer = 0;
        blitRegion.dstSubresource.layerCount = 1;
        blitRegion.dstSubresource.mipLevel = 0;

        VkBlitImageInfo2 blitInfo = new();

        blitInfo.srcImage = aFrom.MyVkImage;
        blitInfo.srcImageLayout = VkImageLayout.TransferSrcOptimal;
        
        blitInfo.dstImage = aTo;
        blitInfo.dstImageLayout = VkImageLayout.TransferDstOptimal;
        
        blitInfo.filter = VkFilter.Linear;
        blitInfo.regionCount = 1;
        blitInfo.pRegions = &blitRegion;

        vkCmdBlitImage2(MyVkCommandBuffer, &blitInfo);
    }
    
    public void ClearColor(Image aImage, VkImageLayout aImageLayout, VkClearColorValue aColor)
    {
        VkImageSubresourceRange clearRange = new VkImageSubresourceRange(VkImageAspectFlags.Color);
        vkCmdClearColorImage(MyVkCommandBuffer, aImage.MyVkImage, aImageLayout, &aColor, 1, &clearRange);
    }
}