using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class FenPatient : Window
    {
        NLHEntities myBDD;
        Gestion ges;

        public FenPatient(Gestion g)
        {
            myBDD = new NLHEntities();
            InitializeComponent();
            ges = g;
            //empecher l'action coller dans le champs de texte NSS
            txtNSS.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DisablePasting));
        }

        private void DisablePasting(object sender, ExecutedRoutedEventArgs e) 
        {
            e.Handled = true;
        }

        public FenPatient()
        {
            myBDD = new NLHEntities();
            InitializeComponent();
            //empecher l'action coller dans le champs de texte NSS
            txtNSS.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, DisablePasting));

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            reinitialiser();
        }

        private void updateDg()
        {
            
            var query =
                from pat in myBDD.tblPatients
                join comp in myBDD.tblCompagnies on pat.IDCompagnie equals comp.IDCompagnie
                join par in myBDD.tblParents on pat.refParent equals par.refParent
                select new { pat.NSS, pat.nom, pat.prenom, pat.dateNaissance, IDComp = pat.IDCompagnie, nomCompagnie = comp.nom, RefPar = pat.refParent, nomPrenomParent = par.nom + " " + par.prenom };

            dgPatients.DataContext = query.ToList();
            cboCompagnie.DataContext = myBDD.tblCompagnies.ToList();
            cboParent.DataContext = myBDD.tblParents.ToList();

        }
        private void btnAssocierParent_Click(object sender, RoutedEventArgs e)
        {
            FenParent FParent = new FenParent();
            FParent.ShowDialog();
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if ( txtNSS.Text.Trim() == "" ||
            txtNomPatient.Text.Trim() == "" ||
            txtPrenomPatient.Text.Trim() == "" ||
            dpDateNaissance.Text.Trim() == "" ||
            txtAdresse.Text.Trim() == "" ||
            txtVille.Text.Trim() == "" ||
            txtProvince.Text.Trim() == "" ||
            txtCP.Text.Trim() == "" ||
            txtTelephone.Text.Trim() == "" ||
            cboCompagnie.Text.Trim() == "" ||
            cboParent.Text.Trim() == ""
           )
            {
                MessageBox.Show("Tous les champs doivent etre remplis", "Champs vides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            tblPatient tp = new tblPatient();
            tp.NSS = txtNSS.Text;
            tp.nom = txtNomPatient.Text;
            tp.prenom=txtPrenomPatient.Text;
            tp.dateNaissance=dpDateNaissance.SelectedDate;
            tp.adresse=txtAdresse.Text;
            tp.ville=txtVille.Text;
            tp.province=txtProvince.Text;
            tp.codePostal=txtCP.Text;
            tp.telephone=txtTelephone.Text;
            tp.IDCompagnie = (int)cboCompagnie.SelectedValue;
            tp.refParent = (int)cboParent.SelectedValue;
            myBDD.tblPatients.Add(tp);

            try
            {
                myBDD.SaveChanges();
                updateDg();
                reinitialiser();
                MessageBox.Show("Ajout effectue avec succes", "Succes", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            catch (Exception ex)
            {
                MessageBox.Show("Erreur ajout patient", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgPatients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgPatients.SelectedIndex == -1) return;
            var query =
                from pat in myBDD.tblPatients
                join comp in myBDD.tblCompagnies on pat.IDCompagnie equals comp.IDCompagnie
                join par in myBDD.tblParents on pat.refParent equals par.refParent
                select new { pat.NSS, pat.nom, pat.prenom, pat.dateNaissance, pat.adresse,pat.ville,pat.province,pat.codePostal,pat.telephone, IDComp = pat.IDCompagnie, nomCompagnie = comp.nom, RefPar = pat.refParent, nomPrenomParent = par.nom , telParent = par.telephone };

            var lstPatients = query.ToList();
            var tp = lstPatients.ElementAt(dgPatients.SelectedIndex);

            txtNSS.Text = tp.NSS;
            txtNomPatient.Text = tp.nom;
            txtPrenomPatient.Text = tp.prenom;
            dpDateNaissance.SelectedDate = tp.dateNaissance;
            txtAdresse.Text = tp.adresse;
            txtVille.Text = tp.ville;
            txtProvince.Text = tp.province;
            txtCP.Text = tp.codePostal;
            txtTelephone.Text = tp.telephone;
            cboCompagnie.Text = tp.nomCompagnie;
            cboParent.Text = tp.nomPrenomParent;

            btnModifier.IsEnabled = true;
            
            btnAjouter.IsEnabled = false;
            txtNSS.IsReadOnly = !btnAjouter.IsEnabled;
        }

        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            if (txtNSS.Text.Trim() == "" ||
            txtNomPatient.Text.Trim() == "" ||
            txtPrenomPatient.Text.Trim() == "" ||
            dpDateNaissance.Text.Trim() == "" ||
            txtAdresse.Text.Trim() == "" ||
            txtVille.Text.Trim() == "" ||
            txtProvince.Text.Trim() == "" ||
            txtCP.Text.Trim() == "" ||
            txtTelephone.Text.Trim() == "" ||
            cboCompagnie.Text.Trim() == "" ||
            cboParent.Text.Trim() == ""
            )
            {
                MessageBox.Show("Tous les champs doivent etre remplis", "Champs vides", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var query =
                (from pat in myBDD.tblPatients
                 where pat.NSS == txtNSS.Text
                 select pat).ToList();


            query[0].nom = txtNomPatient.Text;
            query[0].prenom = txtPrenomPatient.Text;
            query[0].dateNaissance = dpDateNaissance.SelectedDate;
            query[0].adresse = txtAdresse.Text;
            query[0].ville = txtVille.Text;
            query[0].province = txtProvince.Text;
            query[0].codePostal = txtCP.Text;
            query[0].telephone = txtTelephone.Text;
            query[0].IDCompagnie = (int)cboCompagnie.SelectedValue;
            query[0].refParent = (int)cboParent.SelectedValue;

            try
            {
                myBDD.SaveChanges();
                updateDg();
                reinitialiser();
                MessageBox.Show("Modification effectue avec succes", "Succes", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            catch (Exception ex)
            {
                MessageBox.Show("Erreur modification patient", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void reinitialiser()
        {
            txtNSS.Text = "";
            txtNomPatient.Text = "";
            txtPrenomPatient.Text = "";
            dpDateNaissance.SelectedDate = null;
            txtAdresse.Text = "";
            txtVille.Text = "";
            txtProvince.Text = "";
            txtCP.Text = "";
            txtTelephone.Text = "";
            cboCompagnie.Text = "";
            cboParent.Text = "";
            txtRechercheNSS.Text = "";
            dgPatients.SelectedIndex = -1;

            btnAjouter.IsEnabled = true;
            btnModifier.IsEnabled = false;
            btnRechercher.IsEnabled = false;
            
            txtNSS.IsReadOnly = !btnAjouter.IsEnabled;
            updateDg();
        }

        private void btnReini_Click(object sender, RoutedEventArgs e)
        {
            reinitialiser();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            ges.membre = txtNSS.Text;
            Close();

        }

        private void Window_Activated(object sender, EventArgs e)
        {
            cboParent.DataContext = myBDD.tblParents.ToList();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRechercher_Click(object sender, RoutedEventArgs e)
        {
            var query =
                from pat in myBDD.tblPatients
                join comp in myBDD.tblCompagnies on pat.IDCompagnie equals comp.IDCompagnie
                join par in myBDD.tblParents on pat.refParent equals par.refParent
                where pat.NSS == txtRechercheNSS.Text || pat.nom == txtRechercheNSS.Text
                select new { pat.NSS, pat.nom, pat.prenom, pat.dateNaissance, pat.adresse, pat.ville, pat.province, pat.codePostal, pat.telephone, IDComp = pat.IDCompagnie, nomCompagnie = comp.nom, RefPar = pat.refParent, nomPrenomParent = par.nom, telParent = par.telephone };

            var lstPatients = query.ToList();
            dgPatients.DataContext = lstPatients;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnEffacerRecherche_Click(object sender, RoutedEventArgs e)
        {
            
            var requetePatients =
                from pat in myBDD.tblPatients
                join comp in myBDD.tblCompagnies on pat.IDCompagnie equals comp.IDCompagnie
                join par in myBDD.tblParents on pat.refParent equals par.refParent
                select new { pat.NSS, pat.nom, pat.prenom, pat.dateNaissance, pat.adresse, pat.ville, pat.province, pat.codePostal, pat.telephone, IDComp = pat.IDCompagnie, nomCompagnie = comp.nom, RefPar = pat.refParent, nomPrenomParent = par.nom, telParent = par.telephone };

            var lstPatients = requetePatients.ToList();
            dgPatients.DataContext = lstPatients;

            txtRechercheNSS.Text = "";
        }

        private void txtRechercheNSS_TextChanged(object sender, TextChangedEventArgs e)
        {
            btnRechercher.IsEnabled = (txtRechercheNSS.Text.Length > 0);       
        }

        private void txtNSS_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        private void txtNSS_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            NumberValidationTextBox(sender, e);
        }
    }
}
