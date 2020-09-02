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
    /// Logique d'interaction pour FenParent.xaml
    /// </summary>
    public partial class FenParent : Window
    {
        NLHEntities myBDD;
        public FenParent()
        {
            InitializeComponent();
        }
   
        private void majDG()
        {
            dgParents.DataContext = myBDD.tblParents.ToList();
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (txtNomParent.Text.Trim() == "" ||
                txtPrenomParent.Text.Trim() == "" ||
                txtAdresse.Text.Trim() == "" ||
                txtVille.Text.Trim() == "" ||
                txtProvince.Text.Trim() == "" ||
                txtCP.Text.Trim() == "" ||
                txtTelephone.Text.Trim() == ""
                )
            {
                MessageBox.Show("Tous les champs doivent etre renseignés", "Champs vides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            tblParent p = new tblParent();
            p.nom = txtNomParent.Text;
            p.prenom = txtPrenomParent.Text;
            p.adresse = txtAdresse.Text;
            p.ville = txtVille.Text; 
            p.province = txtProvince.Text;
            p.codePostal = txtCP.Text;
            p.telephone = txtTelephone.Text;

            myBDD.tblParents.Add(p);
            try
            {
                myBDD.SaveChanges();
                majDG();
                reinitialiser();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur enregistrement parent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myBDD = new NLHEntities();
            majDG();
            reinitialiser();
        }

        private void reinitialiser()
        {
            txtNomParent.Text = "";
            txtPrenomParent.Text = "";
            txtAdresse.Text = "";
            txtVille.Text = "";
            txtProvince.Text = "";
            txtCP.Text = "";
            txtTelephone.Text = "";

            dgParents.SelectedIndex = -1;

            btnAjouter.IsEnabled = true;
            btnModifier.IsEnabled = false;
            btnSupprimer.IsEnabled = false;
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (txtNomParent.Text.Trim() == "" ||
                txtPrenomParent.Text.Trim() == "" ||
                txtAdresse.Text.Trim() == "" ||
                txtVille.Text.Trim() == "" ||
                txtProvince.Text.Trim() == "" ||
                txtCP.Text.Trim() == "" ||
                txtTelephone.Text.Trim() == ""
                )
            {
                MessageBox.Show("Tous les champs doivent etre renseignés", "Champs vides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            tblParent p = (tblParent)dgParents.SelectedItem;
            p.nom = txtNomParent.Text;
            p.prenom = txtPrenomParent.Text;
            p.adresse = txtAdresse.Text;
            p.ville = txtVille.Text;
            p.province = txtProvince.Text;
            p.codePostal = txtCP.Text;
            p.telephone = txtTelephone.Text;

            try
            {
                myBDD.SaveChanges();
                majDG();
                reinitialiser();
            }
            catch (Exception)
            {
                MessageBox.Show("Erreur modifier parent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {

            tblParent p = (tblParent)dgParents.SelectedItem;
            myBDD.tblParents.Remove(p);
            try
            {
                myBDD.SaveChanges();
                majDG();
                reinitialiser();
            }
            catch (Exception)
            {
                MessageBox.Show("Erreur suppression parent", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void dgParents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgParents.SelectedIndex == -1) return;

            tblParent p = (tblParent)dgParents.SelectedItem;
            txtNomParent.Text = p.nom;
            txtPrenomParent.Text = p.prenom;
            txtAdresse.Text = p.adresse;
            txtVille.Text = p.ville;
            txtProvince.Text = p.province;
            txtCP.Text = p.codePostal;
            txtTelephone.Text = p.telephone;

            btnAjouter.IsEnabled = false;
            btnModifier.IsEnabled = true;
            btnSupprimer.IsEnabled = true;
            btnReini.IsEnabled = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (dgParents.SelectedIndex == -1)
            {
                MessageBox.Show("Veuillez selectionner un parent de la liste", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var lstParents = myBDD.tblParents.ToList();
            int tp = lstParents.ElementAt(dgParents.SelectedIndex).refParent;
            Close();
        }

        private void btnReini_Click(object sender, RoutedEventArgs e)
        {
            reinitialiser();
        }
    }
}
