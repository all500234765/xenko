using System;
using System.ComponentModel;
using SiliconStudio.Xenko.Engine;
using SiliconStudio.Xenko.Graphics;

namespace SiliconStudio.Xenko.Rendering.Composers
{
    public class ForceAspectRatioSceneRenderer : SceneRendererBase
    {
        public ISceneRenderer Child { get; set; }

        /// <summary>
        /// Gets or sets the aspect ratio.
        /// </summary>
        /// <value>
        /// The aspect ratio.
        /// </value>
        /// <userdoc>The aspect ratio used if Add Letterbox/Pillarbox is checked.</userdoc>
        [DefaultValue(CameraComponent.DefaultAspectRatio)]
        public float FixedAspectRatio { get; set; } = CameraComponent.DefaultAspectRatio;

        /// <summary>
        /// Gets or sets a value wether to edit the Viewport to force the aspect ratio and add letterboxes or pillarboxes where needed
        /// </summary>
        /// <userdoc>If checked and the viewport will be modified to fit the aspect ratio of Default Back Buffer Width and Default Back Buffer Height and letterboxes/pillarboxes might be added.</userdoc>
        public bool ForceAspectRatio { get; set; } = true;

        /// <inheritdoc/>
        protected override void CollectCore(RenderContext renderContext)
        {
            using (renderContext.SaveViewportAndRestore())
            {
                if (ForceAspectRatio)
                    UpdateViewport(ref renderContext.ViewportState.Viewport0, FixedAspectRatio);

                Child?.Collect(renderContext);
            }
        }

        /// <inheritdoc/>
        protected override void DrawCore(RenderDrawContext context)
        {
            using (context.PushRenderTargetsAndRestore())
            {
                if (ForceAspectRatio)
                {
                    var viewport = context.CommandList.Viewport;
                    UpdateViewport(ref viewport, FixedAspectRatio);
                    context.CommandList.SetViewport(viewport);
                }

                Child?.Draw(context);
            }
        }

        private static void UpdateViewport(ref Viewport currentViewport, float fixedAspectRatio)
        {
            var currentAr = currentViewport.Width / currentViewport.Height;
            var requiredAr = fixedAspectRatio;

            // Pillarbox 
            if (currentAr > requiredAr)
            {
                var newWidth = (float)Math.Max(1.0f, Math.Round(currentViewport.Height * requiredAr));
                var adjX = (float)Math.Round(0.5f * (currentViewport.Width - newWidth));
                currentViewport = new Viewport(currentViewport.X + (int)adjX, currentViewport.Y, (int)newWidth, currentViewport.Height);
            }
            // Letterbox
            else
            {
                var newHeight = (float)Math.Max(1.0f, Math.Round(currentViewport.Width / requiredAr));
                var adjY = (float)Math.Round(0.5f * (currentViewport.Height - newHeight));
                currentViewport = new Viewport(currentViewport.X, currentViewport.Y + (int)adjY, currentViewport.Width, (int)newHeight);
            }
        }
    }
}