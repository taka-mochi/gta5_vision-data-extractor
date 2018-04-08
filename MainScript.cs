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
        /// Settings of detection
        /// </summary>
        private ExportDetectionSettings _save_settings = null;

        /// <summary>
        /// Tick called counts
        /// </summary>
        private int _frame_count = 0;


        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectDetector()
        {
            // load setting from json file
            _save_settings = ExportDetectionSettings.DeserializeFromJsonOrDefault("scripts/vision_data_extractor_settings.json");


            // set event handlers
            this.Tick += onTick;

        }

        /// <summary>
        /// Callback method in every frame (main entry method)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onTick(object sender, EventArgs e)
        {
            _frame_count++;

            // check if this frame is target frame
            if (_frame_count % _save_settings.SaveFrameSpan != 0) return;

            // find target objects around camera
            var camera_position = GameplayCamera.Position;

            List<Entity> found_entities = new List<Entity>();

            // find pedestrians
            if (_save_settings.SaveObjectType == SaveObjectType.PEDESTRIANS ||
                _save_settings.SaveObjectType == SaveObjectType.PED_AND_VEHICLES)
            {
                found_entities.AddRange(World.GetNearbyPeds(camera_position, _save_settings.DetectDistance).ToList());
            }

            // find vehicles
            if (_save_settings.SaveObjectType == SaveObjectType.VEHICLES ||
                _save_settings.SaveObjectType == SaveObjectType.PED_AND_VEHICLES)
            {
                found_entities.AddRange(World.GetNearbyVehicles(camera_position, _save_settings.DetectDistance).ToList());
            }

            // detect 2D bounding box

            // override to image

            // save image
        }
    }
}
