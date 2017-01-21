using System.Collections.Generic;
using ToolDev_IvyGenerator.Interfaces;

namespace ToolDev_IvyGenerator.DirectX
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Media;
    using SharpDX;
    using SharpDX.Direct3D10;
    using SharpDX.DXGI;
    using Device = SharpDX.Direct3D10.Device1;
    using Image = System.Windows.Controls.Image;

    public partial class Dx10RenderCanvas : Image
    {
        private Device _device;
        private Texture2D _renderTarget;
        private Texture2D _depthStencil;
        private RenderTargetView _renderTargetView;
        private DepthStencilView _depthStencilView;
        private Dx10ImageSource _d3DSurface;
        private Stopwatch _renderTimer;

        private IDx10Viewport _viewport;
        private bool _sceneAttached;
        private float _lastUpdate;

        private DateTime? _lastSizeChange;

        public Color4 ClearColor = new Color4(new Color3(0.2f, 0.2f, 0.2f), 1.0f);

        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(
            "Models", typeof(List<ISceneObject>), typeof(Dx10RenderCanvas), new PropertyMetadata(default(ISceneObject[])));

        public List<ISceneObject> Models
        {
            get { return (List<ISceneObject>) GetValue(ModelProperty); }
            set { SetValue(ModelProperty, value); }
        }

        public static readonly DependencyProperty DeviceProperty = DependencyProperty.Register(
        "Device", typeof(Device), typeof(Dx10RenderCanvas), new PropertyMetadata(default(ISceneObject[])));

        public Device Device
        {
            get { return (Device)GetValue(DeviceProperty); }
            set { SetValue(DeviceProperty, value); }
        }

        public static readonly DependencyProperty CameraProperty = DependencyProperty.Register(
        "Camera", typeof(ICamera), typeof(Dx10RenderCanvas), new PropertyMetadata(default(ICamera)));

        public ICamera Camera
        {
            get { return (ICamera)GetValue(CameraProperty); }
            set { SetValue(CameraProperty, value); }
        }

        public Dx10RenderCanvas()
        {
            _renderTimer = new Stopwatch();
            Loaded += Window_Loaded;
            Unloaded += Window_Closing;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Dx10RenderCanvas.IsInDesignMode)
                return;

            StartD3D();
            StartRendering();
        }

        private void Window_Closing(object sender, RoutedEventArgs e)
        {
            if (IsInDesignMode)
                return;

            StopRendering();
            EndD3D();
        }

        private void StartD3D()
        {
            //Device = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);

            _d3DSurface = new Dx10ImageSource();
            _d3DSurface.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;

            CreateAndBindTargets();

            Source = _d3DSurface;
        }

        private void EndD3D()
        {
            if (_viewport != null)
            {
                _viewport.Deinitialize();
                _sceneAttached = false;
            }

            _d3DSurface.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            Source = null;

            Disposer.RemoveAndDispose(ref _d3DSurface);
            Disposer.RemoveAndDispose(ref _renderTargetView);
            Disposer.RemoveAndDispose(ref _depthStencilView);
            Disposer.RemoveAndDispose(ref _renderTarget);
            Disposer.RemoveAndDispose(ref _depthStencil);
            //Disposer.RemoveAndDispose(ref Device);
        }

        private void CreateAndBindTargets()
        {
            _d3DSurface.SetRenderTargetDX10(null);

            Disposer.RemoveAndDispose(ref _renderTargetView);
            Disposer.RemoveAndDispose(ref _depthStencilView);
            Disposer.RemoveAndDispose(ref _renderTarget);
            Disposer.RemoveAndDispose(ref _depthStencil);

            int width = Math.Max((int)base.ActualWidth, 100);
            int height = Math.Max((int)base.ActualHeight, 100);

            Texture2DDescription colordesc = new Texture2DDescription
            {
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                Format = Format.B8G8R8A8_UNorm,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.Shared,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1
            };

            Texture2DDescription depthdesc = new Texture2DDescription
            {
                BindFlags = BindFlags.DepthStencil,
                Format = Format.D32_Float_S8X24_UInt,
                Width = width,
                Height = height,
                MipLevels = 1,
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                OptionFlags = ResourceOptionFlags.None,
                CpuAccessFlags = CpuAccessFlags.None,
                ArraySize = 1,
            };

            _renderTarget = new Texture2D(Device, colordesc);
            _depthStencil = new Texture2D(Device, depthdesc);
            _renderTargetView = new RenderTargetView(Device, _renderTarget);
            _depthStencilView = new DepthStencilView(Device, _depthStencil);

            _d3DSurface.SetRenderTargetDX10(_renderTarget);
        }

        private void StartRendering()
        {
            if (_renderTimer.IsRunning)
                return;

            CompositionTarget.Rendering += OnRendering;
            _renderTimer.Start();
        }

        private void StopRendering()
        {
            if (!_renderTimer.IsRunning)
                return;

            CompositionTarget.Rendering -= OnRendering;
            _renderTimer.Stop();
        }

        private void OnRendering(object sender, EventArgs e)
        {
            if (!_renderTimer.IsRunning)
                return;

            var currSeconds = (float)_renderTimer.Elapsed.TotalSeconds;
            Render(currSeconds - _lastUpdate);
            _d3DSurface.InvalidateD3DImage();
            _lastUpdate = currSeconds;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            _lastSizeChange = DateTime.Now.AddMilliseconds(200);

            //CreateAndBindTargets();
            base.OnRenderSizeChanged(sizeInfo);
        }

        void Render(float deltaT)
        {
            if (_lastSizeChange.HasValue && _lastSizeChange.Value <= DateTime.Now)
            {
                _lastSizeChange = null;
                CreateAndBindTargets();
            }

            _device = Device;

            if (_device == null)
                return;

            Texture2D renderTarget = _renderTarget;
            if (renderTarget == null)
                return;

            int targetWidth = renderTarget.Description.Width;
            int targetHeight = renderTarget.Description.Height;

            _device.OutputMerger.SetTargets(_depthStencilView, _renderTargetView);
            //appContext._deviceContext.Rasterizer.SetViewports(new Viewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f));
            _device.Rasterizer.SetViewports(new Viewport(0, 0, targetWidth, targetHeight, 0.0f, 1.0f));

            _device.ClearRenderTargetView(_renderTargetView, ClearColor);
            _device.ClearDepthStencilView(_depthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1.0f, 0);

            if (_viewport != null)
            {
                if (!_sceneAttached)
                {
                    _sceneAttached = true;
                    _viewport.Initialize(_device, _renderTargetView, this);
                }

                _viewport.Update(deltaT);
                _viewport.Render(deltaT);
            }

            _device.Flush();
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // this fires when the screensaver kicks in, the machine goes into sleep or hibernate
            // and any other catastrophic losses of the d3d device from WPF's point of view
            if (_d3DSurface.IsFrontBufferAvailable)
                StartRendering();
            else
                StopRendering();
        }

        /// <summary>
        /// Gets a value indicating whether the control is in design mode
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                DependencyProperty prop = DesignerProperties.IsInDesignModeProperty;
                bool isDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(prop, typeof(FrameworkElement)).Metadata.DefaultValue;
                return isDesignMode;
            }
        }

        public IDx10Viewport Viewport
        {
            get { return _viewport; }
            set
            {
                if (ReferenceEquals(_viewport, value))
                    return;

                if (_viewport != null)
                    _viewport.Deinitialize();

                _sceneAttached = false;
                _viewport = value;
            }
        }

        public Device GetDevice()
        {
            return Device;
        }
    }
}
