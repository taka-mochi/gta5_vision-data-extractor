using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace gta5_vision_data_extractor
{
    public enum SaveObjectType
    {
        PEDESTRIANS,
        VEHICLES,
        PED_AND_VEHICLES,
    };

    /// <summary>
    /// Serialized detection setting info
    /// </summary>
    [DataContract]
    public class ExportDetectionSettings
    {
        /// <summary>
        /// Output result directory
        /// </summary>
        [DataMember]
        public string OutputDirectory = "default_save_dir";

        /// <summary>
        /// Type of save objects
        /// </summary>
        [DataMember]
        public SaveObjectType SaveObjectType = SaveObjectType.PEDESTRIANS;

        /// <summary>
        /// Detection data is saved per this frame span
        /// </summary>
        [DataMember]
        public int SaveFrameSpan = 60;

        /// <summary>
        /// Find object max distance
        /// </summary>
        [DataMember]
        public float DetectDistance = 100.0f;

        /// <summary>
        /// Serialize to json file
        /// </summary>
        /// <param name="json_path"></param>
        public void SerializeToJson(string json_path)
        {
            var setting = new System.Runtime.Serialization.Json.DataContractJsonSerializerSettings();
            using (var fs = new FileStream(json_path, FileMode.OpenOrCreate))
            {
                fs.Seek(0, SeekOrigin.Begin);
                var serializer = new DataContractJsonSerializer(typeof(ExportDetectionSettings), setting);
                serializer.WriteObject(fs, this);
            }
        }

        /// <summary>
        /// Load Settings from serialized json file
        /// </summary>
        /// <param name="json_path"></param>
        /// <returns></returns>
        public static ExportDetectionSettings DeserializeFromJsonOrDefault(string json_path)
        {
            if (System.IO.File.Exists(json_path))
            {
                var serializer = new DataContractJsonSerializer(typeof(ExportDetectionSettings));
                using (var fs = new FileStream(json_path, FileMode.Open))
                {
                    var ret = serializer.ReadObject(fs) as ExportDetectionSettings;
                    return ret;
                }
            }
            else
            {
                // if file not exist, return default instance
                return new ExportDetectionSettings();
            }
        }
    }
}
