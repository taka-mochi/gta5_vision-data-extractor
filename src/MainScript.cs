using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Native;
using GTA.Math;

namespace gta5_vision_data_extractor
{
    /// <summary>
    /// Main GTA5 mod script class to detect and save objects information in game
    /// </summary>
    public class ObjectDetector : Script
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectDetector()
        {
            this.Tick += onTick;
        }

        /// <summary>
        /// Callback method in every frame (main entry method)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTick(object sender, EventArgs e)
        {

        }

    }
}
