/*
 * NB:
 * - to use in Powershell, run:
 *  nuget install Vanara.Pinvoke.Shell32
 * - to get the definition of a Color
 * Add-Type -AssemblyName System.Drawing -PassThru
 * Once done, you can run things like
 * $a = [System.Drawing.Color]::FromName("red")
 */
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using Vanara.PInvoke;

namespace Utils.Desktop
{
   internal enum DesktopSlideshowOptions
    {
        ShuffleImages = 0x01,
    }
    internal enum DesktopSlideshowState
    {
        Enabled = 0x01,
        Slideshow = 0x02,
        DisabledByRemoteSession = 0x04,
    }
    internal enum DesktopSlideshowDirection
    {
        Forward = 0,
        Backward = 1,
    }
    internal enum DesktopWallpaperPosition
    {
        Center = 0,
        Tile = 1,
        Stretch = 2,
        Fit = 3,
        Fill = 4,
        Span = 5,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [ComImport, Guid("B92B56A9-8B55-4E14-9A89-0199BBB6F93B"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IDesktopWallpaper
    {
        void SetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorId,
                          [MarshalAs(UnmanagedType.LPWStr)] string wallpaper);

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetWallpaper([MarshalAs(UnmanagedType.LPWStr)] string monitorId);

        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetMonitorDevicePathAt(uint monitorIndex);

        [return: MarshalAs(UnmanagedType.U4)]
        uint GetMonitorDevicePathCount();

        [return: MarshalAs(UnmanagedType.Struct)]
        Rect GetMonitorRECT([MarshalAs(UnmanagedType.LPWStr)] string monitorId);

        void SetBackgroundColor([MarshalAs(UnmanagedType.U4)] uint color);

        [return: MarshalAs(UnmanagedType.U4)]
        uint GetBackgroundColor();

        void SetPosition([MarshalAs(UnmanagedType.I4)] DesktopWallpaperPosition position);

        [return: MarshalAs(UnmanagedType.I4)]
        DesktopWallpaperPosition GetPosition();

        void SetSlideshow(Shell32.IShellItemArray items);
        IntPtr GetSlideshow();

        void SetSlideshowOptions(DesktopSlideshowOptions options, uint slideshowTick);

        [PreserveSig]
        uint GetSlideshowOptions(out DesktopSlideshowDirection options, out uint slideshowTick);

        void AdvanceSlideshow([MarshalAs(UnmanagedType.LPWStr)] string monitorId,
                              [MarshalAs(UnmanagedType.I4)] DesktopSlideshowDirection direction);

        DesktopSlideshowDirection GetStatus();

        bool Enable(bool enabled);
    }
    
    /// <summary>
    /// CoClass DesktopWallpaper
    /// </summary>
    [ComImport, Guid("C2CF3110-460E-4fc1-B9D0-8A1C0C9CC4BD")]
    internal class  WallpaperClass
    {
    }
    
    public static class Wallpaper
    {
        public static void SetSolidColor(Color color)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var wallpaper = (IDesktopWallpaper) (new WallpaperClass());
            wallpaper.Enable(false);
            wallpaper.SetBackgroundColor((uint)ColorTranslator.ToWin32(color));
        }

        public static void SetWallPaper(string imagePath)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var wallpaper = (IDesktopWallpaper) (new WallpaperClass());
            for (uint i = 0; i < wallpaper.GetMonitorDevicePathCount(); i++)
            {
                wallpaper.SetPosition(DesktopWallpaperPosition.Fit);
                wallpaper.SetWallpaper(wallpaper.GetMonitorDevicePathAt(i),imagePath);
            }
            wallpaper.Enable(true);
        }
        
        public static void SetSlideShow(string folderPath, double timeSeconds)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            var wallpaper = (IDesktopWallpaper) (new WallpaperClass());
            var duration = (uint)(timeSeconds * 1000);
            wallpaper.SetSlideshowOptions(DesktopSlideshowOptions.ShuffleImages, duration);
            var guid = typeof(Shell32.IShellItem).GUID;
            Shell32.SHCreateItemFromParsingName(folderPath, null, guid, out var pShellItem);
            var shellItem = (Shell32.IShellItem)pShellItem;
            guid = typeof(Shell32.IShellItemArray).GUID;
            Shell32.SHCreateShellItemArrayFromShellItem(shellItem, in guid, out var items);
            wallpaper.SetSlideshow(items);
            for (uint i = 0; i < wallpaper.GetMonitorDevicePathCount(); i++)
            {
                wallpaper.SetPosition(DesktopWallpaperPosition.Fit);
                // AdvanceSlideshow throws "not implemented"!
                //wallpaper.AdvanceSlideshow(wallpaper.GetMonitorDevicePathAt(i), DesktopSlideshowDirection.Forward);
            }
            // as AdvanceSlideshow doesn't work, we disable and re-enable he desktop picture 
            wallpaper.Enable(false);
            wallpaper.Enable(true);
        }
    }
}
