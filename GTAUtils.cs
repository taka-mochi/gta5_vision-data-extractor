using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GTA;
using GTA.Math;
using GTA.Native;

namespace gta5_vision_data_extractor
{
    public class GTAUtils
    {
        struct BoundingBox2D
        {
            public Vector2 min, max;
        }

        /// <summary>
        /// Convert 3D game world position to screen position range (-1,1) 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector2 Convert3DPostoScreenPos(Vector3 pos)
        {
            OutputArgument resX = new OutputArgument();
            OutputArgument resY = new OutputArgument();

            if (Function.Call<bool>(Hash._WORLD3D_TO_SCREEN2D,
                pos.X, pos.Y, pos.Z, resX, resY))
            {
                Vector2 ret;
                ret.X = resX.GetResult<float>();
                ret.Y = resY.GetResult<float>();
                return ret;
            }
            return new Vector2(-1f, -1f);
        }

        /// <summary>
        /// Compute a bounding box that is stored in memory (this is not just same with the entity appearance)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private BoundingBox2D ComputeBoundingBox(Entity entity)
        {
            var model = entity.Model;

            Vector3 bmin, bmax;
            model.GetDimensions(out bmin, out bmax);

            // 3D bb corners
            Vector3[] corners = new Vector3[]
            {
                bmin,
                new Vector3(bmin.X, bmin.Y, bmax.Z),
                new Vector3(bmin.X, bmax.Y, bmax.Z),
                new Vector3(bmin.X, bmax.Y, bmin.Z),
                new Vector3(bmax.X, bmin.Y, bmin.Z),
                new Vector3(bmax.X, bmax.Y, bmin.Z),
                new Vector3(bmax.X, bmin.Y, bmax.Z),
                bmax,
            };

            BoundingBox2D bb_ret = new BoundingBox2D();
            bb_ret.min = new Vector2(float.MaxValue, float.MaxValue);
            bb_ret.max = new Vector2(float.MinValue, float.MinValue);

            var center_pos = GTAUtils.Convert3DPostoScreenPos(entity.GetOffsetInWorldCoords(entity.Position));
            foreach (var corner in corners)
            {
                // get bb in 2d
                var c = entity.GetOffsetInWorldCoords(corner);
                var screen_pos = GTAUtils.Convert3DPostoScreenPos(c);
                if (screen_pos.X == -1f || screen_pos.Y == -1f)
                {
                    bb_ret.min.X = float.MaxValue;
                    bb_ret.max.X = float.MinValue;
                    bb_ret.min.Y = float.MaxValue;
                    bb_ret.max.Y = float.MinValue;
                    return bb_ret;
                }

                // update bb
                bb_ret.min.X = Math.Min(bb_ret.min.X, screen_pos.X);
                bb_ret.min.Y = Math.Min(bb_ret.min.Y, screen_pos.Y);
                bb_ret.max.X = Math.Max(bb_ret.max.X, screen_pos.X);
                bb_ret.max.Y = Math.Max(bb_ret.max.Y, screen_pos.Y);
            }

            return bb_ret;
        }
    }
}
