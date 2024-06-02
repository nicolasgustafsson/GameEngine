using System.Numerics;

namespace Graphics;

public unsafe class Image : IGpuDestroyable
{
    public VkImage MyVkImage;
    public ImageView MyImageView;
    public VmaAllocation MyAllocation;
    public VkExtent3D MyExtent;
    public VkFormat MyImageFormat;
    
    public VkImageLayout MyCurrentLayout = VkImageLayout.Undefined;
    
    public Image(VkFormat aFormat, VkImageUsageFlags aUsageFlags, VkExtent3D aExtent, bool aMipMaps)
    {
        MyExtent = aExtent;
        MyImageFormat = aFormat;
        
        VkImageCreateInfo createInfo = new();
        createInfo.imageType = VkImageType.Image2D;
        createInfo.format = aFormat;
        createInfo.extent = aExtent;
        createInfo.mipLevels = 1;
        
        if (aMipMaps)
            createInfo.mipLevels = (uint)Math.Floor(Math.Log2(Math.Max(aExtent.width, aExtent.height))) + 1;
        
        createInfo.arrayLayers = 1;
        
        createInfo.samples = VkSampleCountFlags.Count1;
        
        createInfo.tiling = VkImageTiling.Optimal;
        createInfo.usage = aUsageFlags;

        VmaAllocationCreateInfo allocCreateInfo = new();
        allocCreateInfo.usage = VmaMemoryUsage.GpuOnly;
        allocCreateInfo.requiredFlags = VkMemoryPropertyFlags.DeviceLocal;
        
        Vma.vmaCreateImage(GlobalAllocator.myVmaAllocator, &createInfo, &allocCreateInfo, out MyVkImage, out MyAllocation, out VmaAllocationInfo allocInfo).CheckResult();

        MyImageView = new(MyVkImage, aFormat, (int)(aUsageFlags & VkImageUsageFlags.DepthStencilAttachment) != 0 ? VkImageAspectFlags.Depth : VkImageAspectFlags.Color, createInfo.mipLevels);
    }
    
    public Image(IGpuImmediateSubmit aSubmit, byte* aData, VkFormat aFormat, VkImageUsageFlags aUsageFlags, VkExtent3D aExtent, bool aMipMaps) : this(aFormat, aUsageFlags | VkImageUsageFlags.TransferDst, aExtent, aMipMaps)
    {
        ulong dataSize = aExtent.width * aExtent.height * aExtent.depth * 4;

        AllocatedRawBuffer staging = new(dataSize, VkBufferUsageFlags.TransferSrc, VmaMemoryUsage.CpuToGpu);

        Buffer.MemoryCopy(aData, staging.MyAllocationInfo.pMappedData, dataSize, dataSize);

        aSubmit.ImmediateSubmit(cmd =>
        {
            cmd.TransitionImage(this, VkImageLayout.TransferDstOptimal);

            VkBufferImageCopy copyRegion = new();
            copyRegion.bufferOffset = 0;
            copyRegion.bufferRowLength = 0;
            copyRegion.bufferImageHeight = 0;

            copyRegion.imageSubresource.aspectMask = VkImageAspectFlags.Color;
            copyRegion.imageSubresource.mipLevel = 0;
            copyRegion.imageSubresource.baseArrayLayer = 0;
            copyRegion.imageSubresource.layerCount = 1;
            copyRegion.imageExtent = aExtent;

            vkCmdCopyBufferToImage(cmd.MyVkCommandBuffer, staging.MyBuffer, MyVkImage, VkImageLayout.TransferDstOptimal, 1, &copyRegion);

            cmd.TransitionImage(this, VkImageLayout.ReadOnlyOptimal);
        });

        staging.Destroy();
    }
    
    public void Destroy()
    {
        MyImageView.Destroy();

        Vma.vmaDestroyImage(GlobalAllocator.myVmaAllocator, MyVkImage, MyAllocation);
    }

    public VkRenderingAttachmentInfo GetAttachmentInfo(VkClearValue? aClear,  VkImageLayout aLayout = VkImageLayout.ColorAttachmentOptimal)
    {
        VkRenderingAttachmentInfo colorAttachment = new();

        colorAttachment.imageView = MyImageView.MyVkImageView;
        colorAttachment.imageLayout = aLayout;
        colorAttachment.loadOp = aClear == null ? VkAttachmentLoadOp.Load : VkAttachmentLoadOp.Clear;
        colorAttachment.storeOp = VkAttachmentStoreOp.Store;
        if (aClear.HasValue)
            colorAttachment.clearValue = aClear.Value;

        return colorAttachment;
    }
    
    public VkRenderingAttachmentInfo GetDepthAttachmentInfo(VkImageLayout layout = VkImageLayout.DepthAttachmentOptimal)
    {
        VkRenderingAttachmentInfo depthAttachment = new();
        
        depthAttachment.imageView = MyImageView.MyVkImageView;
        depthAttachment.imageLayout = layout;
        depthAttachment.loadOp = VkAttachmentLoadOp.Clear;
        depthAttachment.storeOp = VkAttachmentStoreOp.Store;
        depthAttachment.clearValue.depthStencil = new(0f, 0);

        return depthAttachment;
    }
    
    public VkRenderingInfo GetRenderingInfo(VkExtent2D renderExtent, VkRenderingAttachmentInfo colorAttachment, VkRenderingAttachmentInfo depthAttachment)
    {
        VkRenderingInfo renderInfo = new();
        
        renderInfo.renderArea = new VkRect2D(new VkOffset2D ( 0, 0 ), renderExtent);
        renderInfo.layerCount = 1;
        renderInfo.colorAttachmentCount = 1;
        renderInfo.pColorAttachments = &colorAttachment;
        renderInfo.pDepthAttachment = &depthAttachment;
        
        renderInfo.pStencilAttachment = null;

        return renderInfo;
    } 
}