namespace SicaVS
{
    public class Usuario
    {
        public string idusuario { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }
        public string rol { get; set; }
        public static List<Usuario> DB()
        {
            var list = new List<Usuario>()
            {
                new Usuario
                {
                    idusuario="1",
                    usuario="Mateo",
                    password="password",
                    rol="empleado"

                },
                new Usuario
                {
                    idusuario = "2",
                    usuario = "Calos",
                    password = "password",
                    rol = "empleado"

                },
                 new Usuario
                {
                    idusuario = "3",
                    usuario = "Manuel",
                    password = "password",
                    rol = "empleado"

                },
                  new Usuario
                {
                    idusuario = "4",
                    usuario = "Jose",
                    password = "password",
                    rol = "administrador"

                }

            };
            return list;
        }
    }
}
