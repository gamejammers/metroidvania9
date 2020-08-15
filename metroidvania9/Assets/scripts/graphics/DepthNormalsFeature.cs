//
// (c) GameJammers 2020
// http://www.jamming.games
// Thank you https://alexanderameye.github.io/

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Metroidvania
{
    public class DepthNormalsFeature
        : ScriptableRendererFeature
    {
        //
        // passes /////////////////////////////////////////////////////////////
        //

        private class DepthNormalsPass
            : ScriptableRenderPass
        {
            //
            // constants //////////////////////////////////////////////////////
            //

            private static readonly int kDepthBufferBits        = 32;
            private static readonly string kProfilerTag         = "DepthNormals Prepass";
            private static readonly ShaderTagId kShaderTagId    = new ShaderTagId("DepthOnly");
            
            //
            // members ////////////////////////////////////////////////////////
            //

            private FilteringSettings filteringSettings;
            private RenderTargetHandle targetHandle;
            private RenderTextureDescriptor descriptor;
            private Material depthNormalsMaterial               = null;
            
            //
            // initialize /////////////////////////////////////////////////////
            //
            
            public DepthNormalsPass(RenderQueueRange renderQueueRange, LayerMask layerMask, Material material)
            {
                filteringSettings = new FilteringSettings(renderQueueRange, layerMask);
                depthNormalsMaterial = material;
            }

            //
            // ----------------------------------------------------------------
            //
            
            public void Setup(RenderTextureDescriptor baseDesc, RenderTargetHandle targetHandle)
            {
                this.targetHandle = targetHandle;
                baseDesc.colorFormat = RenderTextureFormat.ARGB32;
                baseDesc.depthBufferBits = kDepthBufferBits;
                descriptor = baseDesc;
            }

            //
            // ----------------------------------------------------------------
            //

            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDesc)
            {
                cmd.GetTemporaryRT(targetHandle.id, descriptor, FilterMode.Point);
                ConfigureTarget(targetHandle.Identifier());
                ConfigureClear(ClearFlag.All, Color.black);
            }

            //
            // public methods ///////////////////////////////////////////////// 
            //

            public override void Execute(ScriptableRenderContext ctx, ref RenderingData data)
            {
                CommandBuffer cmd = CommandBufferPool.Get(kProfilerTag);
                ctx.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var sortFlags = data.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(kShaderTagId, ref data, sortFlags);
                drawSettings.perObjectData = PerObjectData.None;

                ref CameraData cameraData = ref data.cameraData;
                Camera camera = cameraData.camera;
                if (cameraData.isStereoEnabled)
                    ctx.StartMultiEye(camera);

                drawSettings.overrideMaterial = depthNormalsMaterial;


                ctx.DrawRenderers(data.cullResults, ref drawSettings, ref filteringSettings);

                cmd.SetGlobalTexture("_CameraDepthNormalsTexture", targetHandle.id);

                ctx.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            //
            // ------------------------------------------------------------------------
            //

            public override void FrameCleanup(CommandBuffer cmd)
            {
                if(targetHandle != RenderTargetHandle.CameraTarget)
                {
                    cmd.ReleaseTemporaryRT(targetHandle.id);
                    targetHandle = RenderTargetHandle.CameraTarget;
                }
            }
            
        } // Pass Class

        //
        // members ////////////////////////////////////////////////////////////
        //
        
        private DepthNormalsPass pass;
        private RenderTargetHandle target;
        private Material material;

        //
        // public methods /////////////////////////////////////////////////////
        //

        public override void Create()
        {
            material = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
            pass = new DepthNormalsPass(RenderQueueRange.opaque, -1, material);
            pass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
            target.Init("_CameraDepthNormalsTexture");
        }

        //
        // --------------------------------------------------------------------
        //
        
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
        {
            pass.Setup(data.cameraData.cameraTargetDescriptor, target);
            renderer.EnqueuePass(pass);
        }

    } // DepthNormalsFeature
} // namespace Metroidvania
