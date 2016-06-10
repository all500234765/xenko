using System;
using System.Runtime.InteropServices;
using System.Security;
using SiliconStudio.Core;

namespace SiliconStudio.Xenko.Native
{
    public class Celt : IDisposable
    {
        public int SampleRate { get; set; }

        public int BufferSize { get; set; }

        public int Channels { get; set; }

        private IntPtr celtPtr;

        static Celt()
        {
#if SILICONSTUDIO_PLATFORM_WINDOWS
            NativeLibrary.PreloadLibrary(NativeInvoke.Library + ".dll");
#else
            NativeLibrary.PreloadLibrary(NativeInvoke.Library + ".so");
#endif
        }

        /// <summary>
        /// Initialize the Celt encoder/decoder
        /// </summary>
        /// <param name="sampleRate">Required sample rate</param>
        /// <param name="bufferSize">Required buffer size</param>
        /// <param name="channels">Required channels</param>
        /// <param name="decoderOnly">If we desire only to decode set this to true</param>
        public Celt(int sampleRate, int bufferSize, int channels, bool decoderOnly)
        {
            SampleRate = sampleRate;
            BufferSize = bufferSize;
            Channels = channels;
            celtPtr = xnCeltCreate(sampleRate, bufferSize, channels, decoderOnly);
            if (celtPtr == IntPtr.Zero)
            {
                throw new Exception("Failed to create an instance of the celt encoder/decoder.");
            }
        }

        /// <summary>
        /// Dispose the Celt encoder/decoder
        /// Do not call Encode or Decode after disposal!
        /// </summary>
        public void Dispose()
        {
            if (celtPtr != IntPtr.Zero)
            {
                xnCeltDestroy(celtPtr);
                celtPtr = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Decodes compressed celt data into PCM 16 bit shorts
        /// </summary>
        /// <param name="inputBuffer">The input buffer</param>
        /// <param name="inputBufferSize">The size of the valid bytes in the input buffer</param>
        /// <param name="outputSamples">The output buffer, the size of frames should be the same amount that is contained in the input buffer</param>
        /// <returns></returns>
        public unsafe int Decode(byte[] inputBuffer, int inputBufferSize, short[] outputSamples)
        {
            fixed (short* samplesPtr = outputSamples)
            fixed (byte* bufferPtr = inputBuffer)
            {
                return xnCeltDecodeShort(celtPtr, bufferPtr, inputBufferSize, samplesPtr, outputSamples.Length / Channels);
            }
        }

        /// <summary>
        /// Decodes compressed celt data into PCM 16 bit shorts
        /// </summary>
        /// <param name="inputBuffer">The input buffer</param>
        /// <param name="inputBufferSize">The size of the valid bytes in the input buffer</param>
        /// <param name="outputSamples">The output buffer, the size of frames should be the same amount that is contained in the input buffer</param>
        /// <returns></returns>
        public unsafe int Decode(byte[] inputBuffer, int inputBufferSize, short* outputSamples)
        {
            fixed (byte* bufferPtr = inputBuffer)
            {
                return xnCeltDecodeShort(celtPtr, bufferPtr, inputBufferSize, outputSamples, BufferSize);
            }
        }

        /// <summary>
        /// Encode PCM audio into celt compressed format
        /// </summary>
        /// <param name="audioSamples">A buffer containing interleaved channels (as from constructor channels) and samples (can be any number of samples)</param>
        /// <param name="outputBuffer">An array of bytes, the size of the array will be the max possible size of the compressed packet</param>
        /// <returns></returns>
        public unsafe int Encode(short[] audioSamples, byte[] outputBuffer)
        {
            fixed (short* samplesPtr = audioSamples)
            fixed (byte* bufferPtr = outputBuffer)
            {
                return xnCeltEncodeShort(celtPtr, samplesPtr, audioSamples.Length / Channels, bufferPtr, outputBuffer.Length);
            }
        }

        /// <summary>
        /// Decodes compressed celt data into PCM 32 bit floats
        /// </summary>
        /// <param name="inputBuffer">The input buffer</param>
        /// <param name="inputBufferSize">The size of the valid bytes in the input buffer</param>
        /// <param name="outputSamples">The output buffer, the size of frames should be the same amount that is contained in the input buffer</param>
        /// <returns></returns>
        public unsafe int Decode(byte[] inputBuffer, int inputBufferSize, float[] outputSamples)
        {
            fixed (float* samplesPtr = outputSamples)
            fixed (byte* bufferPtr = inputBuffer)
            {
                return xnCeltDecodeFloat(celtPtr, bufferPtr, inputBufferSize, samplesPtr, outputSamples.Length / Channels);
            }
        }

        /// <summary>
        /// Encode PCM audio into celt compressed format
        /// </summary>
        /// <param name="audioSamples">A buffer containing interleaved channels (as from constructor channels) and samples (can be any number of samples)</param>
        /// <param name="outputBuffer">An array of bytes, the size of the array will be the max possible size of the compressed packet</param>
        /// <returns></returns>
        public unsafe int Encode(float[] audioSamples, byte[] outputBuffer)
        {
            fixed (float* samplesPtr = audioSamples)
            fixed (byte* bufferPtr = outputBuffer)
            {
                return xnCeltEncodeFloat(celtPtr, samplesPtr, audioSamples.Length / Channels, bufferPtr, outputBuffer.Length);
            }
        }

#if !SILICONSTUDIO_RUNTIME_CORECLR
        [SuppressUnmanagedCodeSecurity]
#endif
        [DllImport(NativeInvoke.Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr xnCeltCreate(int sampleRate, int bufferSize, int channels, bool decoderOnly);

#if !SILICONSTUDIO_RUNTIME_CORECLR
        [SuppressUnmanagedCodeSecurity]
#endif
        [DllImport(NativeInvoke.Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void xnCeltDestroy(IntPtr celt);

#if !SILICONSTUDIO_RUNTIME_CORECLR
        [SuppressUnmanagedCodeSecurity]
#endif
        [DllImport(NativeInvoke.Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe int xnCeltEncodeFloat(IntPtr celt, float* inputSamples, int numberOfInputSamples, byte* outputBuffer, int maxOutputSize);

#if !SILICONSTUDIO_RUNTIME_CORECLR
        [SuppressUnmanagedCodeSecurity]
#endif
        [DllImport(NativeInvoke.Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe int xnCeltDecodeFloat(IntPtr celt, byte* inputBuffer, int inputBufferSize, float* outputBuffer, int numberOfOutputSamples);

#if !SILICONSTUDIO_RUNTIME_CORECLR
        [SuppressUnmanagedCodeSecurity]
#endif
        [DllImport(NativeInvoke.Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe int xnCeltEncodeShort(IntPtr celt, short* inputSamples, int numberOfInputSamples, byte* outputBuffer, int maxOutputSize);

#if !SILICONSTUDIO_RUNTIME_CORECLR
        [SuppressUnmanagedCodeSecurity]
#endif
        [DllImport(NativeInvoke.Library, CallingConvention = CallingConvention.Cdecl)]
        internal static extern unsafe int xnCeltDecodeShort(IntPtr celt, byte* inputBuffer, int inputBufferSize, short* outputBuffer, int numberOfOutputSamples);
    }
}
