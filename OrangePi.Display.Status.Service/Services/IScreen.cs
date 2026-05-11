using Iot.Device.Graphics;

namespace OrangePi.Display.Status.Service.Services
{
    public interface IScreen : IDisposable
    {
        enum FlipType
        {
            Horizontally = 0,
            Vertically = 1,
        }
        public int Width { get; }
        public int Height { get; }

        void Flip(IEnumerable<FlipType> rotationTypes);
        void FlipVertically();
        void FlipHorizontally();
        void Enable();
        void Disable();
        void Clear();
        void DrawImage(BitmapImage bitmapImage);
    }
}
