#nullable enable
using CommandLine;

namespace Utils
{
    [Verb("wallpaper", true, HelpText = "Set the desktop wallpaper")]
    public class OptionsWallpaper
    {
        [Option('c', "color", SetName = "color", Required = false, HelpText = "set the desktop background to a solid color")]
        public string Color { get; set; }
        [Option('p', "picture", SetName = "picture", Required = false, HelpText = "the path to the picture to use as a background (either a color name or an hexadecimal value)")]
        public string Picture { get; set; }
        [Option('s', "slideshow", SetName = "slideshow", Required = false, HelpText = "the path to the folder to use for the slideshow")]
        public string SlideShow { get; set; }
        [Option( "period", SetName = "slideshow", Required = false, Default = 600.0, HelpText = "the interval of times before displaying the next image")]
        public double PeriodSeconds { get; set; }
    }
}
