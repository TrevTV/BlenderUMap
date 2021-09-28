using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Objects.Core.Misc;
using Newtonsoft.Json;

namespace BlenderUMap
{
    internal class AESKeyInfo
    {
        [JsonIgnore] public FGuid FGuid => new(Guid);
        [JsonIgnore] public FAesKey FAes => new(Key);

        public string Guid = "00000000000000000000000000000000";
        public string Key = "0x0000000000000000000000000000000000000000000000000000000000000000";
    }
}
