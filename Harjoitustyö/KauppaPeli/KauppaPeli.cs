using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace KauppaPeli;

public class KauppaPeli : PhysicsGame
{
    IntMeter pistelaskuri;
    DoubleMeter elamalaskuri;
    private EasyHighScore topkyba = new EasyHighScore();
    private bool loppu = false;

    public override void Begin()
    {
        ClearAll();
        loppu = false;

        BoundingRectangle koko = new BoundingRectangle(new Vector(Level.Left, - 200), Level.BoundingRect.TopRight);
        //TODO:Seinästä kimpoaminen objekteille vihaisetasiakkaat ja tuotteet

        for (int i = 0; i < 10; i++) // vihaiset asiakkat
        {
            PhysicsObject vihaisetAsiakkaat = LuoSatunaisetAsiakkaat(this, koko, 30, "vihu");
            vihaisetAsiakkaat.Image = LoadImage("vihasiakas"); // creative commons kuva linkki https://fi.depositphotos.com/52847715/stock-photo-angry-customer-at-supermarket.html
                                                               // tekijä stokkete


        }

        for (int i = 0; i < 20; i++) // kerättävät tuotteet
        {
            PhysicsObject tuotteet = LuoTuotteet(this, koko, 0, "tuotteet");
            tuotteet.Image = LoadImage("tuotteet"); //Creative commons kuva linkki https://fi.depositphotos.com/47120155/stock-photo-shopping-basket-with-groceries.html
                                                    //Tekijä: chressiesjd
        }

        PhysicsObject pelaaja = LuoSatunaisetAsiakkaat(this, koko, 30, "pelaaja");
        pelaaja.Image = LoadImage("pelaaja");

        Level.Background.Image = LoadImage("background.jpeg"); //Cretive commons kuva linkki https://fi.depositphotos.com/10626948/stock-photo-supermarket.html
        Level.BackgroundColor = Color.White;
        Level.CreateBorders();

        Timer.CreateAndStart(1.5, LuoTuote);
        Timer.CreateAndStart(1.5, LuoAsiakas);

        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");

        AddCollisionHandler(pelaaja, "vihu", TormasiAsiakkaasen);
        AddCollisionHandler(pelaaja, "tuotteet", KerasiTuotteen);
        LuoPistelaskuri();
        LuoElamalaskuri();
        LisaaOhjaimet(pelaaja);
    }

    /// <summary>
    /// Aliohjelma luo vihamieliset asiakkaat
    /// </summary>
    /// <param name="peli">mihin peliin ympyrä luodaan</param>
    /// <param name="umpyra">minkä ymppyrän sisälle luodaan eli paikka</param>
    /// <param name="vauhti">ympyrän vauhti alussa</param>
    /// <param name="tunniste">Tunniste tag</param>
    public static PhysicsObject LuoSatunaisetAsiakkaat(PhysicsGame peli, BoundingRectangle umpyra, double vauhti, string tunniste)
    {
        double leveys = RandomGen.NextDouble(50, 50);
        double korkeus = RandomGen.NextDouble(50, 50);
        Vector liike = RandomGen.NextVector(0, vauhti);
        PhysicsObject asiakkaat = new PhysicsObject(leveys, korkeus, Shape.Circle);
        asiakkaat.Position = RandomGen.NextVector(umpyra);
        asiakkaat.Hit(liike);
        asiakkaat.Tag = "vihu";
        peli.Add(asiakkaat);
        return asiakkaat;
    }
    /// <summary>
    /// lyödään kolmiota eli pelaajaa voimavektorilla 
    /// </summary>
    /// <param name="ympyra">lyötävä kohde</param>
    /// <param name="mihin">voimavektori</param>
    public static void LiikutaPelaajaa(PhysicsObject ympyra, Vector mihin)
    {
        ympyra.Hit(mihin);
    }
    /// <summary>
    /// Aliohjelma luo vihamieliset asiakkaat
    /// </summary>
    /// <param name="peli">mihin peliin ympyrä luodaan</param>
    /// <param name="umpyra">minkä ymppyrän sisälle luodaan eli paikka</param>
    /// <param name="vauhti">ympyrän vauhti alussa</param>
    /// <param name="tunniste">Tunniste tag</param>
    public static PhysicsObject LuoTuotteet(PhysicsGame peli, BoundingRectangle umpyra, double vauhti, string tunniste)
    {
        double leveys = RandomGen.NextDouble(50, 50);
        double korkeus = RandomGen.NextDouble(50, 50);
        Vector liike = RandomGen.NextVector(0, vauhti);
        PhysicsObject tuotteet = new PhysicsObject(leveys, korkeus, Shape.Circle);
        tuotteet.Position = RandomGen.NextVector(umpyra);
        tuotteet.Color = Color.Blue;
        tuotteet.Tag = "tuotteet";
        tuotteet.Hit(liike);
        peli.Add(tuotteet);
        return tuotteet;

    }

