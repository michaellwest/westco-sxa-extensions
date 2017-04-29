using Sitecore.Data;

namespace Westco.XA.Feature.Maps
{
    public class Templates
    {
        public struct StaticMap
        {
            public static ID Id = ID.Parse("{30B8B44C-6274-45E6-B2EC-0F16C1C02271}");

            public struct Fields
            {
                public static readonly ID Mode = new ID("{B38F60DA-3A94-4081-A8EE-C672F0BCC66C}");
                public static readonly ID Poi = new ID("{D6850EF8-D10B-43B3-95C0-3FF3AD82974C}");
                public static readonly ID Width = new ID("{9E2729D1-A77E-4BF0-95FC-316D0EB008D3}");
                public static readonly ID Height = new ID("{88D0A8A3-BA85-4EDE-B864-6C267EE09870}");
            }
        }
    }
}
