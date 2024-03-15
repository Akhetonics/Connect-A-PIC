using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CAP_Core.Tools
{
    public interface ITool
    {
        public void Update();
        public void Initialize();
        public void Exit();
    }
}
