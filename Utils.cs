using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace gta5_vision_data_extractor
{
    /// <summary>
    /// Generic util class
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Create and return the current fullscreen image
        /// </summary>
        /// <returns></returns>
        public static Bitmap GetPrimaryScreenImage()
        {
            var primaryScreen = Screen.PrimaryScreen;

            // create new bitmap
            int width = primaryScreen.Bounds.Width;
            int height = primaryScreen.Bounds.Height;
            Bitmap sc_image = new Bitmap(width, height);

            // copy from screen
            var graphic = Graphics.FromImage(sc_image);
            graphic.CopyFromScreen(0, 0, 0, 0, new Size(width, height));
            graphic.Dispose();

            return sc_image;
        }

        /// <summary>
        /// Save the image to the path as an image file
        /// </summary>
        /// <param name="image"></param>
        /// <param name="save_path"></param>
        public static void SaveBitmap(Image image, string save_path)
        {
            image.Save(save_path);
        }
    }
}
