using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentAPI.Infrastructure.Models
{
    public class ExportDocument
    {
        public class InitiateRequest
        {
            public int ExportColdFormOverlay => 1;

            public bool IsExportCold => false;

            public bool HideAnnotation => false;

            public List<Dictionary<string, int>> Documents { get; set; }

            public string PageRange { get; set; }

            public int PageNum => 0;

            public int PageVersionNum => 0;

            public string SubPageRange => null;

            public bool UsePDFFormat { get; set; }

            public bool ExportIndex => false;

            public bool ExportImage => false;

            public bool IsOpenDocument => true;

            public int? QueryContextId => null;

            public int? Idxes => null;
        }
    }
}
