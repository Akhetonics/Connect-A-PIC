using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Grid
{
    public class LightManager
    {
        public event EventHandler<bool> OnLightSwitched;
        private bool isLightOn;
        public bool IsLightOn
        {
            get => isLightOn; set
            {
                isLightOn = value; OnLightSwitched?.Invoke(this, value);
            }
        }
    }
}
