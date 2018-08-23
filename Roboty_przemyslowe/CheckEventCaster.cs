using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roboty_przemyslowe
{
    class CheckEventCaster
    {
        bool reseted;
        private UInt16 _Time;
        public UInt16 Time
        {
            set { _Time = value; }
        }

        private UInt16 _ResetTimerValue;
        
        public UInt16 ResetTimerValue
        {
            get { return _ResetTimerValue; }
            set { _ResetTimerValue = value; }
        }

        public delegate void EventDelegate();
        public EventDelegate CheckTick;
        public EventDelegate CheckTick2;

        /// <summary>
        /// Sets the time to cast the event, time in ms= ResetTime*100.
        /// </summary>
        /// <param name="ResetTime"></param>
        public CheckEventCaster(UInt16 ResetTime)
        {
            ResetTimerValue = ResetTime;
        }

        public async void StartTimer()
        {
            
            while (true)
            {
                reseted = false;
                _Time = ResetTimerValue;

                while (_Time > 0 && reseted == false)
                {
                    if (null != CheckTick2)
                        CheckTick2();
                    await Task.Delay(100);
                    _Time--;
                }

                if (CheckTick != null && reseted!=true)
                {
                    CheckTick();
                }
                await Task.Delay(10);
             }
        }

        public void TimerReset()
        {
            _Time = ResetTimerValue;
            reseted = true;
        }
        
    }
}