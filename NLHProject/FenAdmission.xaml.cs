using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class FenAdmission : Window
    {
        static int numD = NLHConstantes.TOUS_LES_DEPARTEMENTS;
        
        NLHEntities myBDD;
        Gestion g;
        public FenAdmission()
        {
            InitializeComponent();
            g = new Gestion();
            myBDD = new NLHEntities();
        }

        private void btnChoisirPatient_Click(object sender, RoutedEventArgs e)
        {
            FenPatient FPatient = new FenPatient(g);
            FPatient.ShowDialog();
            txtNSS.Text = g.membre;
            
            if (txtNSS.Text == "") 
                return;
            
            var SelectedPatient =
                (from pat in myBDD.tblPatients
                 select pat).Where(x => x.NSS == txtNSS.Text).FirstOrDefault();

            txtNomPatient.Text = SelectedPatient.nom;
            txtPrenomPatient.Text = SelectedPatient.prenom;
            txtAssurancePatient.Text = SelectedPatient.tblCompagny.nom;

            //dgLits.DataContext = null;
        }

        public bool estMineur(string nss)
        {
            if (nss == "")  
                return false;

            var patientMineur =
                 (from pat in myBDD.tblPatients
                 where (pat.NSS == nss) && ( (DateTime.Now.Year - ((DateTime)pat.dateNaissance).Year) <= 16)
                 select pat).FirstOrDefault();

            if (patientMineur != null)  
                return true;

            return false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            updateDg();
            dpAdmission.SelectedDate = DateTime.Today;
        }

        private void updateDg()
        {
            var queryLits =
                from lit in myBDD.tblLits
                join typeLit in myBDD.tblTypesLits on lit.numeroType equals typeLit.numeroType
                join dep in myBDD.tblDepartements on lit.IDDepartement equals dep.IDDepartement
                where lit.occupe == false
                select new { numLit = lit.numerolit, etat = "Libre", type = typeLit.description, departement = dep.nom};

            dgLits.DataContext = queryLits.ToList();
            dgMedecin.DataContext = myBDD.tblMedecins.ToList();
        }
        
        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            FLogin fenLogin = new FLogin();
            Close();
            fenLogin.ShowDialog();
        }

        private void btnListerLits_Click(object sender, RoutedEventArgs e)
        {
            //si aucune chirurgie n'est programmee alors :
            //si le patient est majeur alors lister tous les lits dispo 
            //sinon lister les lits du departement pediatrie seulement
            if (string.IsNullOrEmpty(txtNSS.Text))
                MessageBox.Show("Vous devez selectionner un patient", "Champs vide!", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                if (chkChirurgieProg.IsChecked == false)
                {
                    if (estMineur(txtNSS.Text) == false)
                        maListerLits(NLHConstantes.TOUS_LES_DEPARTEMENTS);
                    else
                        maListerLits(NLHConstantes.DEPART_PEDIATRIE);
                }
                else
                    maListerLits(NLHConstantes.DEPART_CHIRURGIE);
            }
        }

        private void chkChirurgieProg_Click(object sender, RoutedEventArgs e)
        {
            if (txtNSS.Text != "") { 
                if (chkChirurgieProg.IsChecked == true)
                {
                    maListerLits(NLHConstantes.DEPART_CHIRURGIE);
                }
                else if (chkChirurgieProg.IsChecked == false)
                {
                    if (estMineur(txtNSS.Text)) 
                        maListerLits(NLHConstantes.DEPART_PEDIATRIE);
                    else
                        maListerLits(NLHConstantes.TOUS_LES_DEPARTEMENTS); 
                }
            }
        }

        private void maListerLits(int numDep)
        {
            var r = 
                from lit in myBDD.tblLits
                join typel in myBDD.tblTypesLits on lit.numeroType equals typel.numeroType
                join dep in myBDD.tblDepartements on lit.IDDepartement equals dep.IDDepartement
                where lit.occupe == false
                select new { numLit = lit.numerolit, etat = lit.occupe, type = typel.description, departement = dep.nom, idDep = lit.IDDepartement };

            dgLits.DataContext = r.Where(x => (numDep == NLHConstantes.TOUS_LES_DEPARTEMENTS) || (x.idDep == numDep)).ToList();

            numD = numDep;
        }

        public void reinitialiser()
        {
            txtNomPatient.Text = "";
            txtPrenomPatient.Text = "";
            txtAssurancePatient.Text = "";
            dpChirurgie.SelectedDate = null;
            chkChirurgieProg.IsChecked = false;
            dpAdmission.SelectedDate = DateTime.Today;
            dpConge.SelectedDate = null;
            chkTeleviseur.IsChecked = false;
            chkTelephone.IsChecked = false;
            txtNSS.Text = "";
            updateDg();
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (!lesChampsSontRemplis())
            {
                MessageBox.Show("Vous devez selectionner un patient, un lit et un medecin pour creer un dossier", "Champs vides!", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            tblDossierAdmission nouveauDossier = new tblDossierAdmission();

            var listeLits =
                (from lit in myBDD.tblLits
                 where lit.occupe == false
                select lit).ToList();
            
            var selectedLit = listeLits.ElementAt(dgLits.SelectedIndex);
            MessageBox.Show(selectedLit.numerolit.ToString());
            var listeMedecins =
              (from med in myBDD.tblMedecins
              select med).ToList();
            
            var selectedMedecin = listeMedecins.ElementAt(dgMedecin.SelectedIndex);
            
            //on modifie ici l'etat du lit
            selectedLit.occupe = true;

            nouveauDossier.dateChirurgie = dpChirurgie.SelectedDate;
            nouveauDossier.chirurgieProgrammee = chkChirurgieProg.IsChecked;
            nouveauDossier.dateAdmission = dpAdmission.SelectedDate;
            nouveauDossier.dateConge = null;
            nouveauDossier.televiseur = chkTeleviseur.IsChecked;
            nouveauDossier.telephone = chkTelephone.IsChecked;
            nouveauDossier.NSS = txtNSS.Text;
            nouveauDossier.numeroLit = selectedLit.numerolit;

            nouveauDossier.IDMedecin = selectedMedecin.IDMedecin;

            myBDD.tblDossierAdmissions.Add(nouveauDossier);

            var rep = MessageBox.Show("Etes vous sur d'ajouter ce dossier ?", "Ajout de dossier", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (rep == MessageBoxResult.Yes)
            {
                try
                {
                    myBDD.SaveChanges();
                    MessageBox.Show("Ajout effectué avec succes", "succes!", MessageBoxButton.OK, MessageBoxImage.Information);
                    reinitialiser();
                }
                catch(Exception)
                {
                    MessageBox.Show("Ajout de dossier echoué", "Echec!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnReinistialiser_Click(object sender, RoutedEventArgs e)
        {
            reinitialiser();
        }

        private void btnVoirFacture_Click(object sender, RoutedEventArgs e)
        {
            string prixTeleviseur = "0",
                   prixTelephone = "0",
                   prixLit="0";
            bool privee = false;
            int facteur = 0;
            if (chkTeleviseur.IsChecked == true)
                prixTeleviseur="42,50";

            if (chkTelephone.IsChecked == true)
                prixTelephone = "7,5";


            //verifier si patient a une assurance publique ou privee

            var selectedPatient =
                (from pat in myBDD.tblPatients
                where pat.NSS == txtNSS.Text
                select pat).FirstOrDefault();
            if (selectedPatient.tblCompagny.IDCompagnie == 1)
                privee = false;
            else
                privee = true;

            MessageBox.Show(privee.ToString());


            //chercher le Type du lit

            var requeteLits =
            from lit in myBDD.tblLits
            join typel in myBDD.tblTypesLits on lit.numeroType equals typel.numeroType
            join dep in myBDD.tblDepartements on lit.IDDepartement equals dep.IDDepartement
            join dossier in myBDD.tblDossierAdmissions on lit.numerolit equals dossier.numeroLit into joined
            from i in joined.DefaultIfEmpty()
            select new { typeLit = typel.numeroType, occupe = lit.occupe, idDep = dep.IDDepartement, typeDeLit = lit.numeroType };
            var reqq = requeteLits.Where(x => (x.occupe == false) && (numD == NLHConstantes.TOUS_LES_DEPARTEMENTS || x.idDep == numD));


            /*compter les lits par type: standard, prive, semi prive pour calculer le prix apres 
            selon assurance du patient (privee ou public*/

            var listeLitsStandards = requeteLits.Where(x => (x.occupe == false) && (x.typeDeLit == 1)).ToList();
            var listeLitsSemiPrives = requeteLits.Where(x => (x.occupe == false) && (x.typeDeLit == 2)).ToList();
            var listeLitsPrives = requeteLits.Where(x => (x.occupe == false) && (x.typeDeLit == 3)).ToList();
            //facteur est utilisé pour annuler ou garde les frais du lit selon dispo des lits et type d'assurance
            if (privee == false && listeLitsStandards.Count > 0)
                facteur = 1;
            else if (privee == false && listeLitsStandards.Count == 0)
                facteur = 0;
              
            var listeLits = reqq.ToList();
            
            var selectedLit = listeLits.ElementAt(dgLits.SelectedIndex);

            if (selectedLit.typeLit == 1)
                prixLit = "0";

            else if (selectedLit.typeLit == 2)
                prixLit = (267*facteur).ToString();

            else
                prixLit = (571 * facteur).ToString();

            //Storage des données
            Application.Current.Properties["­­prixTeleviseur"] = prixTeleviseur;
            Application.Current.Properties["­­prixTel"] = prixTelephone;
            Application.Current.Properties["­­prixLit"] = prixLit;
            Application.Current.Properties["­­NP"] = txtNomPatient.Text;
            Application.Current.Properties["­­PP"] = txtPrenomPatient.Text;

            FenFacture fenFacture = new FenFacture();
            fenFacture.Show();
        }

        private bool lesChampsSontRemplis() 
        {
            bool champsRemplis;
            champsRemplis = dpAdmission.SelectedDate != null;
            champsRemplis = champsRemplis && !string.IsNullOrEmpty(txtNSS.Text);
            champsRemplis = champsRemplis && dgMedecin.SelectedIndex > -1;
            champsRemplis = champsRemplis && dgLits.SelectedIndex > -1;

            return champsRemplis;
        }
    }
}
