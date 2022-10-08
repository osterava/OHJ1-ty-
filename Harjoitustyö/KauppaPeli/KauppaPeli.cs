using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace KauppaPeli;

public class KauppaPeli : PhysicsGame
{
    public override void Begin()
    {
        BoundingRectangle alaosa = new BoundingRectangle(new Vector(Level.Left, 0), Level.BoundingRect.BottomRight);
        BoundingRectangle ylaosa = new BoundingRectangle(Level.BoundingRect.TopLeft, new Vector(Level.Right, 0));
        //TODO:BoundingRectangle kokopeli = new BoundingRectangle(Level.BoundingRect.BottomLeft, new Vector(Level.TopRight, 0)); ohjaus?

        for (int i = 0; i < 15; i++) // vihaiset asiakkat
        {
            PhysicsObject vihaisetAsiakkaat = LuoSatunaisetAsiakkaat(this, alaosa, 60);
            vihaisetAsiakkaat.Image = LoadImage("vihasiakas"); // creative commons kuva linkki https://fi.depositphotos.com/52847715/stock-photo-angry-customer-at-supermarket.html
                                                               // tekijä stokkete


        }

        for (int i = 0; i < 20; i++)
        {
            PhysicsObject tuotteet = LuoTuotteet(this, alaosa, 60);
            tuotteet.Image = LoadImage("tuotteet"); //Creative commons kuva linkki https://fi.depositphotos.com/47120155/stock-photo-shopping-basket-with-groceries.html
                                                    //Tekijä: chressiesjd

        }

        PhysicsObject pelaaja = LuoSatunaisetAsiakkaat(this, ylaosa, 50);
        pelaaja.Image = LoadImage("pelaaja");

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        Level.Background.Image = LoadImage("background.jpeg");
        Level.BackgroundColor = Color.White;
        Level.CreateBorders();

        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä avustus");
        Keyboard.Listen(Key.Up, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja ylös", pelaaja, new Vector(0, 200));
        Keyboard.Listen(Key.Down, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja alas", pelaaja, new Vector(0, -200));
        Keyboard.Listen(Key.Left, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja vasemmalle", pelaaja, new Vector(-200, 0));
        Keyboard.Listen(Key.Right, ButtonState.Pressed, LiikutaPelaajaa, "Pelaaja oikealle", pelaaja, new Vector(200, 0));


    }
    /// <summary>
    /// Aliohjelma luo vihamieliset asiakkaat
    /// </summary>
    /// <param name="peli">mihin peliin ympyrä luodaan</param>
    /// <param name="umpyra">minkä ymppyrän sisälle luodaan eli paikka</param>
    /// <param name="vauhti">ympyrän vauhti alussa</param>
    public static PhysicsObject LuoSatunaisetAsiakkaat(PhysicsGame peli, BoundingRectangle umpyra, double vauhti)
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
    public static PhysicsObject LuoTuotteet(PhysicsGame peli, BoundingRectangle umpyra, double vauhti)
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

}


