using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;


namespace WeatherAnwendung
{
    [XmlInclude( typeof(WheatherData) )]

    public class WheatherData
    {
        public int cityID { get; set; }
        public string cityName { get; set; }
        public string countryName { get; set; }
        public double maxTemp { get; set; }
        public double minTemp { get; set; }
        public double aktuellTemp { get; set; }
        public string clouds { get; set; }
        public DateTime lastAbdate { get; set; }
        public string GetInfo { get=>cityName+"/"+countryName+"   Temparatur:"+ 
                                aktuellTemp+"   Min:"+ minTemp+"   Max:"+maxTemp+"   Zustand:"+clouds+
                                "   Letzte Aktualisierung:"+lastAbdate; }
    }
}
