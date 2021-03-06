using System;
using System.Diagnostics;
using System.Drawing;
using CommandLine;
using WinUtils.Desktop;

namespace WinUtils
{
    static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parserResults = Parser.Default.ParseArguments<OptionsWallpaper>(args);
                parserResults.WithParsed<OptionsWallpaper>(SetWallpaper);
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Ouch: {e}");
                Environment.Exit(-1);
            }
        }

        private static void SetWallpaper(OptionsWallpaper options)
        {
            if (!string.IsNullOrWhiteSpace(options.Picture))
            {
                Wallpaper.SetWallPaper(options.Picture);
                return;
            }
            if (!string.IsNullOrWhiteSpace(options.SlideShow))
            {
                Wallpaper.SetSlideShow(options.SlideShow, options.PeriodSeconds);
                return;
            }
            if (!string.IsNullOrWhiteSpace(options.Color))
            {
                Color c;
                if (options.Color.StartsWith("#"))
                {
                    // NB: throw
                    Int32 iColor = Convert.ToInt32(options.Color.Substring(1), 16);
                    c = Color.FromArgb(iColor);
                }
                else
                {
                    c = Color.FromName(options.Color);
                    if (!c.IsKnownColor)
                    {
                        // try to parse an hex value
                        throw new Exception($"Unable to parse the color '{options.Color}'");
                    }
                }
                Wallpaper.SetSolidColor(c);                            
            }

            throw new Exception("Unable to parse the wallpaper options");
        }
    }
}
