/* 
*   NatCam Core
*   Copyright (c) 2017 Yusuf Olokoba
*/

namespace NatCamU.Core.Platforms {

    using AOT;
    using UnityEngine;
    using System;
    using Dispatch;
    using Native = Platforms.NatCamNative;

    public abstract partial class INatCamBase {

        #region --Events--
        protected PreviewCallback onStart, onFrame;
        #endregion


        #region --Op vars--
        protected Texture2D preview;
        protected IDispatch dispatch;
        protected PhotoCallback photoCallback;
		protected static int twidth;
		protected static int theight;
        private static INatCamBase instance {get {return NatCam.Implementation as INatCamBase;}}
        #endregion


        #region --Native Callbacks--

        [MonoPInvokeCallback(typeof(Native.StartCallback))]
        protected static void OnStart (IntPtr texPtr, int width, int height) {
            instance.dispatch.Dispatch(() => {
#if NATCAM_PROFESSIONAL
                if (instance.preview == null) instance.InitializePreviewBuffer();
#endif
                if ((int)texPtr > 0)
                {
                    instance.preview = instance.preview ?? Texture2D.CreateExternalTexture(width, height, TextureFormat.RGBA32, false, false, texPtr);
                    if (instance.preview.width != width || instance.preview.height != height) instance.preview.Resize(width, height, instance.preview.format, false);
                }
                else
                {
					twidth=width;
					theight=height;
                }
                if (instance.onStart != null) instance.onStart();
            });
        }

        [MonoPInvokeCallback(typeof(Native.PreviewCallback))]
        protected static void OnFrame (IntPtr texPtr) {
            instance.dispatch.Dispatch(() => {
                if (!instance.preview) 
				{
					if ((int)texPtr > 0 && twidth>0 && theight>0)
					{
						instance.preview = instance.preview ?? Texture2D.CreateExternalTexture(twidth, theight, TextureFormat.RGBA32, false, false, texPtr);
						if (instance.preview.width != twidth || instance.preview.height != theight) instance.preview.Resize(twidth, theight, instance.preview.format, false);
					}
					if (instance.onStart != null) instance.onStart();
					return;
				}
                instance.preview.UpdateExternalTexture(texPtr);
                if (instance.onFrame != null) instance.onFrame();
            });
        }
        
        [MonoPInvokeCallback(typeof(Native.PhotoCallback))]
        protected static void OnPhoto (UIntPtr imgPtr, int width, int height, int size, byte orientation) {
            instance.dispatch.Dispatch(() => {
                if (imgPtr == UIntPtr.Zero) return;
                if (instance.photoCallback == null) return;
                var photo = new Texture2D(width, height, TextureFormat.RGBA32, false);
                photo.LoadRawTextureData(unchecked((IntPtr)(long)(ulong)imgPtr), size);
                photo.Apply();
                Native.ReleasePhoto();
                instance.photoCallback(photo, (Orientation)orientation);
                instance.photoCallback = null;
            });
        }
        #endregion
    }
}