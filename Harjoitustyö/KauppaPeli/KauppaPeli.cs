using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace KauppaPeli;

/// @author osterava
/// @version 4.11.2022
/// <summary>
///  Peli, jossa keräillään tuotteita omaan ostoskärryyn
/// </summary>
// https://tim.jyu.fi/view/kurssit/tie/ohj1/2022s/demot/demo10?answerNumber=25&b=oLNB7gIsCK6M&size=1&task=tauno&user=osterava#tauno
public class KauppaPeli : PhysicsGame
{
    private IntMeter pistelaskuri;
    private IntMeter tuotelaskuri;
    private DoubleMeter elamalaskuri;
    private EasyHighScore topkyba = new EasyHighScore();
    private bool loppu = false;
    private PhysicsObject tuote;

    public override void Begin()
    {
        
        ClearAll();
        loppu = false;

        BoundingRectangle koko = new BoundingRectangle(new Vector(Level.Left, - 200), Level.BoundingRect.TopRight);
        //TODO:Seinästä kimpoaminen objekteille vihaisetasiakkaat ja tuotteet

        for (int i = 0; i < 10; i++) // juomat
        {
            PhysicsObject juomat = LuoJuomat(this, koko, 30, "tuotteet");
            juomat.Image = LoadImage("juoma.png"); // creative commons kuva linkki https://fi.depositphotos.com/52847715/stock-photo-angry-customer-at-supermarket.html
                                                               // tekijä stokkete
        }


        PhysicsObject pelaaja = LuoJuomat(this, koko, 30, "pelaaja");
        pelaaja.Image = LoadImage("pelaaja");



        Level.Background.Image = LoadImage("background2.jpeg"); //Cretive commons kuva linkki https://fi.depositphotos.com/10626948/stock-photo-supermarket.html
        Level.BackgroundColor = Color.White;
        Level.CreateBorders();

        Timer.CreateAndStart(1, LuoTuote);
        Timer.CreateAndStart(1.2, LuoAsiakas);

        AddCollisionHandler(pelaaja, "vihu", TormasiAsiakkaasen);
        AddCollisionHandler(pelaaja, "tuotteet", KerasiTuotteen);

        LuoPistelaskuri();
        LuoElamalaskuri();
        LisaaOhjaimet(pelaaja);
        List<int> lista = new List<int>(0);
        int lkt = LaskeKeratytTuotteet(lista,tuote, pelaaja,"tuotteet");
        LuoTuoteLaskuri(lkt);

    }

    private int LaskeKeratytTuotteet(List<int> l,PhysicsObject kerattava,PhysicsObject keraaja, string tunniste)
    {
        if (pistelaskuri.Value > l.Count)
        {
           tunniste.Append(l);  
        }
        MessageDisplay.Add($"Tuotteita kerätty {l.Count} ");

        return l.Count;
    }

    /// <summary>
    ///     Luo pelaajalle kerättäviä juomia
    /// </summary>
    /// <param name="peli">mihin peliin ympyrä luodaan</param>
    /// <param name="umpyra">minkä ymppyrän sisälle luodaan eli paikka</param>
    /// <param name="vauhti">ympyrän vauhti alussa</param>
    /// <param name="tunniste">Tunniste tag</param>
    private static PhysicsObject LuoJuomat(PhysicsGame peli, BoundingRectangle umpyra, double vauhti, string tunniste)
    {
        double leveys = RandomGen.NextDouble(50, 50);
        double korkeus = RandomGen.NextDouble(50, 50);
        PhysicsObject juoma = new PhysicsObject(leveys, korkeus, Shape.Circle);
        juoma.Position = RandomGen.NextVector(umpyra);
        juoma.Tag = "tuotteet";
        peli.Add(juoma);
        return juoma;
    }


    /// <summary>
    ///     Lyödään ympyrää eli pelaajaa voimavektorilla 
    /// </summary>
    /// <param name="ympyra">lyötävä kohde</param>
    /// <param name="mihin">voimavektori</param>
    private static void LiikutaPelaajaa(PhysicsObject ympyra, Vector mihin)
    {
        ympyra.Hit(mihin);
    }


    /// <summary>
    ///     Metodi, johon tullan, kun pelaaja osuu johonkin vihaiseen asiakkaaseen
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="vihu"></param>
    private void TormasiAsiakkaasen(PhysicsObject pelaaja, PhysicsObject vihu)
    {
        Explosion rajahdys = new Explosion(vihu.Width * 2);
        rajahdys.Position = vihu.Position;
        rajahdys.UseShockWave = false;
        this.Add(rajahdys);
        elamalaskuri.Value -= 1;
        MessageDisplay.Add("VARO VIHAISIA ASIAKKAITA");
    }


    /// <summary>
    ///     Metodi,johon tullaan, kun pelaaja kerää tuotteen
    /// </summary>
    /// <param name="pelaaja"> kuka kerää</param>
    /// <param name="tuote"> minkä tuotteen </param>
    private void KerasiTuotteen(PhysicsObject pelaaja, PhysicsObject tuote)
    {
        tuote.Destroy();
        pistelaskuri.Value += 2;
        elamalaskuri.Value += 1;
    }


