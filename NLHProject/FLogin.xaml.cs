using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NLHProject
{
    /// <summary>
    /// Logique d'interaction pour FLogin.xaml
    /// </summary>
    public partial class FLogin : Window
    {
        NLHEntities myBDD;
        public FLogin()
        {
            InitializeComponent();
        }


        private void btnConnecter_Click(object sender, RoutedEventArgs e)
        {
            string userName = txtNomUtilisateur.Text;

            var query =
                        (from utilisateur in myBDD.tblAdmins
                         where utilisateur.user == userName
                         select new { lenom = utilisateur.nom, leprenom = utilisateur.prenom, 
                             leuser = utilisateur.user, lepass = utilisateur.password, 
                             lerole = utilisateur.role }).ToList();

            if (query.Count == 0) 
            {
                MessageBox.Show(string.Format($"le user {userName} n'existe pas"));
            }
            else
            {
                var u = query[0];
                if (u.lepass.Trim() != ptxtMotDePasse.Password)
                {
                    MessageBox.Show("Mot de passe erroné !");
                }
                else
                {
                    if (u.lerole.Trim().ToLower() == "administrateur")
                    {
                        
                        FenAdmin FAdmin = new FenAdmin();
                        Close();
                      
                        FAdmin.ShowDialog();
                        
                    }

                    if (u.lerole.Trim().ToLower() == "medecin")
                    {
                        FenMedecin FMedecin = new FenMedecin();
                        Application.Current.Properties["­­user"] = u.leuser;
                         
                      
                        Close();
                        FMedecin.ShowDialog();
                        
                    }
                    if (u.lerole.Trim().ToLower() == "prepose")
                    {
                        FenAdmission FAdmission = new FenAdmission();
                        Close();
                        FAdmission.ShowDialog();
                        
                    }
                }

            }
        }

        private void FenLogin_Loaded(object sender, RoutedEventArgs e)
        {
            myBDD = new NLHEntities();
            txtNomUtilisateur.Focus();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
