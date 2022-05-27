using System;

namespace CriticalSlash
{
   [Serializable] 
    public class CriticalSlashSettings
    {
        public float WindowInCentiSeconds = 25f;
        public float RecoverySpeedIncreasePercent = 50f;
        public float DamageIncreasePercent = 50f;
    }
}