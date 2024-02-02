using Cargo.DeviceData.Models;

namespace Cargo.DeviceData.Interfaces
{
    public interface IDataConverter
    {
        public List<DeviceModel> Process();
    }
}
