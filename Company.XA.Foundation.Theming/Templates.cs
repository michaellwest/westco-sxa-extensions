﻿using Sitecore.Data;

namespace Company.XA.Foundation.Theming
{
    public class Templates
    {
        public struct PlainInclude
        {
            public static ID Id = ID.Parse("{FE373FC0-CD4D-4287-BE67-9C4C9C1E2A07}");

            public struct Fields
            {
                public static readonly ID Mode = new ID("{33E4C4EE-CA6B-4D32-8476-2769E0D98925}");
                public static readonly ID AssetUrl = new ID("{FEF3E6D6-18FA-4C01-84AF-6533872F89BE}");
                public static readonly ID SriHash = new ID("{6BEC0F1F-557A-4DE6-8859-05B569692D7C}");
                public static readonly ID Cors = new ID("{76F5998D-1442-4BC4-83B5-57DCDFD279BB}");
                public static readonly ID RawContent = new ID("{26F1DD9D-104F-4597-AC5A-EA5346C1372D}");
                public static readonly ID IsGlobalAsset = new ID("{64C5DBCD-1852-4A81-8712-D0386CBF7DBD}");
            }
        }

        public struct Page
        {
            public static ID Id = ID.Parse("{69ED42F9-3E4C-4C30-B119-18235FAE6947}");

            public struct Fields
            {
                public static readonly ID PageAssets = new ID("{72A00709-0572-4356-B2B5-F69A6A604D1A}");
            }
        }
    }
}
