using System;

namespace BiblioGest.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime DerniereConnexion { get; set; }
        public bool EstActif { get; set; }
        public string PasswordHash { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateModification { get; set; }

        public User()
        {
            DateCreation = DateTime.Now;
            DerniereConnexion = DateTime.Now;
            EstActif = true;
        }
    }
} 