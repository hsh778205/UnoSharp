using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoSharp
{
    class Rank
    {
        public DateTime _BeginTime,_EndTime;
        public int minute, second;

        public string _Bout;
        public void Start()
        {
            _BeginTime = DateTime.Now;
            
        }
        public void Finish()
        {
            _EndTime= DateTime.Now;
            long timeStamp = ((DateTimeOffset)_EndTime).ToUnixTimeSeconds()-((DateTimeOffset)_BeginTime).ToUnixTimeSeconds();
            minute= (int)timeStamp/60;
            second= (int)timeStamp % 60;
        }
    }
}
