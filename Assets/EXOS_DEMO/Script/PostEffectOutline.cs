using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace exiii.Unity.Sample
{
    public class PostEffectOutline : MonoBehaviour
    {

        [SerializeField]
        private Shader _shader;
        [SerializeField]
        private Color _outlineColor;

        // Use this for initialization
        void Start()
        {
            // アウトラインのカラー指定.
            InitOutlineColor();
        }

        // Update is called once per frame
        void Update()
        {

        }

        // アウトラインのカラー指定.
        protected void InitOutlineColor()
        {
            Camera rCam = GetComponent<Camera>();
            if (rCam == null)
            {
                return;
            }
            if (rCam.allowMSAA || rCam.allowHDR)
            {
                Debug.LogError("Unsupported MSAA or HDR.");
                return;
            }
            Material rMat = new Material(_shader);
            rMat.SetColor("_OutlineColor", _outlineColor);

            // コマンドバッファへ.
            CommandBuffer rBuffer = new CommandBuffer();
            int nTempTextureIdentifier = Shader.PropertyToID("_WorldPostEffectTempTexture");
            rBuffer.GetTemporaryRT(nTempTextureIdentifier, -1, -1);
            rBuffer.Blit(BuiltinRenderTextureType.CurrentActive, nTempTextureIdentifier);
            rBuffer.Blit(nTempTextureIdentifier, BuiltinRenderTextureType.CurrentActive, rMat);
            rBuffer.ReleaseTemporaryRT(nTempTextureIdentifier);
            rCam.AddCommandBuffer(CameraEvent.BeforeImageEffects, rBuffer);
        }
    }
}