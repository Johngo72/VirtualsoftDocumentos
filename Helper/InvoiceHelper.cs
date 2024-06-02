using SicaVS.Modelos;
using SicaVS.Objectos;
using System.Data;
using System.IO.Compression;
using System.Reflection;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace SicaVS.Helper
{
    public class InvoiceHelper
    {
        public static string EMPRESA { get; set; } = "PRINCIPAL";
        private string NOMBREXML { get; set; }

        private string RUTAGUARDAR { get; set; } = $"D:\\FACTURAS\\{EMPRESA}\\FACTURAS\\";

        public Parametrizacion parametrizacion = new Parametrizacion();
        public void CrearFactura(InvoiceFact invoice)
        {
            InvoiceType Invoice = new InvoiceType();
            bool repetirFactura = true;
            var crearFactura = CreateInvoice(invoice.nfact, invoice, ref Invoice);
        }

        public bool CreateInvoice(int Num, InvoiceFact item, ref InvoiceType Invoice)
        {
            try
            {
                /*if (checkBox2.Checked && !Constantes.POSINT)
                {
                    if (Convert.ToDateTime(item.Fechaini.ToString()).Date != DateTime.Now.Date)
                    {
                        // Sql.ActualizarData("UPDATE `cartera - fact` SET Fechaini = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "'  WHERE Nfact = " + item.Nfact.ToString());
                        // Sql.ActualizarData("UPDATE `facturas` SET Fechaini = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' WHERE Nfact = " + item.Nfact.ToString());
                        item.Fechaini = DateTime.Now.Date.ToString("yyyy-MM-dd");

                    }
                }
                else if (checkBox2.Checked && Constantes.POSINT)
                {


                    if (Convert.ToDateTime(item.Fechaini.ToString()).Date != DateTime.Now.Date)
                    {
                        // Sql.ActualizarData("UPDATE `pos" + Constantes.NPosint + "-cartera - fact` SET Fechaini = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "', Fechafin = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' WHERE Nfact = " + item.Nfact.ToString() + " AND Forma = '" + Constantes.Forma + "'");
                        // Sql.ActualizarData("UPDATE `pos" + Constantes.NPosint + "-facturas` SET Fechaini = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "', Fechafin = '" + DateTime.Now.Date.ToString("yyyy-MM-dd") + "' WHERE Nfact = " + item.Nfact.ToString() + " AND Forma = '" + Constantes.Forma + "'");
                        item.Fechaini = DateTime.Now.Date.ToString("yyyy-MM-dd");
                        item.Fechafin = DateTime.Now.Date.ToString("yyyy-MM-dd");
                    }
                }*/

                bool Exportacion = false;

                int zonaResultado = 0;
                if (int.TryParse(item.Nzona.ToString(), out zonaResultado))
                {
                    if (item.Nzona.ToString() != null)
                    {
                        Exportacion = (item.Nzona.ToString() == "" ? false : (Convert.ToInt32(item.Nzona.ToString()) >= 319 ? true : false));
                    }
                    else
                    {
                        // richTextBox1.Text += "\r La factura N= " + item.nfact.ToString() + " no contiene Numero de Zona, por favor contactar con administrador del sistema";
                        return false;
                    }
                }
                else
                {
                    item.Nzona = "1";
                }

                List<string> retnames = new List<string>();

                Decimal Resultado1 = 0;
                Decimal Resultado2 = 0;
                Decimal Resultado3 = 0;

                if (Decimal.TryParse(item.Frtfte.ToString(), out Resultado1))
                {
                    if (Convert.ToDecimal(item.Frtfte.ToString()) > 0)
                    {
                        retnames.Add("Frtfte");
                    }
                }
                else
                {
                    item.Frtfte = "0";
                }

                if (item.Frtfteiva != null)
                {
                    if (Decimal.TryParse(item.Frtfteiva.ToString(), out Resultado2))
                    {
                        if (Convert.ToDecimal(item.Frtfteiva.ToString()) > 0)
                        {
                            retnames.Add("Frtfteiva");
                        }
                    }
                    else
                    {
                        item.Frtfteiva = "0";
                    }
                }


                /*if (Constantes.POSINT == false)
                {
                    if (Decimal.TryParse(item.FRTFTEICA.ToString(), out Resultado3))
                    {
                        if ((Objetos.Constantes.POSINT ? false : Convert.ToDecimal(item.FRTFTEICA.ToString()) > 0))
                        {
                            retnames.Add("FRTFTEICA");
                        }
                    }
                    else
                    {
                        item.FRTFTEICA = "0";
                    }
                }*/
                // Num += 990000000;

                DataTable detalles = new DataTable("detalles");
                DataTable detallesesp = new DataTable("detallesesp");
                if (item.factind != 1)
                {
                    detalles = item.Datos.detalles;
                    item.Datos.detalles.Select("nfact = '" + Num + "'").CopyToDataTable(detalles, LoadOption.OverwriteChanges);
                }

                List<string> lineasadicionales = new List<string>();


                if (Decimal.TryParse(item.fletes.ToString(), out Resultado3))
                {
                    if (Convert.ToDecimal(item.fletes.ToString()) > 0)
                    {
                        lineasadicionales.Add("fletes");
                        if (Exportacion)
                        {
                            item.fletes = Convert.ToDecimal(item.fletes.ToString()) * Convert.ToDecimal(item.trm.ToString());
                        }
                    }
                }
                else
                {
                    item.fletes = 0;
                }

                if (Convert.ToDecimal(item.seguro.ToString()) > 0)
                {
                    lineasadicionales.Add("seguro");
                    if (Exportacion)
                    {
                        item.seguro = Convert.ToDecimal(item.seguro.ToString()) * Convert.ToDecimal(item.trm.ToString());
                    }
                }
                else
                {
                    item.seguro = 0;

                }


                if (Decimal.TryParse(item.OtrosGastos.ToString(), out Resultado3))
                {
                    if (Convert.ToDecimal(item.OtrosGastos.ToString()) > 0)
                    {
                        lineasadicionales.Add("otrosgastos");
                        if (Exportacion)
                        {
                            item.OtrosGastos = Convert.ToDecimal(item.OtrosGastos.ToString()) * Convert.ToDecimal(item.trm.ToString());
                        }
                    }
                }
                else
                {
                    item.OtrosGastos = 0;
                }
                // Xml Namescpaces
                var namespaces = new XmlSerializerNamespaces();
                namespaces.Add("cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2");
                namespaces.Add("ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2");
                namespaces.Add("sts", "dian:gov:co:facturaelectronica:Structures-2-1");
                namespaces.Add("ds", "http://www.w3.org/2000/09/xmldsig#");
                namespaces.Add("xades", "http://uri.etsi.org/01903/v1.3.2#");
                namespaces.Add("cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2");
                // Memory Instances
                //X509DataType x509Data = new X509DataType();
                // x509Data.ItemsElementName = new ItemsChoiceType[1];
                // x509Data.Items = new object[1];
                // QualifyingPropertiesType qualifyingProperties = new QualifyingPropertiesType();
                //interfazDian.Invoice_MemoryInstance(ref Invoice, ((factind == 1 ? (Datos.detallesIND.Rows.Count == 0 ? Datos.detalles_ESPECIALIND.Rows.Count : Datos.detallesIND.Rows.Count) : (detalles.Rows.Count == 0 ? detallesesp.Rows.Count : detalles.Rows.Count))), true, retnames, "COP", lineasadicionales, checkBox1.Checked);
                // Variables
                /*Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.InvoiceControl.InvoiceAuthorization.Value = "InvoiceAuthorization";
                Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.InvoiceControl.AuthorizationPeriod.StartDate.Value = DateTime.Now.AddDays(1);
                Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.InvoiceControl.AuthorizationPeriod.EndDate.Value = DateTime.Now.AddDays(2);
                Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.InvoiceControl.AuthorizedInvoices.Prefix.Value = "PREF01";
                Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.InvoiceControl.AuthorizedInvoices.From.Value = "PREF01";
                Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.InvoiceControl.AuthorizedInvoices.To.Value = "PREF01TO";
                Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.InvoiceSource.IdentificationCode.Value = "038745145";
                Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.SoftwareProvider.ProviderID.Value = "900-1545-112.06";
                Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.SoftwareProvider.SoftwareID.Value = "01";*/
                // Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.SoftwareSecurityCode.Value = interfazDian.ComputeSha384Hash(parametrizacion.SoftwareID + parametrizacion.Pin + item.PrefijoFact.ToString() + Num.ToString());

                // Inicia Cuerpo de factura
                // Datos de la Factura
                //Invoice.UBLVersionID.Value = "UBL 2.1";
                //Invoice.CustomizationID.Value = ("10");
                //Invoice.ProfileID.Value = "DIAN 2.1: Factura Electrónica de Venta";
                // Invoice.ProfileExecutionID.Value = Facturaelectronicadian.Properties.Settings.Default.ambiente.ToLower() == "pruebas" ? "2" : "1";
                Invoice.ID.Value = item.PrefijoFact.ToString() + Num.ToString();
                //Invoice.IssueDate.Value = DateTime.Now;
                //Invoice.IssueTime.Value = DateTime.Now;
                Invoice.IssueDate.Value = new DateTime(Convert.ToDateTime(item.Fechaini.ToString()).Year, Convert.ToDateTime(item.Fechaini.ToString()).Month, Convert.ToDateTime(item.Fechaini.ToString()).Day, Convert.ToDateTime(item.UFECHA.ToString()).Hour, Convert.ToDateTime(item.UFECHA.ToString()).Minute, Convert.ToDateTime(item.UFECHA.ToString()).Second);
                //Invoice.IssueTime.Value = Convert.ToDateTime(item.UFECHA.ToString()).Hour.ToString("00") + ":" + Convert.ToDateTime(item.UFECHA.ToString()).Minute.ToString("00") + ":" + Convert.ToDateTime(item.UFECHA.ToString()).Second.ToString("00") + "-05:00";
                // Invoice.InvoiceTypeCode.Value = (checkBox1.Checked == true ? "03" : (Exportacion ? "02" : "01"));
                Invoice.DocumentCurrencyCode.Value = parametrizacion.DocumentCurrencyCode;

                /*if (Invoice.InvoiceTypeCode.Value == "03")
                {
                    Invoice.AdditionalDocumentReference = new DocumentReferenceType[1];
                    Invoice.AdditionalDocumentReference[0] = new DocumentReferenceType();
                    Invoice.AdditionalDocumentReference[0].ID = new IDType();
                    Invoice.AdditionalDocumentReference[0].ID.Value = item.PrefijoFact.ToString() + Num.ToString();
                    Invoice.AdditionalDocumentReference[0].IssueDate = new IssueDateType();
                    Invoice.AdditionalDocumentReference[0].IssueDate.Value = new DateTime(Convert.ToDateTime(item.Fechaini.ToString()).Year, Convert.ToDateTime(item.Fechaini.ToString()).Month, Convert.ToDateTime(item.Fechaini.ToString()).Day, Convert.ToDateTime(item.UFECHA.ToString()).Hour, Convert.ToDateTime(item.UFECHA.ToString()).Minute, Convert.ToDateTime(item.UFECHA.ToString()).Second);
                    Invoice.AdditionalDocumentReference[0].DocumentTypeCode = new DocumentTypeCodeType();
                    Invoice.AdditionalDocumentReference[0].DocumentTypeCode.Value = "R1";
                }*/
                /* Cuenta Facturador
                // Datos del Facturador
                Invoice.AccountingSupplierParty.AdditionalAccountID[0].Value = "1";
                // Invoice.AccountingSupplierParty.Party.PartyName[0].Name.Value = Datos.busint.Rows[0].Empresa.ToString();

                Invoice.AccountingSupplierParty.Party.PhysicalLocation.Address.ID.Value = parametrizacion.AddressID;
                Invoice.AccountingSupplierParty.Party.PhysicalLocation.Address.CityName.Value = parametrizacion.CityName;
                Invoice.AccountingSupplierParty.Party.PhysicalLocation.Address.CountrySubentity.Value = parametrizacion.Department;
                Invoice.AccountingSupplierParty.Party.PhysicalLocation.Address.CountrySubentityCode.Value = parametrizacion.CountrySubentityCode;
                //Invoice.AccountingSupplierParty.Party.PhysicalLocation.Address.AddressLine[0].Line.Value = Datos.busint.Rows[0].EDireccion.ToString();
                Invoice.AccountingSupplierParty.Party.PhysicalLocation.Address.Country.Name.Value = "Colombia";
                Invoice.AccountingSupplierParty.Party.PhysicalLocation.Address.Country.IdentificationCode.Value = "CO";
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].RegistrationName.Value = Datos.busint.Rows[0].Empresa.ToString();
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].CompanyID.Value = parametrizacion.Nit;
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].TaxLevelCode.Value = parametrizacion.TaxLevelCode;
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].TaxScheme.Name.Value = "IVA";
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].RegistrationAddress.ID.Value = parametrizacion.AddressID;
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].RegistrationAddress.CityName.Value = parametrizacion.CityName;
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].RegistrationAddress.CountrySubentity.Value = parametrizacion.Department;
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].RegistrationAddress.CountrySubentityCode.Value = parametrizacion.CountrySubentityCode;
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].RegistrationAddress.AddressLine[0].Line.Value = Datos.busint.Rows[0].EDireccion.ToString();
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].RegistrationAddress.Country.IdentificationCode.Value = "CO";
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].RegistrationAddress.Country.Name.Value = "Colombia";
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].TaxScheme.ID.Value = "01";
                Invoice.AccountingSupplierParty.Party.PartyTaxScheme[0].TaxScheme.Name.Value = "IVA";
                //Invoice.AccountingSupplierParty.Party.PartyLegalEntity[0].RegistrationName.Value = Datos.busint.Rows[0].Empresa.ToString();
                Invoice.AccountingSupplierParty.Party.PartyLegalEntity[0].CompanyID.Value = parametrizacion.Nit;
                Invoice.AccountingSupplierParty.Party.PartyLegalEntity[0].CorporateRegistrationScheme.ID.Value = item.PrefijoFact.ToString();
                Invoice.AccountingSupplierParty.Party.PartyLegalEntity[0].CorporateRegistrationScheme.Name.Value = parametrizacion.WebSite;


                //Invoice.AccountingSupplierParty.Party.Contact.Name.Value = Datos.busint.Rows[0].Empresa.ToString();
                // Invoice.AccountingSupplierParty.Party.Contact.Telephone.Value = Datos.busint.Rows[0].ETelefono.ToString();
                // Invoice.AccountingSupplierParty.Party.Contact.ElectronicMail.Value = Datos.busint.Rows[0].EEmail.ToString().Replace("E-mail:", "").Replace("", "").Trim();

                Invoice.OrderReference.ID.Value = item.nped.ToString();
                Invoice.OrderReference.IssueDate.Value = Convert.ToDateTime(item.Fechaini.ToString());
                // Datos del Cliente

                Invoice.AccountingCustomerParty.AdditionalAccountID[0].Value = item.Naturaleza.ToString();
                // Invoice.AccountingCustomerParty.Party.PartyName[0].Name.Value = item.NombreFact.ToString();
                if (item.COD.ToString() == "CO")
                {
                    Invoice.AccountingCustomerParty.Party.PhysicalLocation.Address.ID.Value = item.CodDIAN.ToString();
                    Invoice.AccountingCustomerParty.Party.PhysicalLocation.Address.CountrySubentityCode.Value = item.CodDep.ToString();
                }
                Invoice.AccountingCustomerParty.Party.PhysicalLocation.Address.CityName.Value = item.nomciuddian.ToString();
                Invoice.AccountingCustomerParty.Party.PhysicalLocation.Address.CountrySubentity.Value = item.nomdptodian.ToString();
                Invoice.AccountingCustomerParty.Party.PhysicalLocation.Address.AddressLine[0].Line.Value = item.Direccion.ToString();
                Invoice.AccountingCustomerParty.Party.PhysicalLocation.Address.Country.Name.Value = item.nompais.ToString();
                Invoice.AccountingCustomerParty.Party.PhysicalLocation.Address.Country.IdentificationCode.Value = item.COD.ToString();
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].RegistrationName.Value = item.NombreFact.ToString().Replace("&", "&amp;");
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].CompanyID.Value = item.Nitocc.ToString();
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].CompanyID.schemeID = item.DV.ToString() == "" ? "0" : item.DV.ToString();
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].TaxLevelCode.Value = item.RespFiscal.ToString() == "" ? "R-99-PN" : item.RespFiscal.ToString();
                if (item.COD.ToString() == "CO")
                {
                    Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].RegistrationAddress.ID.Value = item.CodDIAN.ToString();
                    Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].RegistrationAddress.CountrySubentityCode.Value = item.CodDep.ToString();
                }
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].RegistrationAddress.CityName.Value = item.nomciuddian.ToString();
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].RegistrationAddress.CountrySubentity.Value = item.nomdptodian.ToString();
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].RegistrationAddress.AddressLine[0].Line.Value = item.Direccion.ToString();
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].RegistrationAddress.Country.IdentificationCode.Value = item.COD.ToString();
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].RegistrationAddress.Country.Name.Value = item.nompais.ToString();
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].TaxScheme.ID.Value = "01";
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].TaxScheme.Name.Value = "IVA";
                Invoice.AccountingCustomerParty.Party.PartyLegalEntity[0].RegistrationName.Value = item.NombreFact.ToString();
                Invoice.AccountingCustomerParty.Party.PartyLegalEntity[0].CompanyID.Value = item.Nitocc.ToString().Trim();
                Invoice.AccountingCustomerParty.Party.PartyLegalEntity[0].CompanyID.schemeID = item.DV.ToString() == "" ? "0" : item.DV.ToString();
                Invoice.AccountingCustomerParty.Party.PartyLegalEntity[0].CorporateRegistrationScheme.Name.Value = "0";
                Invoice.AccountingCustomerParty.Party.Contact.Name.Value = item.Nombre.ToString();
                Invoice.AccountingCustomerParty.Party.Contact.Telephone.Value = item.Telefono.ToString();
                Invoice.AccountingCustomerParty.Party.Contact.ElectronicMail.Value = item.Email.ToString().Replace("", "");
                string schemename = "NIT";
                string schemeid = "31";
                switch (item.CN.ToString())
                {
                    case "C":
                        schemename = "Cédula de ciudadanía";
                        schemeid = "13";
                        break;
                    case "N":
                        schemename = "NIT";
                        schemeid = "31";
                        break;
                    case "CE":
                        schemename = "Cédula de extranjería";
                        schemeid = "22";
                        break;

                    case "TI":
                        schemename = "Tarjeta de identidad";
                        schemeid = "12";
                        break;
                    case "P":
                        schemename = "Pasaporte";
                        schemeid = "41";
                        break;
                    case "R":
                        schemename = "NIT de otro país";
                        schemeid = "50";
                        break;
                    case "RC":
                        schemename = "Registro civil";
                        schemeid = "11";
                        break;
                    case "TE":
                        schemename = "Tarjeta de extranjería";
                        schemeid = "21";
                        break;
                    case "DE":
                        schemename = "Documento de identificación extranjero";
                        schemeid = "42";
                        break;
                    case "NP":
                        schemename = "NIT de otro país";
                        schemeid = "50";
                        break;
                    case "BU":
                        schemename = "NUIP *";
                        schemeid = "91";
                        break;
                    default:
                        break;
                }
                Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].CompanyID.schemeName = schemeid;
                Invoice.AccountingCustomerParty.Party.PartyLegalEntity[0].CompanyID.schemeName = schemeid;

                if (Invoice.AccountingCustomerParty.AdditionalAccountID[0].Value == "2")
                {
                    Invoice.AccountingCustomerParty.Party.PartyIdentification = new PartyIdentificationType[1];
                    Invoice.AccountingCustomerParty.Party.PartyIdentification[0] = new PartyIdentificationType();
                    Invoice.AccountingCustomerParty.Party.PartyIdentification[0].ID = new IDType();
                    Invoice.AccountingCustomerParty.Party.PartyIdentification[0].ID.Value = item.Nitocc.ToString();
                    Invoice.AccountingCustomerParty.Party.PartyIdentification[0].ID.schemeName = schemename;
                    Invoice.AccountingCustomerParty.Party.PartyIdentification[0].ID.schemeID = schemeid;

                }
                */
                //Opcional Grupo de campos para información  relacionadas con el pago del documento. 
                /*Invoice.PaymentMeans[0].ID.Value = Convert.ToDateTime(item.Fechaini.ToString()) == Convert.ToDateTime(item.Fechafin.ToString()) ? "1" : "2";
                Invoice.PaymentMeans[0].PaymentMeansCode.Value = "ZZZ";
                Invoice.PaymentMeans[0].PaymentDueDate.Value = Convert.ToDateTime(item.Fechafin.ToString());
                Invoice.PaymentMeans[0].PaymentID[0].Value = "ZZZ";

                if (Exportacion)
                {
                    Invoice.DeliveryTerms = new DeliveryTermsType();
                    Invoice.DeliveryTerms.SpecialTerms = new SpecialTermsType[1];
                    Invoice.DeliveryTerms.SpecialTerms[0] = new SpecialTermsType();
                    Invoice.DeliveryTerms.LossRiskResponsibilityCode = new LossRiskResponsibilityCodeType();
                    //Invoice.DeliveryTerms.LossRisk = new LossRiskType[1];
                    //Invoice.DeliveryTerms.LossRisk[0] = new LossRiskType();


                    Invoice.PaymentExchangeRate = new ExchangeRateType();
                    Invoice.PaymentExchangeRate.SourceCurrencyCode = new SourceCurrencyCodeType();
                    Invoice.PaymentExchangeRate.SourceCurrencyBaseRate = new SourceCurrencyBaseRateType();
                    Invoice.PaymentExchangeRate.TargetCurrencyCode = new TargetCurrencyCodeType();
                    Invoice.PaymentExchangeRate.TargetCurrencyBaseRate = new TargetCurrencyBaseRateType();
                    Invoice.PaymentExchangeRate.CalculationRate = new CalculationRateType();
                    Invoice.PaymentExchangeRate.Date = new DateType1();

                    Invoice.DeliveryTerms.SpecialTerms[0].Value = "Portes Pagados";
                    Invoice.DeliveryTerms.LossRiskResponsibilityCode.Value = "CFR";

                    if (Convert.ToDecimal(item.trm.ToString()) > 0)
                    {
                        Invoice.PaymentExchangeRate.SourceCurrencyCode.Value = "USD";
                        Invoice.PaymentExchangeRate.SourceCurrencyBaseRate.Value = 1.00M;
                        Invoice.PaymentExchangeRate.TargetCurrencyCode.Value = "COP";
                        Invoice.PaymentExchangeRate.TargetCurrencyBaseRate.Value = 1.00M;
                        Invoice.PaymentExchangeRate.CalculationRate.Value = Convert.ToDecimal(item.trm.ToString());
                        Invoice.PaymentExchangeRate.Date.Value = parametrizacion.Date;

                    }

                }
                */
                //Opcional Grupo de campos para información  relacionadas con un anticipo
                //Invoice.PrepaidPayment[0].ID.Value = "SFR3123856";
                //Invoice.PrepaidPayment[0].InstructionID.Value = "Prepago recibido";
                //Invoice.PrepaidPayment[0].PaidAmount.Value = 1000.00M;
                //Invoice.PrepaidPayment[0].ReceivedDate.Value = DateTime.Now;
                //Invoice.PrepaidPayment[0].PaidDate.Value = DateTime.Now;


                //Lineas de factura

                decimal BaseGrabableCalc = 0;
                decimal BaseGrabableCalcConsumo = 0;
                decimal valorbruto = 0;

                int index = 0;
                int lineas = 1;
                bool checkdetalles = false;
                // DETALLE FACTURA
                /*foreach (DataRow detalle in ((factind == 1 ? (Datos.detallesIND.Rows.Count == 0 ? Datos.detalles_ESPECIALIND.Rows : Datos.detallesIND.Rows) : (detalles.Rows.Count == 0 ? detallesesp.Rows : detalles.Rows))))
                {
                    if (Exportacion)
                    {
                        detalle.precio = Convert.ToDecimal(detalle.precio.ToString()) * Invoice.PaymentExchangeRate.CalculationRate.Value;
                        Invoice.InvoiceLine[index].Item.StandardItemIdentification = new ItemIdentificationType();
                        Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID = new IDType();
                        Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeID = "020";
                        Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeName = "Partida Arancelarias";
                        Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeAgencyID = "195";
                    }

                    checkdetalles = true;

                    bool Descuento = false;
                    if (Convert.ToDecimal(((DataRow)detalle).Desc.ToString()) > 0)
                    {
                        Descuento = true;
                        //Opcional Grupo de campos para información relacionadas con un cargo o un descuento
                        interfazDian.AddAllowanceCharge(ref Invoice, index, "COP");

                        Invoice.InvoiceLine[index].AllowanceCharge[0].ID.Value = "1";
                        Invoice.InvoiceLine[index].AllowanceCharge[0].AllowanceChargeReason[0].Value = "Descuento";
                        Invoice.InvoiceLine[index].AllowanceCharge[0].MultiplierFactorNumeric.Value = interfazDian.EFRound(Convert.ToDecimal(((DataRow)detalle).Desc.ToString()));
                        Invoice.InvoiceLine[index].AllowanceCharge[0].BaseAmount.Value = interfazDian.EFRound(Convert.ToDecimal(((DataRow)detalle).precio.ToString()) * Convert.ToDecimal(((DataRow)detalle).Cantidad.ToString()));
                        Invoice.InvoiceLine[index].AllowanceCharge[0].Amount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].AllowanceCharge[0].BaseAmount.Value * (Invoice.InvoiceLine[index].AllowanceCharge[0].MultiplierFactorNumeric.Value / 100));

                    }


                    Invoice.InvoiceLine[index].ID.Value = lineas.ToString();
                    Invoice.InvoiceLine[index].InvoicedQuantity.unitCode = Exportacion ? "NIU" : "94";
                    Invoice.InvoiceLine[index].Price.BaseQuantity.unitCode = Exportacion ? "NIU" : "94";
                    Invoice.InvoiceLine[index].InvoicedQuantity.Value = Convert.ToDecimal(((DataRow)detalle).Cantidad.ToString());
                    Invoice.InvoiceLine[index].LineExtensionAmount.Value = interfazDian.EFRound((Descuento ? ((Exportacion ? Convert.ToDecimal(((DataRow)detalle).precio.ToString()) * Convert.ToDecimal(((DataRow)detalle).Cantidad.ToString()) : Convert.ToDecimal(((DataRow)detalle).precio.ToString()) * Convert.ToDecimal(((DataRow)detalle).Cantidad.ToString()))) - (Invoice.InvoiceLine[index].AllowanceCharge[0].Amount.Value) : (Exportacion ? Convert.ToDecimal(((DataRow)detalle).precio.ToString()) * Convert.ToDecimal(((DataRow)detalle).Cantidad.ToString()) : Convert.ToDecimal(((DataRow)detalle).precio.ToString()) * Convert.ToDecimal(((DataRow)detalle).Cantidad.ToString()))));

                    if (Objetos.Constantes.POSINT && Facturaelectronicadian.Properties.Settings.Default.EMPRESA != "BUSINT")
                    {
                        // Calculo de Impuesto IVA por linea
                        Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value = (Convert.ToDecimal(((DataRow)detalle).I.ToString()) > 0 ? Math.Round(interfazDian.EFRound(Invoice.InvoiceLine[index].LineExtensionAmount.Value / Convert.ToDecimal("1," + Convert.ToDecimal(((DataRow)detalle).I.ToString()))), 2) + 00M : 0.00M);
                        Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(interfazDian.EFRound(Convert.ToDecimal(((DataRow)detalle).I.ToString()) + 0.00m).ToString("0.00"));
                        BaseGrabableCalc += (Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value : 0);
                        Invoice.InvoiceLine[index].LineExtensionAmount.Value = (Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value : Invoice.InvoiceLine[index].LineExtensionAmount.Value);


                        // Calculo de Impuesto al Consumo por linea
                        if (Convert.ToDecimal(((DataRow)detalle).PivaC.ToString()) > 0)
                        {
                            Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value = (Convert.ToDecimal(((DataRow)detalle).PivaC.ToString()) > 0 ? Math.Round(interfazDian.EFRound(Invoice.InvoiceLine[index].LineExtensionAmount.Value / ((Convert.ToDecimal(((DataRow)detalle).PivaC.ToString()) / 100m) + 1m)), 2) + 00M : 0.00M);
                            Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(interfazDian.EFRound(Convert.ToDecimal(((DataRow)detalle).PivaC.ToString()) + 0.00m).ToString("0.00"));
                            // BaseGrabableCalc += (Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value : 0);
                            BaseGrabableCalcConsumo += (Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value : 0);
                            Invoice.InvoiceLine[index].LineExtensionAmount.Value = (Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value : Invoice.InvoiceLine[index].LineExtensionAmount.Value);
                        }
                        else
                        {
                            Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value = 0;
                            Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value = 0;
                        }

                    }
                    else
                    {
                        Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value = (Convert.ToDecimal(item.I.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0.00M);
                        Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(interfazDian.EFRound(Convert.ToDecimal(item.I.ToString()) + 0.00m).ToString("0.00"));
                        BaseGrabableCalc += Invoice.InvoiceLine[index].LineExtensionAmount.Value;
                    }

                    valorbruto += Invoice.InvoiceLine[index].LineExtensionAmount.Value;

                    Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value * ((Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value : 0) / 100));
                    Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "01";
                    Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "IVA";
                    Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value = Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxAmount.Value;
                    Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount = new RoundingAmountType();
                    Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount.currencyID = Invoice.DocumentCurrencyCode.Value;
                    Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount.Value = 0.00M;


                    Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value * ((Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value : 0) / 100));
                    Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "04";
                    Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "INC";
                    Invoice.InvoiceLine[index].TaxTotal[2].TaxAmount.Value = Invoice.InvoiceLine[index].TaxTotal[2].TaxSubtotal[0].TaxAmount.Value;
                    Invoice.InvoiceLine[index].TaxTotal[2].RoundingAmount = new RoundingAmountType();
                    Invoice.InvoiceLine[index].TaxTotal[2].RoundingAmount.currencyID = Invoice.DocumentCurrencyCode.Value;
                    Invoice.InvoiceLine[index].TaxTotal[2].RoundingAmount.Value = 0.00M;

                    //Invoice.InvoiceLine[index].TaxTotal[1].TaxSubtotal[0].TaxableAmount.Value = 0.00M;
                    //Invoice.InvoiceLine[index].TaxTotal[1].TaxSubtotal[0].TaxCategory.Percent.Value = 0.00M;
                    //Invoice.InvoiceLine[index].TaxTotal[1].TaxSubtotal[0].TaxAmount.Value = 0.00M;
                    //Invoice.InvoiceLine[index].TaxTotal[1].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "03";
                    //Invoice.InvoiceLine[index].TaxTotal[1].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "ICA";
                    //Invoice.InvoiceLine[index].TaxTotal[1].TaxAmount.Value = 0.00M;



                    Invoice.InvoiceLine[index].TaxTotal[1] = null;


                    if (Invoice.InvoiceLine[index].TaxTotal[2].TaxAmount.Value == 0 && Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value == 0)
                    {
                        Invoice.InvoiceLine[index].TaxTotal = null;
                    }
                    else
                    {
                        if (Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value == 0)
                        {
                            Invoice.InvoiceLine[index].TaxTotal[0] = null;

                        }

                        if (Invoice.InvoiceLine[index].TaxTotal[2].TaxAmount.Value == 0)
                        {
                            Invoice.InvoiceLine[index].TaxTotal[2] = null;
                        }
                    }


                    if (Invoice.InvoiceLine[index].LineExtensionAmount.Value == 0.00M)
                    {
                        Invoice.InvoiceLine[index].PricingReference = new PricingReferenceType();
                        Invoice.InvoiceLine[index].PricingReference.AlternativeConditionPrice = new PriceType[1];
                        Invoice.InvoiceLine[index].PricingReference.AlternativeConditionPrice[0] = new PriceType();
                        Invoice.InvoiceLine[index].PricingReference.AlternativeConditionPrice[0].PriceTypeCode = new PriceTypeCodeType();
                        Invoice.InvoiceLine[index].PricingReference.AlternativeConditionPrice[0].PriceTypeCode.Value = "01";
                        Invoice.InvoiceLine[index].PricingReference.AlternativeConditionPrice[0].PriceAmount = new PriceAmountType();
                        Invoice.InvoiceLine[index].PricingReference.AlternativeConditionPrice[0].PriceAmount.currencyID = "COP";
                        Invoice.InvoiceLine[index].PricingReference.AlternativeConditionPrice[0].PriceAmount.Value = interfazDian.EFRound((Exportacion ? Convert.ToDecimal(((DataRow)detalle).precio.ToString()) : Math.Round(Convert.ToDecimal(((DataRow)detalle).precio.ToString()), 0)));

                    }

                    int indexretdet = 0;
                    foreach (var retendetalles in retnames)
                    {
                        switch (retendetalles)
                        {
                            case "Frtfte":
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxableAmount.Value = interfazDian.EFRound((Convert.ToDecimal(item.RT.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0.00M));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(interfazDian.EFRound(Convert.ToDecimal(item.RT.ToString())).ToString("0.00"));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxableAmount.Value * ((Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value : 0) / 100));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "06";
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "ReteRenta";
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxAmount.Value = Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxAmount.Value;
                                break;
                            case "Frtfteiva":
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxableAmount.Value = interfazDian.EFRound((Convert.ToDecimal(item.RTI.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0.00M));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(interfazDian.EFRound(Convert.ToDecimal(item.RTI.ToString())).ToString("0.00"));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxableAmount.Value * ((Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value : 0) / 100));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "05";
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "ReteIVA";
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxAmount.Value = Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxAmount.Value;

                                break;
                            case "FRTFTEICA":
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxableAmount.Value = interfazDian.EFRound((Convert.ToDecimal(item.RTICA.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0.00M));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(interfazDian.EFRound(Convert.ToDecimal(item.RTICA.ToString())).ToString("0.00"));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxableAmount.Value * ((Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.Percent.Value : 0) / 100));
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "07";
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "ReteICA";
                                Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxAmount.Value = Invoice.InvoiceLine[index].WithholdingTaxTotal[indexretdet].TaxSubtotal[0].TaxAmount.Value;

                                break;
                            default:
                                break;
                        }
                        indexretdet += 1;
                    }

                    Invoice.InvoiceLine[index].Item.Description[0].Value = ((DataRow)detalle).Descripcion.ToString() + " " + ((DataRow)detalle).PintaImp.ToString();
                    Invoice.InvoiceLine[index].Price.PriceAmount.Value = interfazDian.EFRound((Exportacion ? Convert.ToDecimal(((DataRow)detalle).precio.ToString()) : Math.Round(Convert.ToDecimal(((DataRow)detalle).precio.ToString()), 0)));
                    Invoice.InvoiceLine[index].Price.BaseQuantity.Value = Convert.ToDecimal(((DataRow)detalle).Cantidad.ToString());

                    if (Exportacion)
                    {
                        Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.Value = "0101";
                    }

                    Invoice.InvoiceLine[index].Item.StandardItemIdentification = new ItemIdentificationType();
                    Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID = new IDType();
                    Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.Value = ((DataRow)detalle).Ref.ToString();
                    Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeID = "999";
                    Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeName = "Estándar de adopción del contribuyente";
                    index += 1;
                    lineas += 1;
                }*/

                // detalles FACTURA
                // Flete#
                /*
                foreach (var lineasad in lineasadicionales)
                {
                    switch (lineasad)
                    {
                        case "fletes":
                            Invoice.InvoiceLine[index].ID.Value = lineas.ToString();
                            Invoice.InvoiceLine[index].InvoicedQuantity.unitCode = Exportacion ? "NIU" : "94";
                            Invoice.InvoiceLine[index].Price.BaseQuantity.unitCode = Exportacion ? "NIU" : "94";
                            Invoice.InvoiceLine[index].InvoicedQuantity.Value = 1;
                            Invoice.InvoiceLine[index].LineExtensionAmount.Value = interfazDian.EFRound(Convert.ToDecimal(item.fletes.ToString()));
                            BaseGrabableCalc += (Convert.ToDecimal(item.I.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value = (Convert.ToDecimal(item.I.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0.00M);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value = interfazDian.EFRound(Convert.ToDecimal(item.I.ToString()) + 0.00m);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value * ((Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value : 0) / 100));
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "01";
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "IVA";
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value = Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxAmount.Value;
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount = new RoundingAmountType();
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount.currencyID = Invoice.DocumentCurrencyCode.Value;
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount.Value = 0.00M;
                            Invoice.InvoiceLine[index].TaxTotal[1] = null;
                            Invoice.InvoiceLine[index].TaxTotal[2] = null;

                            Invoice.InvoiceLine[index].Item.Description[0].Value = "Fletes";
                            Invoice.InvoiceLine[index].Price.PriceAmount.Value = Invoice.InvoiceLine[index].LineExtensionAmount.Value;
                            Invoice.InvoiceLine[index].Price.BaseQuantity.Value = 1;

                            Invoice.InvoiceLine[index].Item.StandardItemIdentification = new ItemIdentificationType();
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID = new IDType();
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.Value = "FLETE";
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeID = "999";
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeName = "Estándar de adopción del contribuyente";


                            if (Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value == 0)
                            {
                                Invoice.InvoiceLine[index].TaxTotal = null;

                            }
                            valorbruto += Invoice.InvoiceLine[index].LineExtensionAmount.Value;

                            break;
                        case "seguro":
                            Invoice.InvoiceLine[index].ID.Value = lineas.ToString();
                            Invoice.InvoiceLine[index].InvoicedQuantity.unitCode = Exportacion ? "NIU" : "94";
                            Invoice.InvoiceLine[index].Price.BaseQuantity.unitCode = Exportacion ? "NIU" : "94";
                            Invoice.InvoiceLine[index].InvoicedQuantity.Value = 1;
                            Invoice.InvoiceLine[index].LineExtensionAmount.Value = interfazDian.EFRound(Convert.ToDecimal(item.seguro.ToString()));
                            BaseGrabableCalc += (Convert.ToDecimal(item.I.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value = (Convert.ToDecimal(item.I.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0.00M);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value = interfazDian.EFRound(Convert.ToDecimal(item.I.ToString()) + 0.00m);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value * ((Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value : 0) / 100));
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "01";
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "IVA";
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value = Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxAmount.Value;
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount = new RoundingAmountType();
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount.currencyID = Invoice.DocumentCurrencyCode.Value;
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount.Value = 0.00M;
                            Invoice.InvoiceLine[index].TaxTotal[1] = null;
                            Invoice.InvoiceLine[index].TaxTotal[2] = null;

                            Invoice.InvoiceLine[index].Item.Description[0].Value = "seguro";
                            Invoice.InvoiceLine[index].Price.PriceAmount.Value = Invoice.InvoiceLine[index].LineExtensionAmount.Value;
                            Invoice.InvoiceLine[index].Price.BaseQuantity.Value = 1;

                            Invoice.InvoiceLine[index].Item.StandardItemIdentification = new ItemIdentificationType();
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID = new IDType();
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.Value = "SEGURO";
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeID = "999";
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeName = "Estándar de adopción del contribuyente";


                            if (Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value == 0)
                            {
                                Invoice.InvoiceLine[index].TaxTotal = null;

                            }
                            valorbruto += Invoice.InvoiceLine[index].LineExtensionAmount.Value;
                            break;

                        case "otrosgastos":
                            Invoice.InvoiceLine[index].ID.Value = lineas.ToString();
                            Invoice.InvoiceLine[index].InvoicedQuantity.unitCode = Exportacion ? "NIU" : "94";
                            Invoice.InvoiceLine[index].Price.BaseQuantity.unitCode = Exportacion ? "NIU" : "94";
                            Invoice.InvoiceLine[index].InvoicedQuantity.Value = 1;
                            Invoice.InvoiceLine[index].LineExtensionAmount.Value = interfazDian.EFRound(Convert.ToDecimal(item.OtrosGastos.ToString()));
                            BaseGrabableCalc += (Convert.ToDecimal(item.I.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value = (Convert.ToDecimal(item.I.ToString()) > 0 ? Invoice.InvoiceLine[index].LineExtensionAmount.Value : 0.00M);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value = interfazDian.EFRound(Convert.ToDecimal(item.I.ToString()) + 0.00m);
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value * ((Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value > 0 ? Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value : 0) / 100));
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "01";
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "IVA";
                            Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value = Invoice.InvoiceLine[index].TaxTotal[0].TaxSubtotal[0].TaxAmount.Value;
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount = new RoundingAmountType();
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount.currencyID = Invoice.DocumentCurrencyCode.Value;
                            Invoice.InvoiceLine[index].TaxTotal[0].RoundingAmount.Value = 0.00M;
                            Invoice.InvoiceLine[index].TaxTotal[1] = null;
                            Invoice.InvoiceLine[index].TaxTotal[2] = null;

                            Invoice.InvoiceLine[index].Item.Description[0].Value = "Otros Gastos";
                            Invoice.InvoiceLine[index].Price.PriceAmount.Value = Invoice.InvoiceLine[index].LineExtensionAmount.Value;
                            Invoice.InvoiceLine[index].Price.BaseQuantity.Value = 1;

                            Invoice.InvoiceLine[index].Item.StandardItemIdentification = new ItemIdentificationType();
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID = new IDType();
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.Value = "OTROS";
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeID = "999";
                            Invoice.InvoiceLine[index].Item.StandardItemIdentification.ID.schemeName = "Estándar de adopción del contribuyente";


                            if (Invoice.InvoiceLine[index].TaxTotal[0].TaxAmount.Value == 0)
                            {
                                Invoice.InvoiceLine[index].TaxTotal = null;

                            }
                            valorbruto += Invoice.InvoiceLine[index].LineExtensionAmount.Value;
                            break;
                        default:
                            break;
                    }

                    index += 1;
                    lineas += 1;
                }
                */
                //Impuesto IVA
                /* if (Objetos.Constantes.POSINT && Facturaelectronicadian.Properties.Settings.Default.EMPRESA != "BUSINT")
                {

                    Invoice.TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value = BaseGrabableCalc;
                    //Invoice.TaxTotal[0].TaxAmount.Value = interfazDian.EFRound(Invoice.TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value * ((Convert.ToDecimal(item.Piva3.ToString()) > 0 ? Convert.ToDecimal(item.Piva3.ToString()) : 0) / 100) + 0.00M);
                    Invoice.TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(Convert.ToDecimal(item.Piva3.ToString()).ToString("0.00"));


                    Invoice.TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value = BaseGrabableCalcConsumo;
                    //Invoice.TaxTotal[2].TaxAmount.Value = interfazDian.EFRound(Invoice.TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value * ((Convert.ToDecimal(item.PIvaCons.ToString()) > 0 ? Convert.ToDecimal(item.PIvaCons.ToString()) : 0) / 100) + 0.00M);
                    Invoice.TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(Convert.ToDecimal(item.PIvaCons.ToString()).ToString("0.00"));

                }
                else
                {
                    //Invoice.TaxTotal[0].TaxAmount.Value = interfazDian.EFRound(BaseGrabableCalc * ((Convert.ToDecimal(item.I.ToString()) > 0 ? Convert.ToDecimal(item.I.ToString()) : 0) / 100) + 0.00M);
                    //Invoice.TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value = interfazDian.EFRound((Convert.ToDecimal(item.I.ToString()) > 0 ? BaseGrabableCalc + 0.00M : 0.00M));
                    Invoice.TaxTotal[0].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(Convert.ToDecimal(item.I.ToString()).ToString("0.00"));

                    // IMPUESTO AL CONSUMO EN BUSINT
                    Invoice.TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value = 0;
                    Invoice.TaxTotal[2].TaxAmount.Value = 0.00M;
                    Invoice.TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value = 0.00M;

                }

                Invoice.TaxTotal[0].TaxSubtotal[0].TaxAmount.Value = Invoice.TaxTotal[0].TaxAmount.Value;
                Invoice.TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "01";
                Invoice.TaxTotal[0].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "IVA";
                Invoice.TaxTotal[0].RoundingAmount = new RoundingAmountType();
                Invoice.TaxTotal[0].RoundingAmount.currencyID = Invoice.DocumentCurrencyCode.Value;
                Invoice.TaxTotal[0].RoundingAmount.Value = 0.00M;

                //Impuesto CONSUMO
                if (Invoice.TaxTotal[2].TaxSubtotal[0].TaxCategory.Percent.Value > 0)
                {
                    Invoice.TaxTotal[2].TaxSubtotal[0].TaxAmount.Value = Invoice.TaxTotal[2].TaxAmount.Value;
                }

                Invoice.TaxTotal[2].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "04";
                Invoice.TaxTotal[2].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "INC";
                Invoice.TaxTotal[2].RoundingAmount = new RoundingAmountType();
                Invoice.TaxTotal[2].RoundingAmount.currencyID = Invoice.DocumentCurrencyCode.Value;
                Invoice.TaxTotal[2].RoundingAmount.Value = 0.00M;

                if (Invoice.TaxTotal[0].TaxAmount.Value == 0 && Invoice.TaxTotal[2].TaxAmount.Value == 0)
                {
                    Invoice.TaxTotal = null;
                }
                else
                {
                    if (Invoice.TaxTotal[0].TaxAmount.Value == 0)
                    {
                        Invoice.TaxTotal[0] = null;
                    }

                    if (Invoice.TaxTotal[2].TaxAmount.Value == 0)
                    {
                        Invoice.TaxTotal[2] = null;
                    }
                }
                */
                //Impuesto ICA
                //Invoice.TaxTotal[1].TaxAmount.Value = 0.00M;
                //Invoice.TaxTotal[1].TaxSubtotal[0].TaxableAmount.Value = 0.00M;
                //Invoice.TaxTotal[1].TaxSubtotal[0].TaxAmount.Value = 0.00M;
                //Invoice.TaxTotal[1].TaxSubtotal[0].TaxCategory.Percent.Value = 0.000M;
                //Invoice.TaxTotal[1].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "03";
                //Invoice.TaxTotal[1].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "ICA";
                //Invoice.TaxTotal[1] = null;

                int indexret = 0;
                decimal reten = 0;
                //retenciones
                foreach (var retes in retnames)
                {
                    switch (retes)
                    {
                        case "Frtfte":
                            //Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value = interfazDian.EFRound(BaseGrabableCalc * ((Convert.ToDecimal(item.RT.ToString()) > 0 ? Convert.ToDecimal(item.RT.ToString()) : 0) / 100) + 0.00M);
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxableAmount.Value = interfazDian.EFRound((Convert.ToDecimal(item.RT.ToString()) > 0 ? BaseGrabableCalc + 0.00M : 0.00M));
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxAmount.Value = Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value;
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "06";
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "ReteRenta";
                            // Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(Convert.ToDecimal(item.RT.ToString()).ToString("0.00"));
                            //reten += Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value;
                            break;
                        case "Frtfteiva":
                            //Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value = interfazDian.EFRound(BaseGrabableCalc * ((Convert.ToDecimal(item.RTI.ToString()) > 0 ? Convert.ToDecimal(item.RTI.ToString()) : 0) / 100) + 0.00M);
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxableAmount.Value = interfazDian.EFRound((Convert.ToDecimal(item.RTI.ToString()) > 0 ? BaseGrabableCalc + 0.00M : 0.00M));
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxAmount.Value = Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value;
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "05";
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "ReteIVA";
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(Convert.ToDecimal(item.RTI.ToString()).ToString("0.00"));
                            //reten += Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value;
                            break;
                        case "FRTFTEICA":
                            //Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value = interfazDian.EFRound(BaseGrabableCalc * ((Convert.ToDecimal(item.RTICA.ToString()) > 0 ? Convert.ToDecimal(item.RTICA.ToString()) : 0) / 100) + 0.00M);
                            // Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxableAmount.Value = interfazDian.EFRound((Convert.ToDecimal(item.RTICA.ToString()) > 0 ? BaseGrabableCalc + 0.00M : 0.00M));
                            /*Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxAmount.Value = Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value;
                            Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.TaxScheme.ID.Value = "07";
                            Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.TaxScheme.Name.Value = "ReteICA";
                            //Invoice.WithholdingTaxTotal[indexret].TaxSubtotal[0].TaxCategory.Percent.Value = Convert.ToDecimal(Convert.ToDecimal(item.RTICA.ToString()).ToString("0.00"));
                            reten += Invoice.WithholdingTaxTotal[indexret].TaxAmount.Value;
                            */
                            break;
                        default:
                            break;
                    }
                    indexret += 1;

                }

                //Totales
                //Total Valor Bruto antes de tributos: Total valor bruto, suma de los valores brutos de las líneas de la factura.
                // Invoice.LegalMonetaryTotal.LineExtensionAmount.Value = interfazDian.EFRound(valorbruto + 0.00M);

                //Total Valor Base Imponible : Base imponible para el cálculo de los tributos
                try
                {
                    /*if (Invoice.TaxTotal[0] != null || Invoice.TaxTotal[2] != null)
                    {
                        if (Invoice.TaxTotal[0] != null)
                        {
                            Invoice.LegalMonetaryTotal.TaxExclusiveAmount.Value = Invoice.TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value;
                        }

                        //Total Valor Base Imponible : Base imponible para el cálculo de los tributos ipoconsumo
                        if (Invoice.TaxTotal[2] != null)
                        {
                            Invoice.LegalMonetaryTotal.TaxExclusiveAmount.Value += Invoice.TaxTotal[2].TaxSubtotal[0].TaxableAmount.Value;
                        }
                    }
                    else
                    {
                        Invoice.LegalMonetaryTotal.TaxExclusiveAmount.Value = Invoice.TaxTotal[0].TaxSubtotal[0].TaxableAmount.Value;
                    }*/
                }
                catch (Exception)
                {
                    //Invoice.LegalMonetaryTotal.TaxExclusiveAmount.Value = 0.00M;
                }


                try
                {
                    //Total de Valor Imponible más tributos 
                    /*if (Invoice.TaxTotal[0] != null || Invoice.TaxTotal[2] != null)
                    {
                        Invoice.LegalMonetaryTotal.TaxInclusiveAmount.Value = Invoice.LegalMonetaryTotal.LineExtensionAmount.Value;

                        if (Invoice.TaxTotal[0] != null)
                        {
                            Invoice.LegalMonetaryTotal.TaxInclusiveAmount.Value += Invoice.TaxTotal[0].TaxAmount.Value;
                        }

                        if (Invoice.TaxTotal[2] != null)
                        {
                            Invoice.LegalMonetaryTotal.TaxInclusiveAmount.Value += Invoice.TaxTotal[2].TaxAmount.Value;
                        }
                    }
                    else
                    {
                        Invoice.LegalMonetaryTotal.TaxInclusiveAmount.Value = Invoice.LegalMonetaryTotal.LineExtensionAmount.Value + 0.00M;
                    }*/
                }
                catch (Exception)
                {

                    //Invoice.LegalMonetaryTotal.TaxInclusiveAmount.Value = Invoice.LegalMonetaryTotal.LineExtensionAmount.Value + 0.00M;
                }


                //Opcional Anticipo Total: Suma de todos los pagos anticipados
                //Invoice.LegalMonetaryTotal.PrepaidAmount.Value = 12604.00M;

                //Valor a Pagar de Factura: 
                //Valor total de ítems(incluyendo cargos y descuentos a nivel de ítems) + valor tributos + valor cargos – valor descuentos – valor anticipos
                // Invoice.LegalMonetaryTotal.PayableAmount.Value = interfazDian.EFRound(Invoice.LegalMonetaryTotal.TaxInclusiveAmount.Value);//- (retnames.Count>=1 ?Invoice.WithholdingTaxTotal[0].TaxAmount.Value:0)- (retnames.Count >= 2 ? Invoice.WithholdingTaxTotal[1].TaxAmount.Value:0)- (retnames.Count >= 3?Invoice.WithholdingTaxTotal[2].TaxAmount.Value:0));
                //Invoice.LegalMonetaryTotal.PayableAmount.Value = 1406576.50M;

                if (!checkdetalles)
                {
                    // richTextBox1.Text += "\r La factura N= " + item.nfact.ToString() + " no contiene lineas, por favor contactar con administrador del sistema";
                    //     return false;
                }

                //Calculo del CUFE
                // Invoice.UUID.Value = (checkBox1.Checked == true ? interfazDian.Cude(Invoice) : interfazDian.Cufe(Invoice));
                // Invoice.UUID.schemeID = Facturaelectronicadian.Properties.Settings.Default.ambiente.ToLower() == "pruebas" ? "2" : "1";
                // Invoice.LineCountNumeric.Value = ((factind == 1 ? (Datos.detallesIND.Rows.Count == 0 ? Datos.detalles_ESPECIALIND.Rows.Count : Datos.detallesIND.Rows.Count) : (detalles.Rows.Count == 0 ? detallesesp.Rows.Count : detalles.Rows.Count)) + lineasadicionales.Count);
                // string pedido = item.Nped.ToString() == "" ? "" : "PEDIDO: " + item.Nped.ToString();
                //Invoice.Note[0].Value = pedido;
                /*Invoice.UBLExtensions[0].ExtensionContent.DianExtensions.QRCode.Value =
                "NumFac=" + item.PrefijoFact.ToString() + Num.ToString()
                   + "\r\n" + "FecFac=" + Invoice.IssueDate.Value.ToString("yyyy-MM-dd")
                   + "\r\n" + "HorFac=" + Invoice.IssueDate.Value.ToString("H:mm:sszzz")
                   + "\r\n" + "NitFac=" + parametrizacion.Nit
                   //+ "\r\n" + "DocAdq=" + Invoice.AccountingCustomerParty.Party.PartyTaxScheme[0].CompanyID.Value.ToString()
                   //+ "\r\n" + "ValFac=" + Invoice.LegalMonetaryTotal.LineExtensionAmount.Value
                   //+ "\r\n" + "ValIva=" + (Invoice.LegalMonetaryTotal.TaxInclusiveAmount.Value - Invoice.LegalMonetaryTotal.LineExtensionAmount.Value) + 0.00M
                   + "\r\n" + "ValOtroIm=" + reten.ToString("#.##")
                   + "\r\n" + "ValTolFac="
                   + "\r\n" + "CUFE=UUID"
                   + "\r\n" + "https://catalogo-vpfe.dian.gov.co/document/searchqr?documentkey=1IUD&emissionDate=" + Invoice.IssueDate.Value.ToString("yyyyMMdd");
                */

                // nuevo consecutivo de envio de correo
                string consecutivoArchivo = item.nfact.ToString();


                try
                {
                    var existe = File.Exists(RUTAGUARDAR + "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + "B.xml");
                    if (existe)
                    {
                        File.Delete(RUTAGUARDAR + "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + "B.xml");
                    }

                    existe = File.Exists(RUTAGUARDAR + "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + ".xml");
                    if (existe)
                    {
                        File.Delete(RUTAGUARDAR + "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + ".xml");
                    }

                    existe = File.Exists(RUTAGUARDAR + "z" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + ".zip");
                    if (existe)
                    {
                        File.Delete(RUTAGUARDAR + "z" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + ".zip");
                    }

                }
                catch (Exception ex)
                {
                    //richTextBox1.Text += "Error: " + ex.Message;
                    using (StreamWriter w = File.AppendText("log.txt"))
                    {
                        //   Log(ex.Message.ToString() + ex.TargetSite.ToString() + ex.Data.ToString() + ex.StackTrace.ToString() + ex.InnerException + ex.Source, w);

                    }
                }

                //Serialize to Xml file
                XmlSerializeToFile(Invoice, namespaces, RUTAGUARDAR + "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + "B.xml");



                // crear instancia
                /* var firma = new FirmaElectronica
                {
                    RolFirmante = RolFirmante.EMISOR,
                    RutaCertificado = "O:\\" + parametrizacion.NombreCertificado,

                    ClaveCertificado = parametrizacion.ClaveCertificado

                };*/

                // usar horario colombiano
                var fecha = DateTime.Now;
                var firma = new FirmaElectronica
                {
                    RolFirmante = "Role",
                    RutaCertificado = "O:\\FirmasSicaVs",

                    ClaveCertificado = "123645"

                };


                var archivoXml = new FileInfo(RUTAGUARDAR + "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + "B.xml");
                var facturaFirmada = firma.FirmarFactura(archivoXml, fecha);
                // guardar xml firmado
                File.WriteAllBytes(RUTAGUARDAR + "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + ".xml", facturaFirmada);


                createZipFile(RUTAGUARDAR + "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + ".xml", RUTAGUARDAR + "z" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + ".zip", CompressionLevel.Optimal, "fv" + NOMBREXML + consecutivoArchivo.ToString().PadLeft(8, '0') + ".xml");

                return true;

            }
            catch (Exception ex)
            {
                // richTextBox1.Text += "\r Error = " + ex.Message + ex.InnerException + ex.Source + ex.InnerException + ex.StackTrace + ex.TargetSite + " por favor contactar con administrador del sistema";
                using (StreamWriter w = File.AppendText("log.txt"))
                {
                    //Log(ex.Message.ToString() + ex.TargetSite.ToString() + ex.Data.ToString() + ex.StackTrace.ToString() + ex.InnerException + ex.Source, w);

                }
                return false;
            }
        }

        public static void createZipFile(string inputfile, string outputfile, CompressionLevel compressionlevel, string NAME)
        {
            try
            {
                using (ZipArchive za = ZipFile.Open(outputfile, ZipArchiveMode.Update))
                {
                    //using the same file name as entry name
                    //za.CreateEntryFromFile(inputfile, "face_f08110364070990000000.xml");
                    za.CreateEntryFromFile(inputfile, NAME);
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Invalid input/output file.");
                Environment.Exit(-1);
            }
        }

        public void XmlSerializeToFile(InvoiceType Invoice, XmlSerializerNamespaces namespaces, string filename)
        {
            var serializer = new XmlSerializer(typeof(InvoiceType));
            using (var stream = new StreamWriter(filename))
                serializer.Serialize(stream, Invoice, namespaces);
        }

    }

    public class InvoiceFact
    {
        public DateTime UFECHA { get; set; }
        public string COD { get; set; }
        public int nfact { get; set; }
        public string Fechaini { get; set; }
        public string Fechafin { get; set; }
        public string Nzona { get; set; }
        public string Frtfte { get; set; }
        public string Frtfteiva { get; set; }
        public Decimal fletes { get; set; }
        public string FRTFTEICA { get; set; }
        public int factind { get; set; }
        public string trm { get; set; }
        public Decimal seguro { get; set; }
        public Decimal OtrosGastos { get; set; }
        public Decimal I { get; set; }
        public string PrefijoFact { get; set; }
        public string Naturaleza { get; set; }
        public DatosFacturas Datos { get; set; }
    }
    public class DatosFacturas
    {
        public DataTable detalles { get; set; }

    }
}
