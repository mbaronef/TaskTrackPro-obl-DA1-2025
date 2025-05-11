using Blazored.LocalStorage;
using Dominio;
using Dominio.Excepciones;
using Servicios.Excepciones;
using Servicios.Gestores;

namespace Interfaz.ServiciosInterfaz;

public class LogicaSesion
{
        public Usuario? UsuarioLogueado { get; private set; }
        private const string CURRENT_USER = "current_user";
    
        private readonly ILocalStorageService _localStorage;
        private readonly GestorUsuarios _gestorUsuarios;
        
        public LogicaSesion(ILocalStorageService localStorage, GestorUsuarios gestorUsuarios)
        {
            _localStorage = localStorage;
            _gestorUsuarios = gestorUsuarios;
        }
        
        public async Task<bool> Login(string email, string contraseña)
        {
            try{
                Usuario usuarioLogueado = _gestorUsuarios.LogIn(email, contraseña);
                UsuarioLogueado = usuarioLogueado;
                await _localStorage.SetItemAsync(CURRENT_USER, usuarioLogueado);
                return true;
            }
            catch(ExcepcionServicios e)
            {
                return false;
            }
        }

        public async Task<bool> HaySesionActiva()
        {
            Usuario? usuario = await _localStorage.GetItemAsync<Usuario>(CURRENT_USER);
            UsuarioLogueado = usuario;

            return usuario is not null;
        }

        public async Task LogOut()
        {
            UsuarioLogueado = null;
            await _localStorage.RemoveItemAsync(CURRENT_USER);
        }

        public bool EsAdminSistema()
        {
            return UsuarioLogueado.EsAdministradorSistema;
        }
        
        public bool EsAdminProyecto()
        {
            return UsuarioLogueado.EsAdministradorProyecto;
        }
}