    /// <summary>
    ///     Luo pelaajalle pistelaskurin
    /// </summary>
    private void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);

        Label pistenaytto = new Label();
        pistenaytto.X = Screen.Left + 100;
        pistenaytto.Y = Screen.Top - 100;
        pistenaytto.TextColor = Color.White;
        pistenaytto.Color = Color.Black;
        pistenaytto.Title = " Pisteet ";
        pistenaytto.BindTo(pistelaskuri);
        Add(pistenaytto);
    }


    /// <summary>
    ///     Luo pelaajalle elamalaskurin
    /// </summary>
    private void LuoElamalaskuri()
    {
        elamalaskuri = new DoubleMeter(3);
        elamalaskuri.MaxValue = 3;
        elamalaskuri.LowerLimit += Havio;

        ProgressBar elamapalkki = new ProgressBar(150,30);
        elamapalkki.X = Level.Right - 100;
        elamapalkki.Y = Level.Top - 100;
        elamapalkki.BarColor = Color.Red;
        elamapalkki.BorderColor = Color.Black;
        elamapalkki.Color = Color.Black;

        elamapalkki.BindTo(elamalaskuri);
        Add(elamapalkki);
    }


    /// <summary>
    ///     Metodi,tapahtuu, kun pelaaja häviää
    /// </summary>
    private void Havio()
    {
        if (loppu) return;
        loppu = true;
        MessageDisplay.Add("Elämät loppuivat, peli päättyy");
        topkyba.EnterAndShow(pistelaskuri.Value);
        topkyba.HighScoreWindow.Closed += AloitaPeli;
        
    }


    /// <summary>
    /// Annetaan pelaajalle vaihtoehtoja, eli mitä haluaa tehdä
    /// </summary>
    /// <param name="Sender">mistä tullaan</param>
    private void AloitaPeli(Window Sender)
    {

        string[] vaihtoehdot = { "Aloita peli", "Top kybä", "Lopeta peli" };
        MultiSelectWindow alkuvalikko = new MultiSelectWindow("Alkuvalikko", vaihtoehdot);
        alkuvalikko.ItemSelected += delegate (int valittu)
        {
            if (valittu == 0) Begin();
            if (valittu == 1) TopKymppi();
            if (valittu == 2) Exit();
        };
        Add(alkuvalikko);

    }


    /// <summary>
    /// Tullaan, kun valitaan vaihtoehdoista TOPKYMPPI
    /// </summary>
    private void TopKymppi()
    {
        topkyba.Show();
        topkyba.HighScoreWindow.Closed += AloitaPeli;
    }

    /// <summary>
    /// Luo timerin avulla luotteet
    /// </summary>
    private void LuoTuote()
    {
        double leveys = RandomGen.NextDouble(50, 50);
        double korkeus = RandomGen.NextDouble(50, 50);
        PhysicsObject tuotteet = new PhysicsObject(leveys, korkeus, Shape.Circle);
        tuotteet.Position = RandomGen.NextVector(Level.Bottom,Level.Top) ;
        tuotteet.Tag = "tuotteet";
        tuotteet.Image = LoadImage("tuotteet.ong.png");
        tuotteet.LifetimeLeft = TimeSpan.FromSeconds(9);
        Vector liike = RandomGen.NextVector(0, 0);
        tuotteet.Hit(liike);
        Add(tuotteet);
    }


    /// <summary>
    ///     Luo timerin avulla peliin asiakkaita
    /// </summary>
    private void LuoAsiakas()
    {
        double leveys = RandomGen.NextDouble(50, 50);
        double korkeus = RandomGen.NextDouble(50, 50);
        PhysicsObject asiakkaat = new PhysicsObject(leveys, korkeus, Shape.Circle);
        asiakkaat.Position = RandomGen.NextVector(Level.Bottom,Level.Top);
        asiakkaat.Tag = "vihu";
        asiakkaat.Image = LoadImage("asiakas");
        asiakkaat.LifetimeLeft = TimeSpan.FromSeconds(8);
        Vector liike = RandomGen.NextVector(0, 30);
        asiakkaat.Hit(liike);
        Add(asiakkaat);
    }


    /// <summary>
    ///     Lisää peliin ohjaimet
    /// </summary>
    /// <param name="kenelle">kenelle lisätään</param>
    private void LisaaOhjaimet(PhysicsObject kenelle)
    {
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä avustus");
        Keyboard.Listen(Key.Up, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja ylös", kenelle, new Vector(0, 200));
        Keyboard.Listen(Key.Down, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja alas", kenelle, new Vector(0, -200));
        Keyboard.Listen(Key.Left, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja vasemmalle", kenelle, new Vector(-200, 0));
        Keyboard.Listen(Key.Right, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja oikealle", kenelle, new Vector(200, 0));

    }
    private void LuoTuoteLaskuri(int lkm)
    {
        tuotelaskuri = new IntMeter(100);
        tuotelaskuri.Value = lkm;

        Label pistenaytto = new Label();
        pistenaytto.X = 0;
        pistenaytto.Y = 0;
        pistenaytto.TextColor = Color.White;
        pistenaytto.Color = Color.Black;
        pistenaytto.Title = "Tuotteita Kerätty ";
        pistenaytto.BindTo(tuotelaskuri);
        Add(pistenaytto);
    }
}



