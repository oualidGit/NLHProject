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
    /// Logique d'interaction pour FenAdmin.xaml
    /// </summary>
    public partial class FenAdmin : Window
    {
        NLHEntities myBDD;
        public FenAdmin()
        {
            InitializeComponent();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            
            FLogin fen = new FLogin();
            Close();
            fen.Show();
            
        }

        private void majDG() 
        {
            dgMedecin.DataContext = myBDD.tblMedecins.ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myBDD = new NLHEntities();
            majDG();
            var querySpec =
                        from depart in myBDD.tblDepartements
                        select new
                        {
                            depart.nom,
                        };
            foreach (var item in querySpec.ToList())
            {
                cboSpecialite.Items.Add(item.nom);
            }

            reinitialiser();
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {

            if (txtNomMedecin.Text.Trim() == "" ||
                txtPrenomMedecin.Text.Trim() == "" ||
                cboSpecialite.Text.Trim() == ""
                )
            {
                MessageBox.Show("Tous les champs doivent etre renseignés", "Champs vides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            tblMedecin m = new tblMedecin();
            m.nom = txtNomMedecin.Text;
            m.prenom = txtPrenomMedecin.Text;
            m.specialite = cboSpecialite.Text;

            myBDD.tblMedecins.Add(m);
            try
            {
                myBDD.SaveChanges();
                majDG();
                reinitialiser();
            }
            catch (Exception)
            {
                MessageBox.Show("Erreur enregistrement medecin", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {

            tblMedecin m = (tblMedecin)dgMedecin.SelectedItem;
            var nbrMedecinDansDossier =
                (from dossier in myBDD.tblDossierAdmissions
                where dossier.IDMedecin == m.IDMedecin
                select dossier).ToList().Count;

            if (nbrMedecinDansDossier > 0)
                MessageBox.Show("Ce medecin ne peut etre supprimé car il figure deja dans un dossier d'admission", "Suppression impossible", MessageBoxButton.OK, MessageBoxImage.Error);

            else
            {
                var rep = MessageBox.Show("Etes vous sur de vouloir supprimer le medecin : " + m.nom, "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (rep == MessageBoxResult.No) return;
                myBDD.tblMedecins.Remove(m);
                try
                {
                    myBDD.SaveChanges();
                    majDG();
                    reinitialiser();
                }
                catch (Exception)
                {
                    MessageBox.Show("Erreur suppression medecin", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void dgMedecin_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgMedecin.SelectedIndex == -1) return;
            tblMedecin m = (tblMedecin)dgMedecin.SelectedItem;
            txtNomMedecin.Text = m.nom;
            txtPrenomMedecin.Text = m.prenom;
            cboSpecialite.Text = m.specialite;

            btnAjouter.IsEnabled = false;
            btnModifier.IsEnabled = true;
            btnSupprimer.IsEnabled = true;
            btnReini.IsEnabled = true;

        }

        private void btnReini_Click(object sender, RoutedEventArgs e)
        {
            reinitialiser();
        }

        private void reinitialiser() 
        {
            txtNomMedecin.Text = "";
            txtPrenomMedecin.Text = "";
            cboSpecialite.Text = "";
            dgMedecin.SelectedIndex = -1;

            btnAjouter.IsEnabled = true;
            btnModifier.IsEnabled = false;
            btnSupprimer.IsEnabled = false;
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (txtNomMedecin.Text.Trim() == "" ||
                txtPrenomMedecin.Text.Trim() == "" ||
                cboSpecialite.Text.Trim() == ""
               )
            {
                MessageBox.Show("Tous les champs doivent etre remseignes", "Champs vides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            tblMedecin m = (tblMedecin)dgMedecin.SelectedItem;
            m.nom = txtNomMedecin.Text;
            m.prenom = txtPrenomMedecin.Text;
            m.specialite = cboSpecialite.Text;

            try
            {
                myBDD.SaveChanges();
                majDG();
                reinitialiser();
            }
            catch (Exception)
            {
                MessageBox.Show("Erreur modification medecin", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
