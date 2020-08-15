//
// (c) GameJammers 2020
// http://www.jamming.games
// God bless you https://alexanderameye.github.io/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class OutlineFeature 
    : ScriptableRendererFeature
{
    //
    // passes //////////////////////////////////////////////////////////////////
    //
    
    class OutlinePass 
        : ScriptableRenderPass
    {
        //
        // members ////////////////////////////////////////////////////////////
        //
        
        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }
        public Material outlineMaterial = null;
        RenderTargetHandle temporaryColorTexture;

        //
        // setup //////////////////////////////////////////////////////////////
        //

        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }

        //
        // --------------------------------------------------------------------
        //

        public OutlinePass(Material outlineMaterial)
        {
            this.outlineMaterial = outlineMaterial;
        }

        //
        // --------------------------------------------------------------------
        //

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {

        }

        //
        // public methods /////////////////////////////////////////////////////
        //
        
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("_OutlinePass");

            RenderTextureDescriptor opaqueDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDescriptor.depthBufferBits = 0;

            if (destination == RenderTargetHandle.CameraTarget)
            {
                cmd.GetTemporaryRT(temporaryColorTexture.id, opaqueDescriptor, FilterMode.Point);
                Blit(cmd, source, temporaryColorTexture.Identifier(), outlineMaterial, 0);
                Blit(cmd, temporaryColorTexture.Identifier(), source);

            }
            else Blit(cmd, source, destination.Identifier(), outlineMaterial, 0);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        //
        // --------------------------------------------------------------------
        //
        
        public override void FrameCleanup(CommandBuffer cmd)
        {

            if (destination == RenderTargetHandle.CameraTarget)
                cmd.ReleaseTemporaryRT(temporaryColorTexture.id);
        }
    }

    //
    // types //////////////////////////////////////////////////////////////////
    //

    [System.Serializable]
    public class OutlineSettings
    {
        public Material outlineMaterial = null;
    }

    //
    // members ////////////////////////////////////////////////////////////////
    //

    public OutlineSettings settings = new OutlineSettings();
    OutlinePass outlinePass;
    RenderTargetHandle outlineTexture;

    //
    // public methods /////////////////////////////////////////////////////////
    //

    public override void Create()
    {
        outlinePass = new OutlinePass(settings.outlineMaterial);
        outlinePass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        outlineTexture.Init("_OutlineTexture");
    }

    //
    // ------------------------------------------------------------------------
    //
    
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.outlineMaterial == null)
        {
            Debug.LogWarningFormat("Missing Outline Material");
            return;
        }
        outlinePass.Setup(renderer.cameraColorTarget, RenderTargetHandle.CameraTarget);
        renderer.EnqueuePass(outlinePass);
    }
}

