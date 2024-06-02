namespace SicaVS.Objectos
{
    public class SignatureXPathExpression
    {
        #region Private variables

        private Dictionary<string, string> _namespaces;

        #endregion

        #region Public properties

        public string XPathExpression { get; set; }

        public Dictionary<string, string> Namespaces
        {
            get
            {
                return _namespaces;
            }
        }

        #endregion

        #region Constructors

        public SignatureXPathExpression()
        {
            _namespaces = new Dictionary<string, string>();
        }

        #endregion
    }
}
