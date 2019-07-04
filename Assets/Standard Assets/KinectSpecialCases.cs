using RootSystem = System;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using System.Security;

namespace Windows.Kinect
{
    [RootSystem.Runtime.InteropServices.StructLayout(RootSystem.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct PointF
    {
        public float X { get; set; }
        public float Y { get; set; }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PointF))
            {
                return false;
            }

            return this.Equals((ColorSpacePoint)obj);
        }

        public bool Equals(ColorSpacePoint obj)
        {
            return (X == obj.X) && (Y == obj.Y);
        }

        public static bool operator ==(PointF a, PointF b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PointF a, PointF b)
        {
            return !(a.Equals(b));
        }
    }

    public sealed partial class AudioBeamSubFrame
    {
        [RootSystem.Runtime.InteropServices.DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_AudioBeamSubFrame_CopyFrameDataToArray",
            CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_AudioBeamSubFrame_CopyFrameDataToIntPtr(RootSystem.IntPtr pNative, RootSystem.IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(RootSystem.IntPtr frameData, uint size)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("AudioBeamSubFrame");
            }

            Windows_Kinect_AudioBeamSubFrame_CopyFrameDataToIntPtr(_pNative, frameData, size);
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern RootSystem.IntPtr Windows_Kinect_AudioBeamSubFrame_LockAudioBuffer(RootSystem.IntPtr pNative);
        public Windows.Kinect.KinectBuffer LockAudioBuffer()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("AudioBeamSubFrame");
            }

            RootSystem.IntPtr objectPointer = Windows_Kinect_AudioBeamSubFrame_LockAudioBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == RootSystem.IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<Windows.Kinect.KinectBuffer>(objectPointer, n => new Windows.Kinect.KinectBuffer(n));
        }
    }

    public sealed partial class AudioBeamFrame
    {
        private Windows.Kinect.AudioBeamSubFrame[] _subFrames = null;

        private void Dispose(bool disposing)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            if (_subFrames != null)
            {
                foreach (var subFrame in _subFrames)
                {
                    subFrame.Dispose();
                }

                _subFrames = null;
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<AudioBeamFrame>(_pNative);
            Windows_Kinect_AudioBeamFrame_ReleaseObject(ref _pNative);

            if (disposing)
            {
                Windows_Kinect_AudioBeamFrame_Dispose(_pNative);
            }

            _pNative = RootSystem.IntPtr.Zero;
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl)]
        private static extern void Windows_Kinect_AudioBeamFrame_Dispose(RootSystem.IntPtr pNative);
        public void Dispose()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            Dispose(true);
            RootSystem.GC.SuppressFinalize(this);
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Windows_Kinect_AudioBeamFrame_get_SubFrames(RootSystem.IntPtr pNative, [RootSystem.Runtime.InteropServices.Out] RootSystem.IntPtr[] outCollection, int outCollectionSize);
        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl)]
        private static extern int Windows_Kinect_AudioBeamFrame_get_SubFrames_Length(RootSystem.IntPtr pNative);
        public RootSystem.Collections.Generic.IList<Windows.Kinect.AudioBeamSubFrame> SubFrames
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("AudioBeamFrame");
                }

                if (_subFrames == null)
                {
                    int collectionSize = Windows_Kinect_AudioBeamFrame_get_SubFrames_Length(_pNative);
                    var outCollection = new RootSystem.IntPtr[collectionSize];
                    _subFrames = new Windows.Kinect.AudioBeamSubFrame[collectionSize];

                    collectionSize = Windows_Kinect_AudioBeamFrame_get_SubFrames(_pNative, outCollection, collectionSize);
                    Helper.ExceptionHelper.CheckLastError();

                    for (int i = 0; i < collectionSize; i++)
                    {
                        if (outCollection[i] == RootSystem.IntPtr.Zero)
                        {
                            continue;
                        }

                        var obj = Helper.NativeObjectCache.GetObject<Windows.Kinect.AudioBeamSubFrame>(outCollection[i]);
                        if (obj == null)
                        {
                            obj = new Windows.Kinect.AudioBeamSubFrame(outCollection[i]);
                            Helper.NativeObjectCache.AddObject<Windows.Kinect.AudioBeamSubFrame>(outCollection[i], obj);
                        }

                        _subFrames[i] = obj;
                    }
                }

                return _subFrames;
            }
        }
    }

    public sealed partial class BodyFrame : IBodyFrame
    {
        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_BodyFrame_GetAndRefreshBodyData(RootSystem.IntPtr pNative, [RootSystem.Runtime.InteropServices.Out] RootSystem.IntPtr[] bodies, int bodiesSize);
        public override void GetAndRefreshBodyData(RootSystem.Collections.Generic.IList<IBody> bodies)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("BodyFrame");
            }

            int _bodies_idx = 0;
            var _bodies = new RootSystem.IntPtr[bodies.Count];
            for (int i = 0; i < bodies.Count; i++)
            {
                if (bodies[i] == null)
                {
                    bodies[i] = new Body();
                }

                _bodies[_bodies_idx] = bodies[i].GetIntPtr();
                _bodies_idx++;
            }

            Windows_Kinect_BodyFrame_GetAndRefreshBodyData(_pNative, _bodies, bodies.Count);
            Helper.ExceptionHelper.CheckLastError();

            for (int i = 0; i < bodies.Count; i++)
            {
                bodies[i].SetIntPtr(_bodies[i]);
            }
        }
    }

    public sealed partial class Body
    {
        internal override void SetIntPtr(RootSystem.IntPtr value) { _pNative = value; }
        internal override RootSystem.IntPtr GetIntPtr() { return _pNative; }

        internal Body() { }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern RootSystem.IntPtr Windows_Kinect_Body_get_Lean(RootSystem.IntPtr pNative);
        public Windows.Kinect.PointF Lean
        {
            get
            {
                if (_pNative == RootSystem.IntPtr.Zero)
                {
                    throw new RootSystem.ObjectDisposedException("Body");
                }

                var objectPointer = Windows_Kinect_Body_get_Lean(_pNative);
                Helper.ExceptionHelper.CheckLastError();

                var obj = (Windows.Kinect.PointF)RootSystem.Runtime.InteropServices.Marshal.PtrToStructure(objectPointer, typeof(Windows.Kinect.PointF));
                RootSystem.Runtime.InteropServices.Marshal.FreeHGlobal(objectPointer);
                return obj;
            }
        }
    }

    public sealed partial class ColorFrame
    {
        [RootSystem.Runtime.InteropServices.DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_ColorFrame_CopyRawFrameDataToArray",
            CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_ColorFrame_CopyRawFrameDataToIntPtr(RootSystem.IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyRawFrameDataToIntPtr(RootSystem.IntPtr frameData, uint size)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("ColorFrame");
            }

            Windows_Kinect_ColorFrame_CopyRawFrameDataToIntPtr(_pNative, frameData, size);
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_ColorFrame_CopyConvertedFrameDataToArray",
            CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_ColorFrame_CopyConvertedFrameDataToIntPtr(RootSystem.IntPtr pNative, IntPtr frameData, uint frameDataSize, Windows.Kinect.ColorImageFormat colorFormat);
        public void CopyConvertedFrameDataToIntPtr(RootSystem.IntPtr frameData, uint size, Windows.Kinect.ColorImageFormat colorFormat)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("ColorFrame");
            }

            Windows_Kinect_ColorFrame_CopyConvertedFrameDataToIntPtr(_pNative, frameData, size, colorFormat);
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern RootSystem.IntPtr Windows_Kinect_ColorFrame_LockRawImageBuffer(RootSystem.IntPtr pNative);
        public Windows.Kinect.KinectBuffer LockRawImageBuffer()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("ColorFrame");
            }

            RootSystem.IntPtr objectPointer = Windows_Kinect_ColorFrame_LockRawImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == RootSystem.IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<Windows.Kinect.KinectBuffer>(objectPointer, n => new Windows.Kinect.KinectBuffer(n));
        }

    }

    public sealed partial class DepthFrame
    {
        [RootSystem.Runtime.InteropServices.DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_DepthFrame_CopyFrameDataToArray",
            CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_DepthFrame_CopyFrameDataToIntPtr(RootSystem.IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(RootSystem.IntPtr frameData, uint size)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("DepthFrame");
            }

            Windows_Kinect_DepthFrame_CopyFrameDataToIntPtr(_pNative, frameData, size / sizeof(ushort));
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern RootSystem.IntPtr Windows_Kinect_DepthFrame_LockImageBuffer(RootSystem.IntPtr pNative);
        public Windows.Kinect.KinectBuffer LockImageBuffer()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("DepthFrame");
            }

            RootSystem.IntPtr objectPointer = Windows_Kinect_DepthFrame_LockImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == RootSystem.IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<Windows.Kinect.KinectBuffer>(objectPointer, n => new Windows.Kinect.KinectBuffer(n));
        }
    }

    public sealed partial class BodyIndexFrame
    {
        [RootSystem.Runtime.InteropServices.DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_BodyIndexFrame_CopyFrameDataToArray",
            CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_BodyIndexFrame_CopyFrameDataToIntPtr(RootSystem.IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(RootSystem.IntPtr frameData, uint size)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("BodyIndexFrame");
            }

            Windows_Kinect_BodyIndexFrame_CopyFrameDataToIntPtr(_pNative, frameData, size);
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern RootSystem.IntPtr Windows_Kinect_BodyIndexFrame_LockImageBuffer(RootSystem.IntPtr pNative);
        public Windows.Kinect.KinectBuffer LockImageBuffer()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("BodyIndexFrame");
            }

            RootSystem.IntPtr objectPointer = Windows_Kinect_BodyIndexFrame_LockImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == RootSystem.IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<Windows.Kinect.KinectBuffer>(objectPointer, n => new Windows.Kinect.KinectBuffer(n));
        }

    }

    public sealed partial class InfraredFrame
    {
        [RootSystem.Runtime.InteropServices.DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_InfraredFrame_CopyFrameDataToArray",
            CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_InfraredFrame_CopyFrameDataToIntPtr(RootSystem.IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(RootSystem.IntPtr frameData, uint size)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("InfraredFrame");
            }

            Windows_Kinect_InfraredFrame_CopyFrameDataToIntPtr(_pNative, frameData, size / sizeof(ushort));
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern RootSystem.IntPtr Windows_Kinect_InfraredFrame_LockImageBuffer(RootSystem.IntPtr pNative);
        public Windows.Kinect.KinectBuffer LockImageBuffer()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("InfraredFrame");
            }

            RootSystem.IntPtr objectPointer = Windows_Kinect_InfraredFrame_LockImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == RootSystem.IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<Windows.Kinect.KinectBuffer>(objectPointer, n => new Windows.Kinect.KinectBuffer(n));
        }

    }
    
    public sealed partial class KinectSensor : IKinectSensor
    {
        private void Dispose(bool disposing)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                return;
            }

            if (IsOpen)
            {
                Close();
            }

            __EventCleanup();

            Helper.NativeObjectCache.RemoveObject<KinectSensor>(_pNative);
            Windows_Kinect_KinectSensor_ReleaseObject(ref _pNative);

            _pNative = RootSystem.IntPtr.Zero;
        }
    }

    public sealed partial class LongExposureInfraredFrame
    {
        [RootSystem.Runtime.InteropServices.DllImport(
            "KinectUnityAddin",
            EntryPoint = "Windows_Kinect_LongExposureInfraredFrame_CopyFrameDataToArray",
            CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl,
            SetLastError = true)]
        private static extern void Windows_Kinect_LongExposureInfraredFrame_CopyFrameDataToIntPtr(RootSystem.IntPtr pNative, IntPtr frameData, uint frameDataSize);
        public void CopyFrameDataToIntPtr(RootSystem.IntPtr frameData, uint size)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("LongExposureInfraredFrame");
            }

            Windows_Kinect_LongExposureInfraredFrame_CopyFrameDataToIntPtr(_pNative, frameData, size / sizeof(ushort));
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern RootSystem.IntPtr Windows_Kinect_LongExposureInfraredFrame_LockImageBuffer(RootSystem.IntPtr pNative);
        public Windows.Kinect.KinectBuffer LockImageBuffer()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("LongExposureInfraredFrame");
            }

            RootSystem.IntPtr objectPointer = Windows_Kinect_LongExposureInfraredFrame_LockImageBuffer(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            if (objectPointer == RootSystem.IntPtr.Zero)
            {
                return null;
            }

            return Helper.NativeObjectCache.CreateOrGetObject<Windows.Kinect.KinectBuffer>(objectPointer, n => new Windows.Kinect.KinectBuffer(n));
        }

    }

    public sealed partial class CoordinateMapper
    {
        private PointF[] _DepthFrameToCameraSpaceTable = null;

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern RootSystem.IntPtr Windows_Kinect_CoordinateMapper_GetDepthCameraIntrinsics(RootSystem.IntPtr pNative);
        public Windows.Kinect.CameraIntrinsics GetDepthCameraIntrinsics()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("CoordinateMapper");
            }

            var objectPointer = Windows_Kinect_CoordinateMapper_GetDepthCameraIntrinsics(_pNative);
            Helper.ExceptionHelper.CheckLastError();

            var obj = (Windows.Kinect.CameraIntrinsics)RootSystem.Runtime.InteropServices.Marshal.PtrToStructure(objectPointer, typeof(Windows.Kinect.CameraIntrinsics));
            RootSystem.Runtime.InteropServices.Marshal.FreeHGlobal(objectPointer);
            return obj;
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern int Windows_Kinect_CoordinateMapper_GetDepthFrameToCameraSpaceTable(RootSystem.IntPtr pNative, RootSystem.IntPtr outCollection, uint outCollectionSize);
        public Windows.Kinect.PointF[] GetDepthFrameToCameraSpaceTable()
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("CoordinateMapper");
            }

            if (_DepthFrameToCameraSpaceTable == null)
            {
                var desc = KinectSensor.GetDefault().DepthFrameSource.FrameDescription;
                _DepthFrameToCameraSpaceTable = new PointF[desc.Width * desc.Height];

                var pointsSmartGCHandle = new Helper.SmartGCHandle(RootSystem.Runtime.InteropServices.GCHandle.Alloc(_DepthFrameToCameraSpaceTable, RootSystem.Runtime.InteropServices.GCHandleType.Pinned));
                var _points = pointsSmartGCHandle.AddrOfPinnedObject();
                Windows_Kinect_CoordinateMapper_GetDepthFrameToCameraSpaceTable(_pNative, _points, (uint)_DepthFrameToCameraSpaceTable.Length);
                Helper.ExceptionHelper.CheckLastError();
            }

            return _DepthFrameToCameraSpaceTable;
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_CoordinateMapper_MapColorFrameToDepthSpace(
            RootSystem.IntPtr pNative,
            RootSystem.IntPtr depthFrameData,
            uint depthFrameDataSize,
            RootSystem.IntPtr depthSpacePoints,
            uint depthSpacePointsSize);
        public void MapColorFrameToDepthSpaceUsingIntPtr(RootSystem.IntPtr depthFrameData, uint depthFrameSize, RootSystem.IntPtr depthSpacePoints, uint depthSpacePointsSize)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("CoordinateMapper");
            }

            uint length = depthFrameSize / sizeof(UInt16);
            Windows_Kinect_CoordinateMapper_MapColorFrameToDepthSpace(_pNative, depthFrameData, length, depthSpacePoints, depthSpacePointsSize);
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_CoordinateMapper_MapColorFrameToCameraSpace(
            RootSystem.IntPtr pNative,
            RootSystem.IntPtr depthFrameData,
            uint depthFrameDataSize,
            RootSystem.IntPtr cameraSpacePoints,
            uint cameraSpacePointsSize);
        public void MapColorFrameToCameraSpaceUsingIntPtr(RootSystem.IntPtr depthFrameData, int depthFrameSize, RootSystem.IntPtr cameraSpacePoints, uint cameraSpacePointsSize)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("CoordinateMapper");
            }

            uint length = (uint)depthFrameSize / sizeof(UInt16);
            Windows_Kinect_CoordinateMapper_MapColorFrameToCameraSpace(_pNative, depthFrameData, length, cameraSpacePoints, cameraSpacePointsSize);
            Helper.ExceptionHelper.CheckLastError();
        }

        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_CoordinateMapper_MapDepthFrameToColorSpace(
            RootSystem.IntPtr pNative,
            RootSystem.IntPtr depthFrameData,
            uint depthFrameDataSize,
            RootSystem.IntPtr colorSpacePoints,
            uint colorSpacePointsSize);
        public void MapDepthFrameToColorSpaceUsingIntPtr(RootSystem.IntPtr depthFrameData, int depthFrameSize, RootSystem.IntPtr colorSpacePoints, uint colorSpacePointsSize)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("CoordinateMapper");
            }

            uint length = (uint)depthFrameSize / sizeof(UInt16);
            Windows_Kinect_CoordinateMapper_MapDepthFrameToColorSpace(_pNative, depthFrameData, length, colorSpacePoints, colorSpacePointsSize);
            Helper.ExceptionHelper.CheckLastError();
        }


        [RootSystem.Runtime.InteropServices.DllImport("KinectUnityAddin", CallingConvention = RootSystem.Runtime.InteropServices.CallingConvention.Cdecl, SetLastError = true)]
        private static extern void Windows_Kinect_CoordinateMapper_MapDepthFrameToCameraSpace(
            RootSystem.IntPtr pNative,
            IntPtr depthFrameData,
            uint depthFrameDataSize,
            RootSystem.IntPtr cameraSpacePoints,
            uint cameraSpacePointsSize);
        public void MapDepthFrameToCameraSpaceUsingIntPtr(RootSystem.IntPtr depthFrameData, int depthFrameSize, RootSystem.IntPtr cameraSpacePoints, uint cameraSpacePointsSize)
        {
            if (_pNative == RootSystem.IntPtr.Zero)
            {
                throw new RootSystem.ObjectDisposedException("CoordinateMapper");
            }

            uint length = (uint)depthFrameSize / sizeof(UInt16);
            Windows_Kinect_CoordinateMapper_MapDepthFrameToCameraSpace(_pNative, depthFrameData, length, cameraSpacePoints, cameraSpacePointsSize);
            Helper.ExceptionHelper.CheckLastError();
        }
    }


    public abstract class IKinectSensor
    {
        public abstract IBodyFrameSource BodyFrameSource { get; }
        public abstract bool IsOpen { get; }
        public abstract void Open();
        public abstract void Close();
    }

    public sealed partial class KinectSensorMock : IKinectSensor
    {
        private BodyFrameSourceMock source = new BodyFrameSourceMock();

        public override IBodyFrameSource BodyFrameSource
        {
            get
            {
                return source;
            }
        }

        public override bool IsOpen => true;

        public override void Close() { }

        public override void Open() { }
    }

    internal class BodyFrameSourceMock : IBodyFrameSource
    {
        public override int BodyCount => reader.BodyCount;

        private BodyFrameReaderMock reader;

        public override IBodyFrameReader OpenReader()
        {
            throw new NotImplementedException("Cannot open mock reader without path!");
        }

        public override IBodyFrameReader OpenReader(string path)
        {
            reader = new BodyFrameReaderMock(path);
            return reader;
        }
    }

    internal class BodyFrameReaderMock : IBodyFrameReader
    {
        [SuppressUnmanagedCodeSecurity]
        internal static class SafeNativeMethods
        {
            [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
            public static extern int StrCmpLogicalW(string psz1, string psz2);
        }

        public sealed class NaturalStringComparer : IComparer<string>
        {
            public int Compare(string a, string b)
            {
                return SafeNativeMethods.StrCmpLogicalW(a, b);
            }
        }

        public List<IBody[]> frames = new List<IBody[]>();

        private int frameIndex = -1;

        public int BodyCount
        {
            get
            {
                return frames[frameIndex].Length;
            }
        }

        public BodyFrameReaderMock(string path)
        {
            List<string> files = new List<string>(Directory.GetFiles(path));
            files = files.Where(f => f.EndsWith(".json")).ToList();

            files.Sort(new NaturalStringComparer());

            foreach (string file in files)
            {
                object raw = JsonConvert.DeserializeObject(File.ReadAllText(file));

                frames.Add(JsonConvert.DeserializeObject<BodyMock[]>(File.ReadAllText(file)));
            }

            if (frames.Count <= 0)
            {
                throw new ArgumentException("Could not read frames from path: " + path);
            }
        }

        public override IBodyFrame AcquireLatestFrame()
        {
            frameIndex++;
            frameIndex %= frames.Count;
            return new BodyFrameMock(frames[frameIndex]);
        }

        public override void Dispose() { }
    }

    internal class BodyFrameMock : IBodyFrame
    {
        private readonly IBody[] bodies;

        public BodyFrameMock(IBody[] bodies)
        {
            this.bodies = bodies;
        }

        public override void Dispose() { }

        public override void GetAndRefreshBodyData(IList<IBody> bodies)
        {
            for (int i = 0; i < this.bodies.Length; i++)
            {
                bodies[i] = this.bodies[i];
            }
        }
    }

    public class BodyMock : IBody
    {
        private readonly Dictionary<Activity, DetectionResult> activities;
        private readonly Dictionary<Appearance, DetectionResult> appearance;
        private readonly FrameEdges clippedEdges;
        private readonly DetectionResult engaged;
        private readonly Dictionary<Expression, DetectionResult> expressions;
        private readonly TrackingConfidence handLeftConfidence;
        private readonly HandState handLeftState;
        private readonly TrackingConfidence handRightConfidence;
        private readonly bool isRestricted;
        private readonly bool isTracked;
        private readonly Dictionary<JointType, JointOrientation> jointOrientations;
        private readonly Dictionary<JointType, Joint> joints;
        private readonly TrackingState leanTrackingState;
        private readonly ulong trackingId;

        [JsonConstructor]
        public BodyMock(Dictionary<Activity, DetectionResult> activities,
            Dictionary<Appearance, DetectionResult> appearance,
            FrameEdges clippedEdges, DetectionResult engaged,
            Dictionary<Expression, DetectionResult> expressions,
            TrackingConfidence handLeftConfidence, HandState handLeftState,
            TrackingConfidence handRightConfidence,
            bool isRestricted,
            bool isTracked,
            Dictionary<JointType, JointOrientation> jointOrientations,
            Dictionary<JointType, Joint> joints,
            TrackingState leanTrackingState,
            ulong trackingId)
        {
            this.activities = activities;
            this.appearance = appearance;
            this.clippedEdges = clippedEdges;
            this.engaged = engaged;
            this.expressions = expressions;
            this.handLeftConfidence = handLeftConfidence;
            this.handLeftState = handLeftState;
            this.handRightConfidence = handRightConfidence;
            this.isRestricted = isRestricted;
            this.isTracked = isTracked;
            this.jointOrientations = jointOrientations;
            this.joints = joints;
            this.leanTrackingState = leanTrackingState;
            this.trackingId = trackingId;
        }

        public override Dictionary<Activity, DetectionResult> Activities => activities;
        public override Dictionary<Appearance, DetectionResult> Appearance => appearance;
        public override FrameEdges ClippedEdges => clippedEdges;
        public override DetectionResult Engaged => engaged;
        public override Dictionary<Expression, DetectionResult> Expressions => expressions;
        public override TrackingConfidence HandLeftConfidence => handLeftConfidence;
        public override HandState HandLeftState => handLeftState;
        public override TrackingConfidence HandRightConfidence => handRightConfidence;
        public override bool IsRestricted => isRestricted;
        public override bool IsTracked => isTracked;
        public override Dictionary<JointType, JointOrientation> JointOrientations => jointOrientations;
        public override Dictionary<JointType, Joint> Joints => joints;
        public override TrackingState LeanTrackingState => leanTrackingState;
        public override ulong TrackingId => trackingId;

        internal override IntPtr GetIntPtr()
        {
            return IntPtr.Add(IntPtr.Zero, 500);
        }

        internal override void SetIntPtr(IntPtr intPtr) { }
    }
}
