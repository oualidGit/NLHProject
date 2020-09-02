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
    public partial class FenMedecin : Window
    {
        NLHEntities myBDD;
        Gestion ges;
        public FenMedecin()
        {
            InitializeComponent();
            ges = new Gestion();
            myBDD = new NLHEntities();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            FLogin fen = new FLogin();
            Close();
            fen.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateDg();
            string NomDuMed = Application.Current.Properties["­­user"].ToString();
            lblBienvenueMedecin.Content = "Bienvenue Docteur : " + NomDuMed.ToUpper();
        }

        private void btnDonneConge_Click(object sender, RoutedEventArgs e)
        {
        
        }
        private void updateDg()
        {
            var query =
                from dossier in myBDD.tblDossierAdmissions
                join pat in myBDD.tblPatients on dossier.NSS equals pat.NSS
                select new { dossier.IDAdmission, dossier.NSS, dossier.dateAdmission, dossier.dateChirurgie, dossier.dateConge, pat.nom, pat.prenom };

            var lstDossiers = query.ToList();
            if (chkCongeNull.IsChecked == true)
            {
                lstDossiers = query.Where(dc => dc.dateConge == null).ToList();
            }
            else if (chkCongeNull.IsChecked == false)
            {
                lstDossiers = query.ToList();
            }

            dgDossiers.DataContext = lstDossiers;
        }

        private void dgDossiers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgDossiers.SelectedIndex == -1) return;
            var query =
                from dossier in myBDD.tblDossierAdmissions
                join pat in myBDD.tblPatients on dossier.NSS equals pat.NSS
                select new { dossier.IDAdmission, dossier.NSS, dossier.dateAdmission, dossier.dateChirurgie, dossier.dateConge, nomP=pat.nom,prenomP=pat.prenom };

            var lstDossiers = query.ToList();
            if (chkCongeNull.IsChecked == true)
            {
                lstDossiers = query.Where(dc => dc.dateConge == null).ToList();
            }
            else if (chkCongeNull.IsChecked == false)
            {
                lstDossiers = query.ToList();
            }

            var tp = lstDossiers.ElementAt(dgDossiers.SelectedIndex);
            txtNom.Text = tp.nomP;
            txtPrenom.Text = tp.prenomP;
            txtNss.Text = tp.NSS;
            dpConge.SelectedDate = tp.dateConge;
        }

        private void btnMAJConge_Click(object sender, RoutedEventArgs e)
        {
            if (dgDossiers.SelectedIndex == -1 || dpConge.SelectedDate==null)
                MessageBox.Show("Vous devez selectionner un dossier et une date de congé", "Données manquantes" , MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                var query =
                    from dossier in myBDD.tblDossierAdmissions
                    select dossier;

                var lstDossiers = query.ToList();
                if (chkCongeNull.IsChecked == true)
                {
                    lstDossiers = query.Where(dc => dc.dateConge == null).ToList();
                }
                else if (chkCongeNull.IsChecked == false)
                {
                    lstDossiers = query.ToList();
                }

                var selectedDossier = lstDossiers.ElementAt(dgDossiers.SelectedIndex);

                if (dpConge.SelectedDate < selectedDossier.dateAdmission)
                {
                    MessageBox.Show("La date de congé doit etre apres la date d'admission", "Date de congé incorrecte", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                selectedDossier.dateConge = dpConge.SelectedDate;

                var slit = (from lit in myBDD.tblLits
                             where lit.numerolit == selectedDossier.numeroLit
                             select lit).First();

                slit.occupe = false;
                
                var rep = MessageBox.Show("Etes vous sur ?", "Mise a jour date congé", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (rep == MessageBoxResult.Yes) { 
                    try
                    {
                        myBDD.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erreur MAJ", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    dpConge.SelectedDate = selectedDossier.dateConge;
                }

                updateDg();
            }
        }

        private void chkCongeNull_Click(object sender, RoutedEventArgs e)
        {
            if (chkCongeNull.IsChecked == true)
            {
                var query =
                from dossier in myBDD.tblDossierAdmissions
                join pat in myBDD.tblPatients on dossier.NSS equals pat.NSS
                where dossier.dateConge == null
                select new { dossier.IDAdmission, dossier.NSS, dossier.dateAdmission, dossier.dateChirurgie, dossier.dateConge, pat.nom, pat.prenom };

                dgDossiers.DataContext = query.ToList();
            }
            else
            {
                updateDg();
            }
        }

        private void chkCongeNull_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
    
}
