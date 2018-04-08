using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

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
        /// Time when script started (to be used for output filename prefix)
        /// </summary>
        private DateTime _script_started_time;


        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectDetector()
        {
            // load setting from json file
            _save_settings = ExportDetectionSettings.DeserializeFromJsonOrDefault("scripts/vision_data_extractor_settings.json");

            _script_started_time = DateTime.Now;

            // set event handlers
            this.Tick += OnTick;

        }

        /// <summary>
        /// Callback method in every frame (main entry method)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e)
        {
            _frame_count++;

            // check if this frame is target frame
            if (_frame_count % _save_settings.SaveFrameSpan == 0)
            {
                DoDetectionExport();
            }
        }

        /// <summary>
        /// Perform detection data export (Toolchain method: call GTA5 native functions & utility functions)
        /// </summary>
        private void DoDetectionExport()
        {
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

            List<GTAUtils.BoundingBox2D> peds_bblist = new List<GTAUtils.BoundingBox2D>();
            List<GTAUtils.BoundingBox2D> vehicle_bblist = new List<GTAUtils.BoundingBox2D>();

            // detect 2D bounding box
            foreach (var entity in found_entities)
            {
                // check if is in screen
                if (!entity.IsVisible) continue;
                if (!entity.IsOnScreen) continue;
                if (entity.IsOccluded) continue;

                // check occlusion by ray-casting
                // ...

                var bb = GTAUtils.ComputeBoundingBox(entity);

                // cannot get bounding box
                if (bb == null) continue;

                if (entity.Model.IsPed)
                {
                    peds_bblist.Add(bb);
                }
                else
                {
                    vehicle_bblist.Add(bb);
                }
            }

            var screenshot = Utils.GetPrimaryScreenImage();

            // override to image
            var overdrawn_image = new Bitmap(screenshot);
            DrawBoundingBoxes(overdrawn_image, peds_bblist, Color.Red);
            DrawBoundingBoxes(overdrawn_image, vehicle_bblist, Color.Blue);

            // save image
            var filename_prefix = _script_started_time.ToString("yyyy-MM-dd_hh-mm-ss_");
            var filepath_prefix = System.IO.Path.Combine(_save_settings.OutputDirectory, filename_prefix);
            var filenumber_str = string.Format("{0:D8}", _frame_count);
            Utils.SaveBitmap(overdrawn_image, filepath_prefix + "with_rect_" + filenumber_str + ".jpg");
            Utils.SaveBitmap(screenshot, filepath_prefix + "original_" + filenumber_str + ".png");
        }

        /// <summary>
        /// Draw all bounding boxes to the given image
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="bbs"></param>
        /// <param name="draw_color"></param>
        private void DrawBoundingBoxes(Bitmap bitmap, List<GTAUtils.BoundingBox2D> bbs, Color draw_color)
        {
            int width = bitmap.Size.Width;
            int height = bitmap.Size.Height;

            Graphics g = Graphics.FromImage(bitmap);
            Pen pen = new Pen(draw_color, 2);

            foreach (var bb in bbs)
            {
                int x = (int)(width * bb.min.X);
                int y = (int)(height * bb.min.Y);
                int w = (int)(width * (bb.max.X - bb.min.X));
                int h = (int)(height * (bb.max.Y - bb.min.Y));

                g.DrawRectangle(pen, new Rectangle(x, y, w, h));
            }

            pen.Dispose();
            g.Dispose();
        }


        
    }
}
