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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace Ucitelsky_system
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //kolekce všech učitelů
        ObservableCollection<Ucitel> ucitele = new ObservableCollection<Ucitel>();

        //kolekce všech studentů
        ObservableCollection<Student> studenti = new ObservableCollection<Student>();

        //aktualně otevřený učitel
        Ucitel otevrenyucitel = null;
        SqlConnection sqlConnection;

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();

            //připojení k db
            string connectionString = ConfigurationManager.ConnectionStrings["Ucitelsky_system.Properties.Settings.SkolniDBString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);
            UkazVsechnyUcitele();


        }


        /// <summary>
        /// Zobrazení všech učitelů v levém spodním seznamu z databáze.
        /// </summary>
        public void UkazVsechnyUcitele()
        {
            try
            {
                ucitele.Clear();
                string dotaz = "Select * from Ucitel";
                SqlCommand prikaz = new SqlCommand(dotaz, sqlConnection);
                SqlDataReader dr;

                sqlConnection.Open();
                dr = prikaz.ExecuteReader();
                while (dr.Read())
                {
                    ucitele.Add(
                        new Ucitel(dr.GetString(dr.GetOrdinal("jmeno")),
                        dr.GetString(dr.GetOrdinal("prijmeni")),
                        (dr.GetDateTime(dr.GetOrdinal("datumNarozeni"))).Date,
                        dr.GetInt32(dr.GetOrdinal("Id"))
                        ));
                }
                dr.Close();
                seznamUcitelu.DisplayMemberPath = "celeJmeno";
                seznamUcitelu.SelectedValuePath = "Id";
                seznamUcitelu.ItemsSource = ucitele;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            finally
            {
                sqlConnection.Close();

            }
        }


        /// <summary>
        /// Zobrazí učitele, který je vybraný v levém spodním seznamu.
        /// </summary>
        private void ZobrazUcitele_Click(object sender, SelectionChangedEventArgs e)
        {
            ucitel.Visibility = Visibility.Visible;
            odebiraniUc.Visibility = Visibility.Hidden;
            pridavaniUc.Visibility = Visibility.Hidden;
            odebiraniSt.Visibility = Visibility.Hidden;
            pridavaniSt.Visibility = Visibility.Hidden;

            foreach (Ucitel u in ucitele)
            {
                if (seznamUcitelu.SelectedValue.ToString()==u.Id)
                {
                    otevrenyucitel = u.deepCopy();
                }
            }
            jmeno.Text = otevrenyucitel.VratCeleJmeno();
            datum.Text = otevrenyucitel.datumNarozeni.ToShortDateString();

            ZobrazPredmety();

            try
            {
                string query = "Select * from Trida";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable tabulkaTridy = new DataTable();
                    sqlDataAdapter.Fill(tabulkaTridy);
                    vsechnyTridy.DisplayMemberPath = "nazev";
                    vsechnyTridy.SelectedValuePath = "Id";
                    vsechnyTridy.ItemsSource = tabulkaTridy.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Zobrazení předmětů učitele.
        /// </summary>
        public void ZobrazPredmety()
        {
            try
            {

                List<Predmet> predmety = null;
                predmety = new List<Predmet>();
                string dotaz = "Select * from Predmet where IdUcitele = @IdUcitele";
                SqlCommand prikaz = new SqlCommand(dotaz, sqlConnection);
                SqlDataReader dr;
                prikaz.Parameters.AddWithValue("@IdUcitele", otevrenyucitel.Id);
                sqlConnection.Open();
                dr = prikaz.ExecuteReader();
                
                while (dr.Read())
                {
                    predmety.Add(
                        new Predmet(
                        dr.GetInt32(dr.GetOrdinal("id")),
                        dr.GetString(dr.GetOrdinal("nazev")),
                        dr.GetTimeSpan(dr.GetOrdinal("zacatek")),
                        dr.GetInt32(dr.GetOrdinal("trvani")),
                        dr.GetInt32(dr.GetOrdinal("Id"))
                        ));
                }

                dr.Close();
                Predmety.SelectedValuePath = "id";
                Predmety.DisplayMemberPath = "nazev";
                Predmety.ItemsSource = predmety;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        /// <summary>
        /// Zobrazí formulář pro přidání učitele.
        /// </summary>
        private void ZobrazPridavaciFormularUc_Click(object sender, RoutedEventArgs e)
        {
            pridavaniUc.Visibility = Visibility.Visible;
            odebiraniUc.Visibility = Visibility.Hidden;
            ucitel.Visibility = Visibility.Hidden;
            odebiraniSt.Visibility = Visibility.Hidden;
            pridavaniSt.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Přidání učitele
        /// </summary>
        private void Pridej_Click(object sender, RoutedEventArgs e)
        {
            string jmeno = pridejJmeno.Text;
            string prijmeni = pridejPrijmeni.Text;
            string datum = pridejDatum.SelectedDate.ToString();
            if (!(jmeno.Equals("")) && !(prijmeni.Equals("")) && !(datum is null))
            {
                try
                {
                    string query = "insert into Ucitel values (@jmeno, @prijmeni, @datum)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@jmeno", jmeno);
                    sqlCommand.Parameters.AddWithValue("@prijmeni", prijmeni);
                    sqlCommand.Parameters.AddWithValue("@datum", pridejDatum.SelectedDate);
                    sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    UkazVsechnyUcitele();
                }
            }
            else
            {
                MessageBox.Show("Prosím vyplňte všechna pole.");
            }
        }



        /// <summary>
        /// Odebrání učitele
        /// </summary>
        private void Odeber_Click(object sender, RoutedEventArgs e)
        {
            if (!(uciteleKOdebrani.SelectedValue is null)) {
                try
                {
                    string query = "delete from Ucitel where Id = @UcitelId";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@UcitelId", uciteleKOdebrani.SelectedValue);
                    sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    UkazVsechnyUcitele();
                }
            }
            else
            {
                MessageBox.Show("Vyberte prosím učitele, kterého chcete odebrat.");
            }
        }


        /// <summary>
        /// Zobrazí formulář pro odebrání učitele
        /// </summary>
        private void ZobrazOdebiraciFormularUc_Click(object sender, RoutedEventArgs e)
        {
            odebiraniUc.Visibility = Visibility.Visible;
            pridavaniUc.Visibility = Visibility.Hidden;
            ucitel.Visibility = Visibility.Hidden;
            odebiraniSt.Visibility = Visibility.Hidden;
            pridavaniSt.Visibility = Visibility.Hidden;

            uciteleKOdebrani.SelectedValuePath = "Id";
            uciteleKOdebrani.ItemsSource = ucitele;
        }

        /// <summary>
        /// Přidá k učitelovi nový předmět.
        /// </summary>
        private void PridejPredmet_Click(object sender, RoutedEventArgs e)
        {
            string nazev = nazevNovehoPredmetu.Text;
            string zacatekHod = zacatekNovehoPredmetuHod.Text;
            string zacatekMin = zacatekNovehoPredmetuHod.Text;
            int zacatekHodInt;
            int zacatekMinInt;
            int trvaniInt;
            string trvani = trvaniNovehoPredmetu.Text;
            bool jdePrevestTrvani = Int32.TryParse(zacatekHod, out trvaniInt);
            bool jdePrevestHod = Int32.TryParse(zacatekHod, out zacatekHodInt);
            bool jdePrevestMin = Int32.TryParse(zacatekMin, out zacatekMinInt);

            if ((!nazev.Equals("")) && (!trvani.Equals(""))&& jdePrevestHod&& jdePrevestMin&& zacatekHodInt>0&& zacatekMinInt>0&&trvaniInt>0)
            {
                TimeSpan zacatek = TimeSpan.Parse(zacatekHod + ":" + zacatekMin);
                try
                {
                    string query = "insert into Predmet values (@nazev, @zacatek, @trvani, @IdUcitele)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@nazev", nazev);
                    sqlCommand.Parameters.AddWithValue("@zacatek", zacatek);
                    sqlCommand.Parameters.AddWithValue("@trvani", trvaniInt);
                    sqlCommand.Parameters.AddWithValue("@IdUcitele", otevrenyucitel.Id);
                    sqlCommand.ExecuteScalar();
                    sqlConnection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    ZobrazPredmety();
                    zacatekNovehoPredmetuHod.Text = "";
                    zacatekNovehoPredmetuHod.Text = "";
                    nazevNovehoPredmetu.Text = "";
                    trvaniNovehoPredmetu.Text = "";
                }
            }
            else
            {
                MessageBox.Show("Vyplňte správně všechna pole.");
            }
        }

        /// <summary>
        /// Zavolá metody "ZrobrazTridyVPredmetu" a "ZrobrazStudentyVPredmetu".
        /// </summary>
        private void ZrobrazPredmet_Click(object sender, SelectionChangedEventArgs e)
        {
            ZrobrazTridyVPredmetu();
            ZrobrazStudentyVPredmetu();
        }

        /// <summary>
        /// Zobrazí v seznamu "Tridy" třídy, které byli k předmětu zapsány.
        /// </summary>
        public void ZrobrazTridyVPredmetu()
        {
            try
            {
                if (!(Predmety.SelectedValue is null))
                {
                    string query = "Select * from Trida where Id in (Select IdTrida from TridaPredmet where idPredmet = @idPredmet)";

                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    using (sqlDataAdapter)
                    {
                        sqlCommand.Parameters.AddWithValue("@idPredmet", Predmety.SelectedValue);

                        DataTable tabulkaTrid = new DataTable();
                        sqlDataAdapter.Fill(tabulkaTrid);
                        Tridy.DisplayMemberPath = "nazev";
                        Tridy.SelectedValuePath = "Id";
                        Tridy.ItemsSource = tabulkaTrid.DefaultView;
                    }
                }
                else
                {
                    Tridy.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Predmety.SelectedValue.ToString());
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// Zobrazí v seznamu "Studenti" studenty, kteří jsou zapsáni v jednotlivých třídách, které jsou zapsány v daném předmětu.
        /// </summary>
        public void ZrobrazStudentyVPredmetu()
        {

            try
            {
                if (!(Predmety.SelectedValue is null))
                {
                    string query = "Select * from student where tridaId in (Select IdTrida from TridaPredmet where idPredmet = @idPredmet)";

                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                    using (sqlDataAdapter)
                    {
                        sqlCommand.Parameters.AddWithValue("@idPredmet", Predmety.SelectedValue);

                        DataTable tabulkaStudentu = new DataTable();
                        sqlDataAdapter.Fill(tabulkaStudentu);
                        Studenti.DisplayMemberPath = "jmeno";
                        Studenti.SelectedValuePath = "Id";
                        Studenti.ItemsSource = tabulkaStudentu.DefaultView;
                    }
                }
                else
                {
                    Studenti.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// Odstraní předmět 
        /// </summary>
            private void OdstranPredmet_Click(object sender, RoutedEventArgs e)
        {
            if (!(Predmety.SelectedValue is null)) {
                try
                {
                    string query = "delete from Predmet where Id = (@Id)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();

                    sqlCommand.Parameters.AddWithValue("@Id", Predmety.SelectedValue);
                    sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    ZobrazPredmety();
                }
            }
            else
            {
                MessageBox.Show("Nejprve vyberte předmět který chcete odstranit.");
            }
            ZrobrazTridyVPredmetu();
            ZrobrazStudentyVPredmetu();
        }

        /// <summary>
        /// Zobrazí formulář pro přidání studenta.
        /// </summary>
        private void ZobrazPridavaciFormularSt_Click(object sender, RoutedEventArgs e)
        {
            pridavaniSt.Visibility = Visibility.Visible;
            odebiraniSt.Visibility = Visibility.Hidden;
            odebiraniUc.Visibility = Visibility.Hidden;
            pridavaniUc.Visibility = Visibility.Hidden;
            ucitel.Visibility = Visibility.Hidden;

            try
            {
                string query = "Select * from Trida";

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable tabulkaTridy = new DataTable();
                    sqlDataAdapter.Fill(tabulkaTridy);
                    pridejTrida.DisplayMemberPath = "nazev";
                    pridejTrida.SelectedValuePath = "Id";
                    pridejTrida.ItemsSource = tabulkaTridy.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        /// <summary>
        /// Zobrazí formulář pro odebrání studenta.
        /// </summary>
        private void ZobrazOdebiraciFormularSt_Click(object sender, RoutedEventArgs e)
        {
            odebiraniSt.Visibility = Visibility.Visible;
            pridavaniSt.Visibility = Visibility.Hidden;
            odebiraniUc.Visibility = Visibility.Hidden;
            pridavaniUc.Visibility = Visibility.Hidden;
            ucitel.Visibility = Visibility.Hidden;

            ukazVsechnyStudenty();

        }

        /// <summary>
        /// Vypíše do seznamu "studentiKOdebrani" všechny studenty z databáze.
        /// </summary>
        public void ukazVsechnyStudenty()
        {
            try
            {
                studenti.Clear();
                string dotaz = "Select * from student";
                SqlCommand prikaz = new SqlCommand(dotaz, sqlConnection);
                SqlDataReader dr;

                sqlConnection.Open();
                dr = prikaz.ExecuteReader();
                while (dr.Read())
                {
                    studenti.Add(
                        new Student(dr.GetString(dr.GetOrdinal("jmeno")),
                        dr.GetString(dr.GetOrdinal("prijmeni")),
                        (dr.GetDateTime(dr.GetOrdinal("datumNarozeni"))).Date,
                        dr.GetInt32(dr.GetOrdinal("Id"))
                        ));
                }
                dr.Close();
                studentiKOdebrani.DisplayMemberPath = "celeJmeno";
                studentiKOdebrani.SelectedValuePath = "Id";
                studentiKOdebrani.ItemsSource = studenti;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                sqlConnection.Close();


            }
        }

        /// <summary>
        /// Přidá studenta do databáze.
        /// Pokud není nějaké pole vypněné, zobrazí chybovou hlášku.
        /// </summary>
        private void PridejSt_Click(object sender, RoutedEventArgs e)
        {
            string jmeno = pridejJmenoSt.Text;
            string prijmeni = pridejPrijmeniSt.Text;
            string datum = pridejDatumSt.Text;
            object trida = pridejTrida.SelectedValue;
            if (!(jmeno.Equals(""))&&!(prijmeni.Equals(""))&&!(datum is null)&&!(trida is null))
            {
                try
                {
                    string query = "insert into student values (@jmeno, @prijmeni, @datum, @tridaId)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@jmeno", jmeno);
                    sqlCommand.Parameters.AddWithValue("@prijmeni", prijmeni);
                    sqlCommand.Parameters.AddWithValue("@datum", pridejDatumSt.SelectedDate);
                    sqlCommand.Parameters.AddWithValue("@tridaId", trida);
                    sqlCommand.ExecuteScalar();

                    MessageBox.Show("Student byl úspěšně přidán.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();

                }
            }
            else
            {
                MessageBox.Show("Nejprve vyplňte všechny informace.");
            }
        }

        /// <summary>
        /// Odeber studenta z databáze.
        /// Pokud není student zvolený, zobrazí chybovou hlášku.
        /// </summary>
        private void odeberSt_Click(object sender, RoutedEventArgs e)
        {
            if (!(studentiKOdebrani.SelectedValue is null))
            {
                try
                {
                    string query = "delete from Student where Id = @StudentId";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@StudentId", studentiKOdebrani.SelectedValue);
                    sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    ukazVsechnyStudenty();
                }
            }
        }

        /// <summary>
        /// Odstraní třídu z databáze.
        /// Pokud nejsou vybrány předmět a třída, zobrazí chybovou hlášku.
        /// </summary>
        private void OdstranTridu_Click(object sender, RoutedEventArgs e)
        {
            if (!(Tridy.SelectedValue is null)&&!(Predmety.SelectedValue is null)) {
                try
                {
                    string query = "delete from TridaPredmet where IdTrida = @IdTrida and IdPredmet = @IdPredmet";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@IdTrida", Tridy.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@IdPredmet", Predmety.SelectedValue);
                    sqlCommand.ExecuteScalar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    ZrobrazTridyVPredmetu();
                    ZrobrazStudentyVPredmetu();
                }
            }
            else
            {
                MessageBox.Show("Nejdřive vyberte předmět a třídu");
            }
        }

        /// <summary>
        /// Přidá třídu do databáze.
        /// Pokud není zbolená třída nebo předmě, zobrazí chybovou hlášku.
        /// </summary>
        private void PridejTriduDoPredmetu_Click(object sender, RoutedEventArgs e)
        {
            if (!(vsechnyTridy.SelectedValue is null)&&!(Predmety.SelectedValue is null)) {
                try
                {
                    string query = "insert into TridaPredmet values (@idTrida, @idPredmet)";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@idTrida", vsechnyTridy.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@idPredmet", Predmety.SelectedValue);

                    sqlCommand.ExecuteScalar();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    sqlConnection.Close();
                    ZrobrazTridyVPredmetu();
                    ZrobrazStudentyVPredmetu();
                }
            }
            else
            {
                MessageBox.Show("Nejdříve vyberte třídu a předmět.");
            }
        }
    }
}
