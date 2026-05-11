namespace OrangePi.Common.Services
{
    public class I2CDisplayLock: ResourceLock
    {
        protected override string LockFilePath => "/var/lock/i2c-display.lock";
    }
}
