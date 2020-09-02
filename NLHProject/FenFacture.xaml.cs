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
    /// Logique d'interaction pour FenFacture.xaml
    /// </summary>
    public partial class FenFacture : Window
    {
        public FenFacture()
        {
            InitializeComponent();
        }

        private void btnRetour_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RemplirFacture();
        }



        public void RemplirFacture()
        {
            string prixTeleviseur = Application.Current.Properties["­­prixTeleviseur"].ToString();
            string prixTel = Application.Current.Properties["­­prixTel"].ToString();
            string prixLit = Application.Current.Properties["­­prixLit"].ToString();
            string nomPat = Application.Current.Properties["­­NP"].ToString();
            string prenomPat = Application.Current.Properties["­­PP"].ToString();

            txtPrixLit.Text = prixLit;
            txtTelephone.Text = prixTel;
            txtTeleviseur.Text = prixTeleviseur;

            //calculer le total
            double pl = Convert.ToDouble(prixLit);
            double pt = Convert.ToDouble(prixTeleviseur);
            double pp = Convert.ToDouble(prixTel);

            double Total = (pl+pt+pp);
            txtTotalA.Text = Convert.ToString(Total);
            txtTTC.Text = Convert.ToString(Total * 1.15);
            txtBonjour.Text = "Bonjour Mr "+prenomPat+" "+nomPat+" Voici les details de votre Facture ";

        }
    }
}
