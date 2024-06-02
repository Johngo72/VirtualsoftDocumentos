using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using SicaVS.Modelos;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SicaVS.Helper
{
    public class JwtHelper
    {
        private readonly Jwt _Jwtoptions;
        private readonly string _cngStr;
        public JwtHelper(IConfiguration configuration)
        {
            _Jwtoptions = configuration.GetSection("Jwt").Get<Jwt>();
            _cngStr = configuration.GetConnectionString("cnStr");

        } 

        public string GenerarToken(InfRegistro registroInf)
        {
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub,_Jwtoptions.Subject),
                 new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                 // bloquea el sistema new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                 new Claim("Id",registroInf.Id.ToString()),
                 new Claim("DocumentoEmisor",registroInf.DocumentoEmisor),
                 new Claim("tokenEmpresa",registroInf.TokenEmpresa),
                 new Claim("BD",registroInf.Basedatos)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Jwtoptions.Key));
            var singIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer:_Jwtoptions.Issuer,
                audience :_Jwtoptions.Audience,
                expires:DateTime.Now.AddDays(364),
                claims :claims,
                signingCredentials: singIn
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public IEnumerable<Claim> InfToken(string token ) 
        {
            var trimstoken = token.Replace("Bearer ", "").Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            var KeyToken=Encoding.UTF8.GetBytes(_Jwtoptions.Key);
            var validationparametros = new TokenValidationParameters 
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _Jwtoptions.Issuer,
                ValidAudience = _Jwtoptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(KeyToken),
            };
            try
            {               
                var principal = tokenHandler.ValidateToken(trimstoken, validationparametros, out SecurityToken validToken);
                return principal.Claims;
            }
            catch (Exception ex)
            {
                return null;
            }            
        }

        public InfRegistro GetClaimValidacion(IEnumerable<Claim> listClaims)
        {
            InfRegistro regConexion = new InfRegistro();
            foreach (var infClaims in listClaims)
            {
                switch (infClaims.Type)
                {
                    case "DocumentoEmisor":
                        regConexion.DocumentoEmisor= infClaims.Value.ToString();
                        break;
                    case "tokenEmpresa":
                        regConexion.TokenEmpresa = infClaims.Value.ToString();
                        break;
                    case "Id":
                        regConexion.Id = Convert.ToInt32(infClaims.Value);
                        break;
                    case "BD":
                        regConexion.Basedatos = infClaims.Value.ToString();
                        break;
                }
            }

            //string cnStr = "Data Source=66.70.229.26;Initial Catalog=Sica_Uramax;User ID=sa;Password=Sica2014;TrustServerCertificate=True;";


            using (SqlConnection connection = new SqlConnection(_cngStr))
            {
                connection.Open();
                string query = "SELECT Id, DocumentoEmisor, BaseDatos, TokenEmpresa,TokenCertificado,CorreoEnvioFacturas,CorreoToken " +
                    "FROM ResolucionesClientesTiposDocumentos WHERE Id = @Id AND TokenEmpresa = @TokenEmpresa";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", regConexion.Id);
                    command.Parameters.AddWithValue("@TokenEmpresa", regConexion.TokenEmpresa);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            regConexion.DocumentoEmisor= reader.GetString(1);
                            regConexion.Basedatos = reader.GetString(3);
                            regConexion.TokenCertificado = reader.GetString(4);
                            regConexion.CorreoEnvioFacturas= reader.GetString(5);
                            
                            regConexion.CorreoToken = reader.GetString(6);
                            regConexion.EstadoConexion = 1;
                            regConexion.MensajeConexion = "Conectado";
                        }
                        else
                        {
                            regConexion.EstadoConexion = 0;
                            regConexion.MensajeConexion ="No se encontro información de valiación, consulte con el proveedor del servicio.";
                        }
                    }

                }
            }
            return regConexion;
        }
        public Dictionary<string, string> GetClaims(IEnumerable<Claim> listClaims)
        {
            var claims = new Dictionary<string, string>();
            foreach (var claim in listClaims){
                if (!claims.ContainsKey(claim.Type))
                {
                    claims[claim.Type] = claim.Value;
                }
            }
            return claims;
        }
    }
}