    /// <summary>
    /// Metodi, johon tullan, kun pelaaja osuu johonkin vihaiseen asiakkaaseen
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="vihu"></param>
    public void TormasiAsiakkaasen(PhysicsObject pelaaja, PhysicsObject vihu)
    {
        Explosion rajahdys = new Explosion(vihu.Width * 2);
        rajahdys.Position = vihu.Position;
        rajahdys.UseShockWave = false;
        this.Add(rajahdys);
        elamalaskuri.Value -= 1;
        MessageDisplay.Add("Varo vihaisia asiakkaita");
    }

    /// <summary>
    /// Tullaan, kun pelaaja kerää tuotteen
    /// </summary>
    /// <param name="pelaaja"> kuka kerää</param>
    /// <param name="tuote"> minkä tuotteen </param>
    public void KerasiTuotteen(PhysicsObject pelaaja, PhysicsObject tuote)
    {
        tuote.Destroy();
        pistelaskuri.Value += 1;
        elamalaskuri.Value += 1;
    }

    /// <summary>
    /// Luo pelaajalle pistelaskurin
    /// </summary>
    void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);

        Label pistenaytto = new Label();
        pistenaytto.X = Screen.Left + 100;
        pistenaytto.Y = Screen.Top - 100;
        pistenaytto.TextColor = Color.White;
        pistenaytto.Color = Color.DarkBlue;
        pistenaytto.Title = "Pisteesi ";
        pistenaytto.BindTo(pistelaskuri);
        Add(pistenaytto);
    }

    /// <summary>
    /// Luo pelaajalle elamalaskurin
    /// </summary>
    private void LuoElamalaskuri()
    {
        elamalaskuri = new DoubleMeter(10);
        elamalaskuri.MaxValue = 4;
        elamalaskuri.LowerLimit += Havio;

        ProgressBar elamapalkki = new ProgressBar(150, 20);
        elamapalkki.X = Screen.Right + 150;
        elamapalkki.Y = Screen.Top - 20;
        elamapalkki.BarColor = Color.Red;
        elamapalkki.BorderColor = Color.Black;
        elamapalkki.Color = Color.DarkBlue;

        elamapalkki.BindTo(elamalaskuri);
        Add(elamapalkki);
    }

    /// <summary>
    /// tapahtuu, kun pelaaja häviää
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
        tuotteet.Image = LoadImage("tuotteet");
        tuotteet.LifetimeLeft = TimeSpan.FromSeconds(7);
        Vector liike = RandomGen.NextVector(0, 0);
        tuotteet.Hit(liike);
        Add(tuotteet);
    }

    /// <summary>
    /// Luo timerin avulla peliin asiakkaita
    /// </summary>
    private void LuoAsiakas()
    {
        double leveys = RandomGen.NextDouble(50, 50);
        double korkeus = RandomGen.NextDouble(50, 50);
        PhysicsObject asiakkaat = new PhysicsObject(leveys, korkeus, Shape.Circle);
        asiakkaat.Position = RandomGen.NextVector(Level.Bottom,Level.Top);
        asiakkaat.Tag = "vihu";
        asiakkaat.Image = LoadImage("vihasiakas");
        asiakkaat.LifetimeLeft = TimeSpan.FromSeconds(5);
        Vector liike = RandomGen.NextVector(0, 30);
        asiakkaat.Hit(liike);
        Add(asiakkaat);
    }

    /// <summary>
    /// Lisää peliin ohjaimet
    /// </summary>
    /// <param name="kenelle">kenelle lisätään</param>
    private void LisaaOhjaimet(PhysicsObject kenelle)
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä avustus");
        Keyboard.Listen(Key.Up, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja ylös", kenelle, new Vector(0, 200));
        Keyboard.Listen(Key.Down, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja alas", kenelle, new Vector(0, -200));
        Keyboard.Listen(Key.Left, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja vasemmalle", kenelle, new Vector(-200, 0));
        Keyboard.Listen(Key.Right, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja oikealle", kenelle, new Vector(200, 0));

    }
}



