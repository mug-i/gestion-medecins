using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Medecins
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqlConnection conDB;
        // Connexion à la base de données SQL
        public MainWindow()
        {
            // assurez vous de changer la valeur de Source et mettre celle de votre Serveur SQL
            conDB = new SqlConnection(@"Data Source=LAPTOP-FO44DTTE\SQLEXPRESS;Initial Catalog=Hopital;Integrated Security=True");
            InitializeComponent();
            //desactiver le textbox tbIDMedecin parce qu'il s'incremente automatiquement;
            tbIDMedecin.IsEnabled = false;
            //appel du fonction qui affiche la liste des medecins;
            charger_Liste_Medecins();

        }


        public void charger_Liste_Medecins()
        {
            // pour charger la table medecin dans la grille dgMedecin
            // gerer les exceptions
            try
            {


                // Vérifier si une connexion à la base de données est déjà ouverte
                if (conDB.State != ConnectionState.Open)
                {
                    // Ouvrir la connexion à la base de données
                    conDB.Open();
                }

                // Requête SQL pour récupérer toutes les données de la table Voiture
                string requete = "SELECT * FROM Medecin";

                // Préparer la commande SQL
                SqlCommand cmd = new SqlCommand(requete, conDB);
                cmd.CommandType = CommandType.Text;

                // Exécuter la commande et récupérer les données dans un lecteur de données
                SqlDataReader dataReader = cmd.ExecuteReader();

                // Créer un objet DataTable pour stocker les données
                DataTable dt = new DataTable();

                // Charger les données dans le DataTable à partir du lecteur de données
                dt.Load(dataReader);

                // Définir la source de données de la grille avec les données du DataTable
                dgMedecin.ItemsSource = dt.DefaultView;

                // Fermer la connexion à la base de données
                conDB.Close();
            }
            catch
            {
                lbMessage.Content = "Verifier connectionString: App.config";
            }


        }

        private void Ligne_Selectionnee(object sender, SelectionChangedEventArgs e)
        {
            // charger la ligne selectionnee dans la dgMedecin vers les controles  TextBox de la page Medecin.

            // Obtenir la grille de données depuis l'expéditeur
            DataGrid grille = sender as DataGrid;

            // Obtenir la ligne sélectionnée en tant que DataRowView
            DataRowView ligne_choisi = grille.SelectedItem as DataRowView;

            // Si une ligne est sélectionnée, remplir les champs de texte avec les données de la ligne
            if (ligne_choisi != null)
            {
                tbIDMedecin.Text = ligne_choisi[0].ToString();
                tbPrenom.Text = ligne_choisi[1].ToString();
                tbnom.Text = ligne_choisi[2].ToString();
                tbNumeroContact.Text = ligne_choisi[3].ToString();
                tbCourriel.Text = ligne_choisi[4].ToString();
                tbSalaire.Text = ligne_choisi[5].ToString();
                tbSpecialite.Text = ligne_choisi[6].ToString();
                tbHopital.Text = ligne_choisi[7].ToString();
            }
        }


        public bool Verifier_champ()
        {

            return (!string.IsNullOrEmpty(tbPrenom.Text) && !string.IsNullOrEmpty(tbnom.Text) && !string.IsNullOrEmpty(tbNumeroContact.Text) && !string.IsNullOrEmpty(tbCourriel.Text)
                && !string.IsNullOrEmpty(tbSalaire.Text) && !string.IsNullOrEmpty(tbSpecialite.Text) && !string.IsNullOrEmpty(tbHopital.Text));
        }

        public bool Medecin_Existant(string phone)
        {
            // verifier si medecin existe
            //1. Si oui return true sinon return false;
            //2. gestion des excpetions try catch

            bool medecin_existe = false;
            // Requête SQL pour compter le nombre de medecin avec le numero de telephone spécifié
            string requete = "SELECT phone FROM Medecin WHERE phone = @phone";

            try
            {


                // Vérifier si la connexion à la base de données n'est pas déjà ouverte
                if (conDB.State != ConnectionState.Open)
                {
                    // Ouvrir la connexion à la base de données
                    conDB.Open();
                }

                // Préparer la commande SQL
                SqlCommand cmd = new SqlCommand(requete, conDB);

                // Ajouter le paramètre MedID à la commande
                cmd.Parameters.AddWithValue("@phone", phone);

                // Exécuter la commande et récupérer les résultats dans un lecteur de données
                SqlDataReader sqlDataReader = cmd.ExecuteReader();

                // Vérifier s'il y a des résultats
                if (sqlDataReader.Read())
                {
                    // Si le numero de contact existe, c'est que le medecin existe
                    if (sqlDataReader["phone"].ToString() != null)
                    {
                        medecin_existe = true;
                    }
                    else
                    {
                        medecin_existe = false;
                    }
                }

                // Fermer la connexion à la base de données
                conDB.Close();
            }
            catch
            {
                lbMessage.Content = "Verifier connectionString: App.config";
            }
           
            return medecin_existe;
        }
        private void btnAjouteur_Med(object sender, RoutedEventArgs e)
        {
            // Ajouter un medecin de la BBD,
            // verifier:
            //1. Medecin n'existe pas d'abord
            //2. tous les champs sont saisis
            //3. gestion des excpetions try catch
            //4. Messages dans le status Bar
            // Vérifier si le medeceni existe dans la base de données par son numero de contact
            try
            {
                if (!Medecin_Existant(tbNumeroContact.Text))
                {

                    // Vérifier si tous les champs sont remplis
                    if (Verifier_champ())
                    {
                        try
                        {


                            // Ouvrir la connexion à la base de données si elle n'est pas déjà ouverte
                            if (conDB.State != ConnectionState.Open)
                            {
                                conDB.Open();
                            }

                            // Préparer la commande SQL d'insertion
                            SqlCommand cmd = new SqlCommand("INSERT INTO Medecin(Prenom,Nom,Phone,email,Salaire,Specialite,Hopital) VALUES(@Prenom,@Nom,@Phone,@email,@Salaire,@Specialite,@Hopital)", conDB);

                            cmd.CommandType = CommandType.Text;

                            // Ajouter les paramètres à la commande
                            //cmd.Parameters.AddWithValue("@MedecinID", tbIDMedecin.Text);
                            cmd.Parameters.AddWithValue("@Prenom", tbPrenom.Text);
                            cmd.Parameters.AddWithValue("@Nom", tbnom.Text);
                            cmd.Parameters.AddWithValue("@Phone", tbNumeroContact.Text);
                            cmd.Parameters.AddWithValue("@email", tbCourriel.Text);
                            cmd.Parameters.AddWithValue("@Salaire", tbSalaire.Text);
                            cmd.Parameters.AddWithValue("@Specialite", tbSpecialite.Text);
                            cmd.Parameters.AddWithValue("@Hopital", tbHopital.Text);

                            // Exécuter la commande
                            cmd.ExecuteNonQuery();

                            // Recharger la liste des medecin
                            charger_Liste_Medecins();

                            lbMessage.Content = "Medecin ajoute dans la liste";
                        }
                        catch
                        {
                            lbMessage.Content = "Verifier connectionString: App.config";
                        }
                    }
                    else
                    {
                        throw new Exception("Veuillez saisir tous les champs!...");
                    }
                }
                else
                {
                    throw new Exception ( "Ce medecin existe dans la liste");
                }

            }
            catch(Exception ex)
            {
                lbMessage.Content = ex.Message;

            }
            
        }

        private void btnsuprimer_Med(object sender, RoutedEventArgs e)
        {
            // Votre code ici pour retirer un medecin de la base de donnees (BDD).
            //1. verifier si le medecin existe si oui retirez le de la BDD, sinon message pour dire qu'il n'existe pas
            // assurez vous de rafraichir la grille apres avoir retire le medecin.

            // Vérifier si le medecin existe dans la base de données et si le champ MedecinID n'est pas vide

            try
            {
                if (Medecin_Existant(tbNumeroContact.Text) && !string.IsNullOrEmpty(tbIDMedecin.Text))
                {
                    try
                    {


                        // Ouvrir la connexion à la base de données si elle n'est pas déjà ouverte
                        if (conDB.State != ConnectionState.Open)
                        {
                            conDB.Open();
                        }

                        // Préparer la requête SQL de suppression
                        string requete = "DELETE FROM Medecin WHERE MedecinID=@MedecinID";

                        SqlCommand cmd = new SqlCommand(requete, conDB);

                        // Ajouter le paramètre à la commande
                        cmd.Parameters.AddWithValue("@MedecinID", tbIDMedecin.Text);

                        cmd.CommandType = CommandType.Text;

                        // Exécuter la commande
                        cmd.ExecuteNonQuery();

                        // Recharger la liste des medecins
                        charger_Liste_Medecins();

                        // Fermer la connexion à la base de données
                        conDB.Close();

                        lbMessage.Content = "Medecin supprimé de la base de données";
                    }
                    catch
                    {

                        lbMessage.Content = "Verifier connectionString: App.config";
                    }

                }
                else
                {
                    throw new Exception ( "Aucun Medecin avec ce numero de contact dans la base de données");
                }

            }
            catch(Exception ex)
            {
                lbMessage.Content = ex.Message;

            }
        }

        private void Superieur(object sender, RoutedEventArgs e)
        {
            if (Salaire_Superieur_A.IsChecked == true) Salaire_Inferieur_A.IsChecked = false;
        }

        private void inferieur(object sender, RoutedEventArgs e)
        {
            if (Salaire_Inferieur_A.IsChecked == true) Salaire_Superieur_A.IsChecked = false;
        }

        private void Salaire_Consulter(object sender, RoutedEventArgs e)
        {

            // votre code pour  creer la requete en fonction du salaire
            // gerer les exceptions try catch
            // gerer les options : salaire superieur a - salaire inferieur a et salaire egale a:

            try
            {
                // Ouvrir la connexion à la base de données si elle n'est pas déjà ouverte
                if (conDB.State != ConnectionState.Open)
                {
                    conDB.Open();
                }

                // Initialiser la requête SQL à null
                string requete = null;

                // Vérifier si la case "Salaire supérieur à" est cochée
                if (Salaire_Superieur_A.IsChecked == true)
                {
                    // Vérifier si la zone de texte pour le salaire est vide
                    if (!string.IsNullOrEmpty(ctbSalaire.Text))
                    {
                        requete = "SELECT * FROM Medecin WHERE Salaire > @Salaire";

                        // Préparer la commande SQL
                        SqlCommand cmd = new SqlCommand(requete, conDB);

                        // Ajouter les paramètres à la commande
                        cmd.Parameters.AddWithValue("@Salaire", ctbSalaire.Text);

                        // Exécuter la commande et récupérer les données dans un lecteur de données
                        SqlDataReader dr = cmd.ExecuteReader();

                        // Créer un objet DataTable pour stocker les données
                        DataTable dataTable = new DataTable();

                        // Charger les données dans le DataTable à partir du lecteur de données
                        dataTable.Load(dr);

                        // Définir la source de données de la grille avec les données du DataTable
                        grille_consulter.ItemsSource = dataTable.DefaultView;
                        ConsMessage.Content = null;
                    }
                    else
                    {
                        ConsMessage.Content = "Veuillez saisir une valeur pour le salaire.";
                    }
                }
                // Vérifier si la case "Salaire inférieur à" est cochée
                else if (Salaire_Inferieur_A.IsChecked == true)
                {
                    // Vérifier si la zone de texte pour le salaire est vide
                    if (!string.IsNullOrEmpty(ctbSalaire.Text))
                    {
                        requete = "SELECT * FROM Medecin WHERE Salaire < @Salaire";

                        // Préparer la commande SQL
                        SqlCommand cmd = new SqlCommand(requete, conDB);

                        // Ajouter les paramètres à la commande
                        cmd.Parameters.AddWithValue("@Salaire", ctbSalaire.Text);

                        // Exécuter la commande et récupérer les données dans un lecteur de données
                        SqlDataReader dr = cmd.ExecuteReader();

                        // Créer un objet DataTable pour stocker les données
                        DataTable dataTable = new DataTable();

                        // Charger les données dans le DataTable à partir du lecteur de données
                        dataTable.Load(dr);

                        // Définir la source de données de la grille avec les données du DataTable
                        grille_consulter.ItemsSource = dataTable.DefaultView;
                        ConsMessage.Content = null;
                    }
                    else
                    {
                        ConsMessage.Content = "Veuillez saisir une valeur pour le salaire.";
                    }
                }
                else
                {
                    ConsMessage.Content = "Cochez une case entre supérieur et inférieur";
                }

                // Fermer la connexion à la base de données
                conDB.Close();
            }
            catch
            {
                ConsMessage.Content = "Veuillez vérifier la connectionString dans App.config";
            }

        }





        private void consulter_Nom_Prenom(object sender, RoutedEventArgs e)
        {
            // ici implementer la requete pour consultation par Nom et/ou Prenom
            // gestion des excpetions try catch
            // assurez vous d'implementer les cas possibles Nom / Prenom / Nom et Prenom / rien
            try
            {
                // Ouvrir la connexion à la base de données si elle n'est pas déjà ouverte
                if (conDB.State != ConnectionState.Open)
                {
                    conDB.Open();
                }

                if (string.IsNullOrEmpty(ctbNom.Text) && string.IsNullOrEmpty(ctbPrenom.Text))
                {
                    throw new InvalidOperationException("Veuillez renseigner le Nom ou le Prénom pour effectuer la recherche.");
                }

                // Initialiser la requête SQL à null
                string requete = null;

                // Construire dynamiquement la requête en fonction des critères de recherche non vides
                requete = (!string.IsNullOrEmpty(ctbNom.Text) ?
                       (!string.IsNullOrEmpty(ctbPrenom.Text) ?
                           "SELECT * FROM Medecin WHERE Nom=@Nom AND Prenom=@Prenom;" :
                           "SELECT * FROM Medecin WHERE Nom=@Nom;") :
                       (!string.IsNullOrEmpty(ctbPrenom.Text) ?
                           "SELECT * FROM Medecin WHERE Prenom=@Prenom;" :
                           "SELECT * FROM Medecin;"));
                // Si la requête est construite, l'exécuter
                if (requete != null)
                {
                    // Préparer la commande SQL
                    SqlCommand cmd = new SqlCommand(requete, conDB);

                    // Ajouter les paramètres à la commande
                    cmd.Parameters.AddWithValue("@Nom", ctbNom.Text);
                    cmd.Parameters.AddWithValue("@Prenom", ctbPrenom.Text);

                    // Exécuter la commande et récupérer les données dans un lecteur de données
                    SqlDataReader dr = cmd.ExecuteReader();

                    // Créer un objet DataTable pour stocker les données
                    DataTable dataTable = new DataTable();

                    // Charger les données dans le DataTable à partir du lecteur de données
                    dataTable.Load(dr);

                    // Définir la source de données de la grille avec les données du DataTable
                    grille_consulter.ItemsSource = dataTable.DefaultView;
                    ConsMessage.Content = null;
                }

            }

            catch (InvalidOperationException ex)
            {
                // Gérer l'exception pour les champs vides
                ConsMessage.Content = ex.Message;
            }
            catch (SqlException ex)
            {
                // Gérer l'exception SQL
                ConsMessage.Content = ex.Message;
            }
            finally
            {
                // Fermer la connexion à la base de données
                if (conDB.State == ConnectionState.Open)
                {
                    conDB.Close();
                }
            }


        }

        private void tbnModifier_Med(object sender, RoutedEventArgs e)
        {
            try
            {
                // Vérifier si le medeceni existe dans la base de données par son numero de contact
                if (Medecin_Existant(tbNumeroContact.Text))
                {
                    // Vérifier si tous les champs sont remplis
                    if (Verifier_champ())
                    {
                        try
                        {


                            // Ouvrir la connexion à la base de données si elle n'est pas déjà ouverte
                            if (conDB.State != ConnectionState.Open)
                            {
                                conDB.Open();
                            }

                            // Préparer la requête SQL de mise à jour
                            string requete = "UPDATE Medecin SET Prenom=@Prenom, Nom=@Nom," +
                              "Phone=@Phone, email=@email, Salaire=@Salaire, Specialite=@Specialite, Hopital=@Hopital WHERE Phone=@Phone";

                            SqlCommand cmd = new SqlCommand(requete, conDB);

                            // Ajouter les paramètres à la commande
                            cmd.Parameters.AddWithValue("@Prenom", tbPrenom.Text);
                            cmd.Parameters.AddWithValue("@Nom", tbnom.Text);
                            cmd.Parameters.AddWithValue("@Phone", tbNumeroContact.Text);
                            cmd.Parameters.AddWithValue("@email", tbCourriel.Text);
                            cmd.Parameters.AddWithValue("@Salaire", tbSalaire.Text);
                            cmd.Parameters.AddWithValue("@Specialite", tbSpecialite.Text);
                            cmd.Parameters.AddWithValue("@Hopital", tbHopital.Text);

                            // Exécuter la commande
                            cmd.ExecuteNonQuery();

                            // Recharger la liste des medecins
                            charger_Liste_Medecins();

                            // Fermer la connexion à la base de données
                            conDB.Close();

                            lbMessage.Content = "Informations de medecin mises à jour avec succès!";
                        }
                        catch
                        {
                            throw new Exception("Verifier connectionString: App.config");
                        }
                    }
                    else
                    {
                        throw new Exception("Veuillez saisir tous les champs!...");
                    }
                }
                else
                {
                    throw new Exception("Cette medecin n'existe pas dans l'inventaire");
                }

            }
            catch (Exception ex)
            {
                lbMessage.Content = ex.Message;
            }
        }
    }
}