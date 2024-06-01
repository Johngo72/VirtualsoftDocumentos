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
        public JwtHelper(IConfiguration configuration)
        {
            _Jwtoptions = configuration.GetSection("Jwt").Get<Jwt>();
        } 
        public string GenerarToken(InfRegistro registroInf)
        {
            var claims = new[]
            {
                 new Claim(JwtRegisteredClaimNames.Sub,_Jwtoptions.Subject),
                 new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                 // bloquea el sistema new Claim(JwtRegisteredClaimNames.Iat,DateTime.UtcNow.ToString()),
                 new Claim("Id",registroInf.IdRegistro.ToString()),
                 new Claim("DocumentoCliente",registroInf.DocumentoCliente),
                 new Claim("tokenEmpresa",registroInf.TokenEmpresa),
                 new Claim("BD",registroInf.datos)
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

        public InfRegistro GetClaimModelo(IEnumerable<Claim> listClaims)
        {
            InfRegistro rs = new InfRegistro();
            foreach (var kvp in listClaims)
            {
                switch (kvp.Type)
                {
                    case "DocumentoCliente":
                        rs.DocumentoCliente = kvp.Value.ToString();
                        break;
                    case "TokenEmpresa":
                        rs.TokenEmpresa = kvp.Value.ToString();
                        break;
                    case "Id":
                        rs.Id = Convert.ToInt32(kvp.Value);
                        break;
                    case "BD":
                        rs.datos = kvp.Value.ToString();
                        break;
                }
            }

            string cnStr = "Data Source=66.70.229.26;Initial Catalog=Sica_Uramax;User ID=sa;Password=Sica2014;TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(cnStr))
            {
                connection.Open();
                string query = "SELECT Id, DocumentoCliente, TokenEmpresa, BaseDatos FROM ResolucionesClientes WHERE Id = @Id AND TokenEmpresa = @TokenEmpresa";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", rs.Id);
                    command.Parameters.AddWithValue("@TokenEmpresa", rs.TokenEmpresa);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            InfRegistro resolucion = new InfRegistro
                            {
                                IdRegistro = reader.GetInt32(0),
                                DocumentoCliente = reader.GetString(1),
                                TokenEmpresa = reader.GetString(2),
                                datos = reader.GetString(3)
                            };
                            
                        }
                    }
                }
            }
            return rs;


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
