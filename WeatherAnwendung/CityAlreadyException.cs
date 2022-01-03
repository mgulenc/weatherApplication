
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherAnwendung
{
    class CityAlreadyException : Exception
    {
        public override string Message => "Sie haben bereits diese Stadt hinzugefügt, bitte tragen Sie noch einen anderen Stadtnamen ein";
    }
}
