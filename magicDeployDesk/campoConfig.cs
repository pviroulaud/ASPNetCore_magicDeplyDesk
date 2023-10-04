using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace magicDeployDesk
{
    public class campoConfig
    {
        public string nombre { get; set; }
        public string valor { get; set; }
    }
    public class configDeploy
    {
        public DateTime horaEjecucionProgramada { get; set; }
        public string nombre { get; set; }
        public List<campoConfig> configuraciones { get; set; }
    }
}
