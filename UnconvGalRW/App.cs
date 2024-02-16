using System.Collections;
using System.Globalization;
using System.Resources;

namespace UnconvGalRW
{
    public class App : IApp
    {
        public bool IsRunning { get; protected set; }

        protected Gallery? Gallery;

        public void Run(bool forceColdStartUp=false)
        {
            StartUp(forceColdStartUp);

            Gallery?.Run();
        }


        void StartUp(bool forceColdStartUp)
        {
            if (!IntegrityState || forceColdStartUp)
                ColdStartUp();

            Gallery = new(1920, 1080, "UGRW")
            {
                UpdateFrequency = 180
            };
        }

        void ColdStartUp()
        {
            Directory.CreateDirectory("Data");
            Directory.CreateDirectory("Data/Textures");
            Directory.CreateDirectory("Data/Shaders");
            ResourceManager MyResourceClass = new ResourceManager(typeof(Properties.Resources));

            ResourceSet? resourceSet = MyResourceClass.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            foreach (DictionaryEntry entry in resourceSet!)
            {
                string? resourceKey = entry.Key.ToString();
                object? resource = entry.Value;



                if (resourceKey?.Contains("Shader")??false)
                {
                    File.WriteAllBytes($"Data/Shaders/{resourceKey}.{resourceKey.Remove(4)}", (byte[])resource!);
                }

            }
        }

        public void Stop()
        {
            if (!IsRunning)
                throw new Exception("Unable to stop nonexistant Gallery.");
            IsRunning = false;  
            Gallery?.Close();
        }


        bool IntegrityState
        {
            get
            {
                if (!Directory.Exists("Data"))
                    return false;
                if (!Directory.Exists("Data/Textures"))
                    return false;
                if (!Directory.Exists("Data/Shaders"))
                    return false;
                if (!File.Exists("Data/Shaders/fragShader.frag"))
                    return false; 
                if (!File.Exists("Data/Shaders/vertShader.vert"))
                    return false;

                return true;
            }
        }
    }
}
