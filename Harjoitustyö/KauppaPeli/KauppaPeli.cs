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

    public override void Begin()
    {
        
        BoundingRectangle alaosa = new BoundingRectangle(new Vector(Level.Left, 0), Level.BoundingRect.BottomRight);
        BoundingRectangle ylaosa = new BoundingRectangle(Level.BoundingRect.TopLeft, new Vector(Level.Right, 0));
        //TODO:BoundingRectangle kokopeli = new BoundingRectangle(Level.BoundingRect.BottomLeft, new Vector(Level.TopRight, 0)); ohjaus?'
        //TODO:Seinästä kimpoaminen objekteille vihaisetasiakkaat ja tuotteet

        for (int i = 0; i < 15; i++) // vihaiset asiakkat
        {
            PhysicsObject vihaisetAsiakkaat = LuoSatunaisetAsiakkaat(this, alaosa, 60, "vihu");
            vihaisetAsiakkaat.Image = LoadImage("vihasiakas"); // creative commons kuva linkki https://fi.depositphotos.com/52847715/stock-photo-angry-customer-at-supermarket.html
                                                               // tekijä stokkete


        }

        for (int i = 0; i < 20; i++) //kerättävät tuotteet
        {
            PhysicsObject tuotteet = LuoTuotteet(this, alaosa, 60,"tuotteet");
            tuotteet.Image = LoadImage("tuotteet"); //Creative commons kuva linkki https://fi.depositphotos.com/47120155/stock-photo-shopping-basket-with-groceries.html
                                                    //Tekijä: chressiesjd

        }

        PhysicsObject pelaaja = LuoSatunaisetAsiakkaat(this, ylaosa, 50, "pelaaja");
        pelaaja.Image = LoadImage("pelaaja");

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Level.Background.Image = LoadImage("background.jpeg"); //Cretive commons kuva linkki https://fi.depositphotos.com/10626948/stock-photo-supermarket.html
                                                               //Tekijä: gyn9037
        Level.BackgroundColor = Color.White;
        Level.CreateBorders();

        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä avustus");
        Keyboard.Listen(Key.Up, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja ylös", pelaaja, new Vector(0, 200));
        Keyboard.Listen(Key.Down, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja alas", pelaaja, new Vector(0, -200));
        Keyboard.Listen(Key.Left, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja vasemmalle", pelaaja, new Vector(-200, 0));
        Keyboard.Listen(Key.Right, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja oikealle", pelaaja, new Vector(200, 0));

        AddCollisionHandler(pelaaja, "vihu", TormasiAsiakkaasen);
        AddCollisionHandler(pelaaja, "tuotteet", KerasiTuotteen);
        LuoPistelaskuri();
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
        PhysicsObject ympyra = new PhysicsObject(leveys, korkeus, Shape.Circle);
        ympyra.Position = RandomGen.NextVector(umpyra);
        ympyra.Hit(liike);
        peli.Add(ympyra);
        return ympyra;
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
        double leveys = RandomGen.NextDouble(45, 45);
        double korkeus = RandomGen.NextDouble(45, 45);
        Vector liike = RandomGen.NextVector(0, vauhti);
        PhysicsObject ympyra = new PhysicsObject(leveys, korkeus, Shape.Circle);
        ympyra.Position = RandomGen.NextVector(umpyra);
        ympyra.Color = Color.Blue;
        ympyra.Hit(liike);
        peli.Add(ympyra);
        return ympyra;
    }

    /// <summary>
    /// Laskee ympyrän pinta-alan ja palauttaa sen törmäysmetodille
    /// </summary>
    /// <param name="r">ympyrän säde</param>
    /// <returns>palauttaa ympyrän säteen</returns>
    /// <example>
    /// <pre name="test">
    ///     YmpyranAla(2) ~~ 12,566370614359;
    ///     YmpyranAla(4) ~~~ 50,265482457437;
    ///     YmpyranAla(0) ~~~ 0;
    /// </pre>
    /// </example>
    public static double YmpyranAla(double r)
    {
        return (Math.PI * (r * r));
    }

    /// <summary>
    /// Metodi, johon tullan, kun pelaaja osuu johonkin vihaiseen asiakkaaseen
    /// </summary>
    /// <param name="pelaaja"></param>
    /// <param name="vihu"></param>
    public void TormasiAsiakkaasen(PhysicsObject pelaaja, PhysicsObject vihu)
    {
        pelaaja.Destroy();
        /* double pelaajanPintaAla = YmpyranAla(pelaaja.Height);
         double vihunPintaAla = YmpyranAla(vihu.Height);

         if (pelaajanPintaAla<vihunPintaAla)
         {
             Explosion rajahdys = new Explosion(vihu.Width * 2);
             rajahdys.Position = vihu.Position;
             rajahdys.UseShockWave = false;
             return;

         } */
        Exit();
    }

    public void KerasiTuotteen(PhysicsObject pelaaja, PhysicsObject tuote)
    {
        tuote.Destroy();
        pistelaskuri.Value += 1;

    }

    void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);

        Label pistenaytto = new Label();
        pistenaytto.X = Screen.Left + 100;
        pistenaytto.Y = Screen.Top - 100;
        pistenaytto.TextColor = Color.Black;
        pistenaytto.Color = Color.White;
        pistenaytto.Title = "Pisteesi  ";

        pistenaytto.BindTo(pistelaskuri);
        Add(pistenaytto);
    }


}



