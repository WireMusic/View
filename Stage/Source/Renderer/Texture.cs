using System;

namespace Stage.Renderer
{
    public class Texture
    {
        private uint RendererID = 0;
        
        public uint Width { get; private set; }
        public uint Height { get; private set; }

        public unsafe Texture(string path)
        {
            uint id = 0;

            byte* imageData = GetImageData(path, out uint width, out uint height, out int internalFormat, out int dataFormat);

            Width = width;
            Height = height;

            _gl.GenTextures(1, &id);
            _gl.BindTexture(0x00000DE1, id);

            RendererID = id;

            _gl.TextureParameteri(RendererID, 0x00002800, 0x00002601);
            _gl.TextureParameteri(RendererID, 0x00002801, 0x00002601);
            _gl.TextureParameteri(RendererID, 0x00002802, 0x00002901);
            _gl.TextureParameteri(RendererID, 0x00002803, 0x00002901);

            _gl.PixelStorei(0x00000CF2, 0);
            _gl.TexImage2D(0x00000DE1, 0, dataFormat, width, height, 0, dataFormat, 0x00001401, (nint)imageData);

            _helper.StbiFree((nint)imageData);
        }

        private static unsafe byte* GetImageData(string path, out uint imageWidth, out uint imageHeight, out int glInternalFormat, out int glDataFormat)
        {
            _helper.StbiSetFlipVerticallyOnLoad(0);
            byte* data = _helper.StbiLoad(path, out int width, out int height, out int channels, 0);
            if (data == null)
                throw new Exception("Could not load image!");

            imageWidth = checked((uint)width);
            imageHeight = checked((uint)height);

            int internalFormat = 0, dataFormat = 0;
            if (channels == 4)
            {
                internalFormat = 0x00008058;
                dataFormat = 0x00001908;
            }
            else
            {
                internalFormat = 0x00008051;
                dataFormat = 0x00001907;
            }

            if (internalFormat == 0 && dataFormat == 0)
                throw new ArgumentException("Unsupported image format!");

            glInternalFormat = internalFormat;
            glDataFormat = dataFormat;

            return data;
        }

        public uint GetID()
        {
            return RendererID;
        }
    }
}
