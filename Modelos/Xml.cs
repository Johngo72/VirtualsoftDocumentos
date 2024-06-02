using SicaVS.Objectos.XMLExtension;

namespace SicaVS.Modelos
{
    public class InvoiceType
    {
        public UBLExtensionType[] UBLExtensions;
        public UBLVersionIDType uBLVersionIDField;

        private IDType idField;
        private IssueDateType issueDateField;
        private DocumentCurrencyCodeType documentCurrencyCodeField;
        private InvoiceLineType[] invoiceLineField;

        private TotalFacturaType totalFacturaField;


        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public TotalFacturaType TotalFactura
        {
            get { return this.totalFacturaField; }
            set { this.totalFacturaField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public IDType ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }


        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public IssueDateType IssueDate
        {
            get { return this.issueDateField; }
            set { this.issueDateField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public DocumentCurrencyCodeType DocumentCurrencyCode
        {
            get { return this.documentCurrencyCodeField; }
            set { this.documentCurrencyCodeField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute("InvoiceLine", Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2")]
        public InvoiceLineType[] InvoiceLine
        {
            get { return this.invoiceLineField; }
            set { this.invoiceLineField = value; }
        }
    }

    public class IDType
    {
        [System.Xml.Serialization.XmlText]
        public string Value { get; set; }
    }

    public class IssueDateType
    {
        [System.Xml.Serialization.XmlText]
        public DateTime Value { get; set; }
    }

    public class DocumentCurrencyCodeType
    {
        [System.Xml.Serialization.XmlText]
        public string Value { get; set; }
    }

    public class InvoiceLineType
    {
        private IDType idField;
        private LineExtensionAmountType lineExtensionAmountField;

        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public IDType ID
        {
            get { return this.idField; }
            set { this.idField = value; }
        }

        [System.Xml.Serialization.XmlElementAttribute(Namespace = "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2")]
        public LineExtensionAmountType LineExtensionAmount
        {
            get { return this.lineExtensionAmountField; }
            set { this.lineExtensionAmountField = value; }
        }
    }

    public class LineExtensionAmountType
    {
        [System.Xml.Serialization.XmlText]
        public decimal Value { get; set; }
    }


    public class TotalFacturaType
    {
        [System.Xml.Serialization.XmlText]
        public decimal Value { get; set; }
    }
}
