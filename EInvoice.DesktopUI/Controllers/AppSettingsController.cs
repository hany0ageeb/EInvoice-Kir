using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using EInvoice.DesktopUI.ViewModel;
namespace EInvoice.DesktopUI.Controllers
{
    public static class AppSettingsController
    {
        public static Settings Index()
        {
            using(var reader = new StreamReader("appconfig.json"))
            {
                Settings settings = Settings;
                return settings;
            }
        }
        public static Settings Settings
        {
            get
            {
                using (var reader = new StreamReader("appconfig.json"))
                {
                    JsonSerializer serializer = JsonSerializer.CreateDefault();
                    serializer.Formatting = Formatting.Indented;
                    Settings settings = serializer.Deserialize<Settings>(new JsonTextReader(reader));
                    settings.Password = Encoding.UTF8.GetString(Convert.FromBase64String(settings.Password));
                    return settings;
                }
            }
            set
            {
                using (var writer = new StreamWriter("appconfig.json"))
                {
                    JsonSerializer serializer = JsonSerializer.CreateDefault();
                    serializer.Formatting = Formatting.Indented;
                    value.Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(value.Password));
                    serializer.Serialize(writer, value);
                }
            }
        }
    }
}
