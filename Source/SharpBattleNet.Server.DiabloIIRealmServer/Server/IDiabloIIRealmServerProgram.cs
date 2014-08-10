namespace SharpBattleNet.Server.DiabloIIRealmServer.Server
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    #endregion

    public interface IDiabloIIRealmServerProgram
    {
        void Start();
        void Stop();
    }
}
